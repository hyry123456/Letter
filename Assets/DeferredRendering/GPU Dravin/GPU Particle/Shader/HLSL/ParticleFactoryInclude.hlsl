struct ParticleNodeData
{
    float3 beginPos;        //该组粒子运行初始位置
    float3 beginSpeed;      //初始速度
    int3 initEnum;          //x:初始化的形状,y:是否使用重力，z:图片编号
    float2 sphereData;      //初始化球坐标需要的数据, x=角度, y=半径
    float3 cubeRange;       //初始化矩形坐标的范围, 分别表示xyz的偏移范围
    float3 lifeTimeRange;   //生存周期的范围,x:随机释放时间,Y:存活时间,Z:最大生存到的时间
    float3 noiseData;       //噪声调整速度时需要的数据, x:噪声采样次数, y:噪声采样频率, z:强度
    int3 outEnum;           //确定输出时算法的枚举，x:followSpeed? y:初始化速度方式
    float2 smoothRange;     //粒子的大小范围，用来对应size曲线的大小
    int2 uvCount;        //x:row，y:column,
    int2 drawData;     //x:颜色条编号,y是大小的编号
};

// struct NoiseParticleData {
//     float4 random;          //xyz是随机数，w是目前存活时间
//     uint2 index;             //状态标记，x是图片编号，y是是否存活
//     float3 worldPos;        //当前位置
//     float liveTime;         //该粒子最多存活时间
//     float3 nowSpeed;        //xyz是当前速度，w是存活时间

//     float4 uvTransData;     //uv动画需要的数据
//     float interpolation;    //插值需要的数据
//     float4 color;           //颜色值，包含透明度
//     float size;             //粒子大小
// };


//单个粒子数据类型不变，但是将编号换为读取的图片编号
#include "ParticleInclude.hlsl"
#include "ParticleNoiseInc.hlsl"

//设置最大支持的Graint数量为6个,因为太多了会造成显存浪费
#define _GRAINT_COUNT 36

float4 _GradientColor[_GRAINT_COUNT];  //颜色计算用的数据
float4 _GradientAlpha[_GRAINT_COUNT];  //透明度计算用的数据
float4 _GradientSizes[_GRAINT_COUNT];  //大小

RWStructuredBuffer<NoiseParticleData> _ParticlesBuffer;   //输入的buffer
RWStructuredBuffer<ParticleNodeData> _GroupNodeBuffer;    //每一组需要的粒子数

//不再使用矩阵采样，直接进行矩阵偏移
float3 GetSphereBeginPos(float2 random, float arc, float radius) {
    float u = lerp(0, arc, random.x);
    float v = lerp(0, arc, random.y);
    float3 pos;
    pos.x = radius * cos(u);
    pos.y = radius * sin(u) * cos(v);
    pos.z = radius * sin(u) * sin(v);
    return pos;
}
float3 GetCubeBeginPos(float3 random, float3 cubeRange){
    float3 begin = -cubeRange/2.0;
    float3 end = cubeRange/2.0;
    float3 pos = lerp(begin, end, random);
    return pos;
}

//对单个粒子进行初始化
void InitialFactory(ParticleNodeData origin, inout NoiseParticleData particle){
    float3 offsetPos;       //存储一下生成的偏移坐标，方便后面计算
    //首先初始化位置
    switch(origin.initEnum.x){
        // case 0:
        //     
        case 1:
            offsetPos = GetSphereBeginPos(particle.random.xy, origin.sphereData.x, origin.sphereData.y);
            particle.worldPos = origin.beginPos + offsetPos;
            break;
        case 2:
            offsetPos = GetCubeBeginPos(particle.random.xyz, origin.cubeRange);
            particle.worldPos = origin.beginPos + offsetPos;
            break;
        default:
            particle.worldPos = origin.beginPos;
            offsetPos = particle.random.xyz;
            break;
    }

    float speed = length(origin.beginSpeed) * particle.random.y;    //根据速度大小确定一个随机速度
    float3 normal = normalize(origin.beginSpeed);
    float3 direct = normalize(offsetPos);

    //初始化速度
    switch(origin.outEnum.y){

        case 1:     //速度是法线以及大小，在粒子位置生成一个垂直于法线且是向外的力度
            particle.nowSpeed = normalize( (direct - normal * dot(direct, normal)) ) * speed;
            break;
        case 2:     //在粒子位置生成一个垂直于法线且是向内的力度
            particle.nowSpeed = -normalize( (direct - normal * dot(direct, normal)) ) * speed;
            break;
        case 3:     //朝向起始位置的速度，也就是往中间汇集
            particle.nowSpeed = -normalize(offsetPos) * speed;
            break;
        case 4:     //离开起始位置的速度，也就是从中间往外面跑
            particle.nowSpeed = normalize(offsetPos) * speed;
            break;
        default: //默认模式,传入速度就是初始化速度
            particle.nowSpeed = origin.beginSpeed * particle.random.xyz;
            break;
    }

    //图片编号以及标记为存活
    particle.index = uint2(origin.initEnum.z, 1);
    //存活时间与最大生存时间初始化
    particle.random.w = 0;  //存活时间初始化
    particle.liveTime = origin.lifeTimeRange.y;
}

void UpdateFactory(inout NoiseParticleData particle){
    particle.worldPos += particle.nowSpeed * _Time.y;      //更新位置
    particle.random.w += _Time.y;
}

AnimateUVData AnimateUV(float time_01, int2 uvCount) {
    float sumTime = uvCount.x * uvCount.y;
    float midTime = time_01 * sumTime;

    float bottomTime = floor(midTime);
    float topTime = bottomTime + 1.0;
    float interpolation = (midTime - bottomTime) / (topTime - bottomTime);
    float bottomRow = floor(bottomTime / uvCount.x);
    float bottomColumn = floor(bottomTime - bottomRow * uvCount.y);
    float topRow = floor(topTime / uvCount.x);
    float topColumn = floor(topTime - topRow * uvCount.y);

    AnimateUVData animateUV;
    animateUV.uvData = float4(bottomColumn, -bottomRow, topColumn, -topRow);
    animateUV.interpolation = interpolation;
    return animateUV;
}

//根据时间获取颜色，以及对应的颜色编号确定颜色
float3 LoadColor(float time_01, ParticleNodeData origin) {
    for(int i = origin.drawData.x * 6 + 1; i < origin.drawData.x * 6 + 6; i++){
        if (time_01 <= _GradientColor[i].w) {
            float radio = smoothstep(_GradientColor[i - 1].w, _GradientColor[i].w, time_01);
            return lerp(_GradientColor[i - 1].xyz, _GradientColor[i].xyz, radio);
        }
    }
    return 0;
}

float LoadAlpha(float time_01, ParticleNodeData origin) {
    for(int i = origin.drawData.x * 6 + 1; i < origin.drawData.x * 6 + 6; i++){
        if (time_01 <= _GradientAlpha[i].y) {
            float radio = smoothstep(_GradientAlpha[i - 1].y, _GradientAlpha[i].y, time_01);
            return lerp(_GradientAlpha[i - 1].x, _GradientAlpha[i].x, radio);
        }
    }

    return 0;
}

//时间控制函数，用来读取Curve中的值
float LoadSize(float time_01, ParticleNodeData origin) {
    for(int i = origin.drawData.y * 6 + 1; i < origin.drawData.y * 6 + 6; i++){
        if(time_01 <= _GradientSizes[i].x){
            //Unity的Curve的曲线本质上是一个三次多项式插值，公式为：y = ax^3 + bx^2 + cx +d
            float a = (_GradientSizes[i - 1].w + _GradientSizes[i].z) * (_GradientSizes[i].x - _GradientSizes[i - 1].x)
                - 2 * (_GradientSizes[i].y - _GradientSizes[i - 1].y);
            float b = (-2 * _GradientSizes[i - 1].w - _GradientSizes[i].z) *
                (_GradientSizes[i].x - _GradientSizes[i - 1].x) + 3 * (_GradientSizes[i].y - _GradientSizes[i - 1].y);
            float c = _GradientSizes[i - 1].w * (_GradientSizes[i].x - _GradientSizes[i - 1].x);
            float d = _GradientSizes[i - 1].y;

            float trueTime = (time_01 - _GradientSizes[i - 1].x) / (_GradientSizes[i].x - _GradientSizes[i - 1].x);
            return a * pow(trueTime, 3) + b * pow(trueTime, 2) + c * trueTime + d;
        }
    }
    return 0;
}


void UpdataSpeed(inout NoiseParticleData i, ParticleNodeData origin){
    i.nowSpeed += CurlNoise3D(i.worldPos * origin.noiseData.y, (int)origin.noiseData.x) * origin.noiseData.z * _Time.z;
    // i.nowSpeed += CurlNoise3D(i.worldPos, 3) * 10 * _Time.z;
}