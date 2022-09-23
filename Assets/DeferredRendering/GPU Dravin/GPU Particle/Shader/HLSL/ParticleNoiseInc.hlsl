#ifndef GPUPIPELINE_PARTICLE_NOISE_INCLUDE
#define GPUPIPELINE_PARTICLE_NOISE_INCLUDE

struct NoiseParticleData {
    float4 random;          //xyz是随机数，w是目前存活时间
    int2 index;             //状态标记，x是当前编号，y是是否存活
    float3 worldPos;        //当前位置
    float4 uvTransData;     //uv动画需要的数据
    float interpolation;    //插值需要的数据
    float4 color;           //颜色值，包含透明度
    float size;             //粒子大小
    float3 nowSpeed;        //xyz是当前速度，w是存活时间
    float liveTime;         //最多存活时间
};


struct Par_Initi_Data{
    float3 beginPos;        //该组粒子运行时间
    float3 velocityBeg;     //初始速度范围
    float3 velocityEnd;     //初始速度范围
    int3 InitEnum;           //初始化判断用的枚举编号根据值

    float2 sphereData;      //初始化球坐标需要的数据
    float3 cubeRange;       //初始化矩形坐标的范围
    float4x4 transfer_M;       //初始化矩形坐标的范围
    float2 lifeTimeRange;   //生存周期的范围

    float3 noiseData;       //噪声调整速度时需要的数据

    int3 outEnum;      //确定输出时算法的枚举
    float2 smoothRange;       //粒子的大小范围

    uint arriveIndex;
};


#define _PI 3.1415926


RWStructuredBuffer<NoiseParticleData> _ParticleNoiseBuffer;   //输入的buffer
RWStructuredBuffer<Par_Initi_Data> _InitializeBuffer;         //每一组需要的粒子数

float _Arc;         //设置的角度，圆形初始化位置时用到该数据
float _Radius;      //球的半径

float3 _CubeRange;  //立方体范围


int _Octave;        //循环的次数，控制生成噪声的细节
float _Frequency;   //采样变化的频率，也就是对坐标的缩放大小
float _Intensity;   //影响的强度值
float4 _SizeBySpeed;    //确定是否大小随时间变化


//使用参数方程作为坐标生成的根据
float3 GetSphereBeginPos(float2 random, float arc, float radius, float4x4 transfer_M) {
    float u = lerp(0, arc, random.x);
    float v = lerp(0, arc, random.y);
    float3 pos;
    pos.x = radius * cos(u);
    pos.y = radius * sin(u) * cos(v);
    pos.z = radius * sin(u) * sin(v);

    return mul(transfer_M, float4(pos, 1)).xyz;
}

float3 GetCubeBeginPos(float3 random, float3 cubeRange, float4x4 transfer_M){
    float3 begin = -cubeRange/2.0;
    float3 end = cubeRange/2.0;
    float3 pos = lerp(begin, end, random);
    return mul(transfer_M, float4(pos, 1)).xyz;
}

NoiseParticleData InitialParticle(NoiseParticleData i, Par_Initi_Data init) {
    switch (init.InitEnum.x){
        case 0:
            i.worldPos = init.beginPos;
            break;
        case 1:
            i.worldPos = GetSphereBeginPos(i.random.xy, init.sphereData.x, init.sphereData.y, init.transfer_M);
            break;
        case 2:
            i.worldPos = GetCubeBeginPos(i.random.xyz, init.cubeRange, init.transfer_M);
            break;
    }
    //改变取值位置，让速度与位置不要这么随机
    i.nowSpeed = lerp(init.velocityBeg, init.velocityEnd, i.random.yzx);
    i.random.w = 0;
    return i;
}



//生成随机方向
float3 hash3d(float3 input) {
    const float3 k = float3(0.3183099, 0.3678794, 0.38975765);
    input = input * k + k.zyx;
    return -1.0 + 2.0 * frac(16.0 * k * frac(input.x * input.y * input.z * (input.x + input.y + input.z)));
}

//进行插值
float Cos_Interpolate(float a, float b, float t)
{
    float ft = t * 3.14159;
    t = (1 - cos(ft)) * 0.5;
    return a * (1 - t) + t * b;
}

//根据3维坐标生成一个float值
float Perlin3DFun(float3 pos) {
    float3 i = floor(pos);
    float3 f = frac(pos);

    //获得八个点，也就是立方体的八个点的对应向量
    float3 g0 = hash3d(i + float3(0.0, 0.0, 0.0));
    float3 g1 = hash3d(i + float3(1.0, 0.0, 0.0));
    float3 g2 = hash3d(i + float3(0.0, 1.0, 0.0));
    float3 g3 = hash3d(i + float3(0.0, 0.0, 1.0));
    float3 g4 = hash3d(i + float3(1.0, 1.0, 0.0));
    float3 g5 = hash3d(i + float3(0.0, 1.0, 1.0));
    float3 g6 = hash3d(i + float3(1.0, 0.0, 1.0));
    float3 g7 = hash3d(i + float3(1.0, 1.0, 1.0));

    //获得点乘后的大小
    float v0 = dot(g0, f - float3(0.0, 0.0, 0.0));  //左前下
    float v1 = dot(g1, f - float3(1.0, 0.0, 0.0));  //右前下
    float v2 = dot(g2, f - float3(0.0, 1.0, 0.0));  //左前上
    float v3 = dot(g3, f - float3(0.0, 0.0, 1.0));  //左后下
    float v4 = dot(g4, f - float3(1.0, 1.0, 0.0));  //右前上
    float v5 = dot(g5, f - float3(0.0, 1.0, 1.0));  //左后上
    float v6 = dot(g6, f - float3(1.0, 0.0, 1.0));  //右后下
    float v7 = dot(g7, f - float3(1.0, 1.0, 1.0));  //右后上

    float inter0 = Cos_Interpolate(v0, v2, f.y);
    float inter1 = Cos_Interpolate(v1, v4, f.y);
    float inter2 = Cos_Interpolate(inter0, inter1, f.x);    //前4点

    float inter3 = Cos_Interpolate(v3, v5, f.y);
    float inter4 = Cos_Interpolate(v6, v7, f.y);
    float inter5 = Cos_Interpolate(inter3, inter4, f.x);

    float inter6 = Cos_Interpolate(inter2, inter5, f.z);

    return inter6;
}

//采样噪声，通过参数确定是否多次采样
float Perlin3DFBM(float3 pos, int octave) {
    float noise = 0.0;
    float frequency = 1.0;
    float amplitude = 1.0;

    for (int i = 0; i < octave; i++)
    {
        noise += Perlin3DFun(pos * frequency) * amplitude;
        frequency *= 2.0;
        amplitude *= 0.5;
    }
    return noise;
}

//根据坐标生成一个方向
float3 CurlNoise3D(float3 pos, int octave)
{
    float eps = 0.1;
    float x = pos.x;
    float y = pos.y;
    float z = pos.z;
    float n1 = Perlin3DFBM(float3(x, y + eps, z), octave);
    float n2 = Perlin3DFBM(float3(x, y - eps, z), octave).x;
    float a = (n1 - n2) / (2.0 * eps);

    float n3 = Perlin3DFBM(float3(x + eps, y, z), octave).x;
    float n4 = Perlin3DFBM(float3(x - eps, y, z), octave).x;
    float b = (n3 - n4) / (2.0 * eps);

    float n5 = Perlin3DFBM(float3(x, y, z + eps), octave).x;
    float n6 = Perlin3DFBM(float3(x, y, z - eps), octave).x;
    float c = (n5 - n6) / (2.0 * eps);

    return float3(a, b, c);
}

NoiseParticleData UpdataPosition(NoiseParticleData i, Par_Initi_Data init){
    i.worldPos += i.nowSpeed * _Time.y;
    i.random.w += _Time.y;
    return i;
}

NoiseParticleData UpdataSpeed(NoiseParticleData i, Par_Initi_Data init){
    i.nowSpeed += CurlNoise3D(i.worldPos * init.noiseData.y, (int)init.noiseData.x) * init.noiseData.z * _Time.z;
    return i;
}

NoiseParticleData OutParticle(NoiseParticleData i, Par_Initi_Data init){
    float time_01 = saturate( i.random.w / i.liveTime );
    AnimateUVData uvData = AnimateUV(time_01);
    i.uvTransData = uvData.uvData;
    i.interpolation = uvData.interpolation;
    i.color = float4(LoadColor(time_01), LoadAlpha(time_01));
    switch (init.outEnum.x) {
    case 1 :
        i.size = LoadSize(smoothstep(init.smoothRange.x, init.smoothRange.y, abs(i.nowSpeed.x)));
        break;
    case 2 :
        i.size = LoadSize(smoothstep(init.smoothRange.x, init.smoothRange.y, abs(i.nowSpeed.y)));
        break;
    case 3:
        i.size = LoadSize(smoothstep(init.smoothRange.x, init.smoothRange.y, abs(i.nowSpeed.z)));
        break;
    default :
        i.size = LoadSize(time_01);
        break;
    }
    return i;
}
#endif