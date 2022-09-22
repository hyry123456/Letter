struct ParticleNodeData
{
    float3 beginPos;        //该组粒子运行初始位置
    int3 initEnum;     //x:初始化的形状,y:是否使用重力，z:暂定
    float2 sphereData;      //初始化球坐标需要的数据
    float3 cubeRange;       //初始化矩形坐标的范围
    float3 lifeTimeRange;   //生存周期的范围,x:随机释放时间,Y:存活时间,Z:最大生存到的时间
    float3 noiseData;       //噪声调整速度时需要的数据
    int3 outEnum;      //确定输出时算法的枚举
    float2 smoothRange;     //粒子的大小范围
};


//单个粒子数据类型不变，但是将编号换为读取的图片编号
#include "ParticleInclude.hlsl"
#include "ParticleNoiseInc.hlsl"

RWStructuredBuffer<NoiseParticleData> _ParticlesBuffer;   //输入的buffer
RWStructuredBuffer<ParticleNodeData> _GroupNodeBuffer;    //每一组需要的粒子数