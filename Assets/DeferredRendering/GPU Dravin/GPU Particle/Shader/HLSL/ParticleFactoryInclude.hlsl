struct ParticleNodeData
{
    float3 beginPos;        //该组粒子运行初始位置
    int3 initEnum;     //x:初始化的形状,y:是否使用重力，z:图片编号
    float2 sphereData;      //初始化球坐标需要的数据
    float3 cubeRange;       //初始化矩形坐标的范围
    float3 lifeTimeRange;   //生存周期的范围,x:随机释放时间,Y:存活时间,Z:最大生存到的时间
    float3 noiseData;       //噪声调整速度时需要的数据
    int3 outEnum;           //确定输出时算法的枚举
    float2 smoothRange;     //粒子的大小范围
};

// struct NoiseParticleData {
//     float4 random;          //xyz是随机数，w是目前存活时间
//     uint2 index;             //状态标记，x是图片编号，y是是否存活
//     float3 worldPos;        //当前位置
//     float4 uvTransData;     //uv动画需要的数据
//     float interpolation;    //插值需要的数据
//     float4 color;           //颜色值，包含透明度
//     float size;             //粒子大小
//     float3 nowSpeed;        //xyz是当前速度，w是存活时间
//     float liveTime;         //该粒子最多存活时间
// };


//单个粒子数据类型不变，但是将编号换为读取的图片编号
#include "ParticleInclude.hlsl"
#include "ParticleNoiseInc.hlsl"

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

//对单个粒子机型初始化
NoiseParticleData InitialFactory(ParticleNodeData origin){
    NoiseParticleData o = (NoiseParticleData)0;
    //首先初始化位置
    switch(origin.initEnum.x){
        case 0:
            o.worldPos = origin.beginPos;
            break;
        case 1:
            o.worldPos = origin.beginPos;

    }

}