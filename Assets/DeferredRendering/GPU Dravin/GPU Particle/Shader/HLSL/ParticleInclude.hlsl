#ifndef GPUPIPELINE_PARTICLE_INCLUDE
#define GPUPIPELINE_PARTICLE_INCLUDE

struct AnimateUVData {
    float4 uvData;			//uv偏移时需要的数据
    float interpolation;
};

//每个都只计算6组
#define _COUNT 6

float4 _Colors[_COUNT];  //颜色计算用的数据
float4 _Alphas[_COUNT];  //透明度计算用的数据
float4 _Sizes[_COUNT];  //大小

int _ParticleCount;     //每一组的粒子数量
float2 _LifeTime;        //存活时间
int _ArriveIndex;       //到达编号
float4 _Time;       //x=time, y=deltaTime, z=fixDeltaTime
int2 _UVCount;				//x是row，y是column


AnimateUVData AnimateUV(float time_01) {
    float sumTime = _UVCount.x * _UVCount.y;
    float midTime = time_01 * sumTime;

    float bottomTime = floor(midTime);
    float topTime = bottomTime + 1.0;
    float interpolation = (midTime - bottomTime) / (topTime - bottomTime);
    float bottomRow = floor(bottomTime / _UVCount.x);
    float bottomColumn = floor(bottomTime - bottomRow * _UVCount.y);
    float topRow = floor(topTime / _UVCount.x);
    float topColumn = floor(topTime - topRow * _UVCount.y);

    AnimateUVData animateUV;
    animateUV.uvData = float4(bottomColumn, -bottomRow, topColumn, -topRow);
    animateUV.interpolation = interpolation;
    return animateUV;
}

//根据时间获取颜色
float3 LoadColor(float time_01) {
    for (int i = 1; i < _COUNT; i++) {
        if (time_01 <= _Colors[i].w) {
            float radio = smoothstep(_Colors[i - 1].w, _Colors[i].w, time_01);
            return lerp(_Colors[i - 1].xyz, _Colors[i].xyz, radio);
        }
    }
    return 0;
}

float LoadAlpha(float time_01) {
    for (int i = 1; i < _COUNT; i++) {
        if (time_01 <= _Alphas[i].y) {
            float radio = smoothstep(_Alphas[i - 1].y, _Alphas[i].y, time_01);
            return lerp(_Alphas[i - 1].x, _Alphas[i].x, radio);
        }
    }
    return 0;
}

//时间控制函数，用来读取Curve中的值
float LoadSize(float time_01) {
    //有数据才循环
    for (int i = 1; i < _COUNT; i++) {
        //找到在范围中的
        if (time_01 <= _Sizes[i].x) {
            //Unity的Curve的曲线本质上是一个三次多项式插值，公式为：y = ax^3 + bx^2 + cx +d
            float a = (_Sizes[i - 1].w + _Sizes[i].z) * (_Sizes[i].x - _Sizes[i - 1].x)
                - 2 * (_Sizes[i].y - _Sizes[i - 1].y);
            float b = (-2 * _Sizes[i - 1].w - _Sizes[i].z) *
                (_Sizes[i].x - _Sizes[i - 1].x) + 3 * (_Sizes[i].y - _Sizes[i - 1].y);
            float c = _Sizes[i - 1].w * (_Sizes[i].x - _Sizes[i - 1].x);
            float d = _Sizes[i - 1].y;

            float trueTime = (time_01 - _Sizes[i - 1].x) / (_Sizes[i].x - _Sizes[i - 1].x);
            return a * pow(trueTime, 3) + b * pow(trueTime, 2) + c * trueTime + d;

        }
    }
    return 0;
}

#endif