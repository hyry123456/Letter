//通过Compute Shader计算的粒子系统的输入文件
#ifndef CS_PARTICLE_INPUT
#define CS_PARTICLE_INPUT

#include "../../../ShaderLibrary/Common.hlsl"

TEXTURE2D(_MainTex);
TEXTURE2D(_DistortionTex);
SAMPLER(sampler_MainTex);
float4 _MainTex_ST;

//由于本身是使用DrawProcedural，没有必要GPU实例化
CBUFFER_START(UnityPerMaterial)
    float4 _ParticleColor;
    //偏移主纹理
    #ifdef _DISTORTION
        float _DistortionSize;
        float _DistortionBlend;
    #endif

    //软粒子
    #ifdef _SOFT_PARTICLE
        float _SoftParticlesDistance;
        float _SoftParticlesRange;
    #endif

    //近平面透明
    #ifdef _NEAR_ALPHA
        float _NearFadeDistance;
        float _NearFadeRange;
    #endif


CBUFFER_END
int _RowCount;
int _ColCount;
float _ParticleSize;


//CS传入的结构体
struct ParticleData {
    float4 random;          //随机方向以及时间
    int2 index;             //粒子编号以及是否显示
    float3 worldPos;
    float4 uvTransData;     //uv动画需要的数据
    float interpolation;    //动画插值
    float4 color;           //当前颜色值，包含透明度
    float size;             //当前粒子大小
};

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

StructuredBuffer<ParticleData> _ParticleBuffer;     //计算的根据buffer
StructuredBuffer<NoiseParticleData> _ParticleNoiseBuffer;         //输入的buffer


//顶点生成平面需要赋值的数据
struct PointToQuad {
    float3 worldPos;
    float4 uvTransfer;
    float uvInterplation;
    float size;
    float4 color;
};

struct FragInput
{
    float4 color : VAR_COLOR;
    float4 pos : SV_POSITION;
    float4 uv : TEXCOORD0;
    float interpolation : UV_INTERPELATION;
};


//计算uv，包含uv动画
float4 GetUV(float2 uv, float4 uvTransData) {
    uv = uv * _MainTex_ST.xy + _MainTex_ST.zw;  //缩放uv
    float4 reUV;
    reUV.xy = uv + uvTransData.xy;
    reUV.zw = uv + uvTransData.zw;

    reUV.xz /= _RowCount;
    reUV.yw /= _ColCount;
    return reUV;
}

//封装点生成面
void outOnePoint(inout TriangleStream<FragInput> tristream, PointToQuad i) 
{
    FragInput o[4] = (FragInput[4])0;

    float3 worldVer = i.worldPos;
    // float3 worldVer = 0;
    float paritcleLen = i.size * _ParticleSize;
    // float paritcleLen = 10;

    float3 worldPos = worldVer + -unity_MatrixV[0].xyz * paritcleLen + -unity_MatrixV[1].xyz * paritcleLen;
    o[0].pos = mul(UNITY_MATRIX_VP, float4(worldPos, 1));
    // o[0].time = i.time;
    o[0].color = i.color;
    o[0].uv = GetUV(float2(0, 0), i.uvTransfer);
    o[0].interpolation = i.uvInterplation;
    // o[0].temData = i.size;

    worldPos = worldVer + UNITY_MATRIX_V[0].xyz * -paritcleLen
        + UNITY_MATRIX_V[1].xyz * paritcleLen;
    o[1].pos = mul(UNITY_MATRIX_VP, float4(worldPos, 1));
    // o[1].time = i.time;
    o[1].color = i.color;
    o[1].uv = GetUV(float2(1, 0), i.uvTransfer);
    o[1].interpolation = i.uvInterplation;
    // o[1].temData = i.size;

    worldPos = worldVer + UNITY_MATRIX_V[0].xyz * paritcleLen
        + UNITY_MATRIX_V[1].xyz * -paritcleLen;
    o[2].pos = mul(UNITY_MATRIX_VP, float4(worldPos, 1));
    // o[2].time = i.time;
    o[2].color = i.color;
    o[2].uv = GetUV(float2(0, 1), i.uvTransfer);
    o[2].interpolation = i.uvInterplation;
    // o[2].temData = i.size;

    worldPos = worldVer + UNITY_MATRIX_V[0].xyz * paritcleLen
        + UNITY_MATRIX_V[1].xyz * paritcleLen;
    o[3].pos = mul(UNITY_MATRIX_VP, float4(worldPos, 1));
    // o[3].time = i.time;
    o[3].color = i.color;
    o[3].uv = GetUV(float2(1, 1), i.uvTransfer);
    o[3].interpolation = i.uvInterplation;
    // o[3].temData = i.size;

    tristream.Append(o[1]);
    tristream.Append(o[2]);
    tristream.Append(o[0]);
    tristream.RestartStrip();

    tristream.Append(o[1]);
    tristream.Append(o[3]);
    tristream.Append(o[2]);
    tristream.RestartStrip();
}

float4 GetBaseColor(FragInput i){
    float4 color1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy);
    float4 color2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.zw);
    return lerp(color1, color2, i.interpolation) * i.color;
}

#ifdef _DISTORTION
    float2 GetDistortion(FragInput i, float baseAlpha){
        float2 color1 = SAMPLE_TEXTURE2D(_DistortionTex, sampler_MainTex, i.uv.xy).xy;
        float2 color2 = SAMPLE_TEXTURE2D(_DistortionTex, sampler_MainTex, i.uv.zw).xy;
        float2 color = lerp(color1, color2, i.interpolation) * i.color.xy;
        return color.xy * baseAlpha * _DistortionSize;
    }

    float GetDistortionBlend(){
        return _DistortionBlend;
    }
#endif

float ChangeAlpha(Fragment fragment){
    float reAlpha = 1;

    #ifdef _NEAR_ALPHA
        reAlpha = saturate( (fragment.depth - _NearFadeDistance) / _NearFadeRange );
    #endif

    #ifdef _SOFT_PARTICLE
        float depthDelta = fragment.bufferDepth - fragment.depth;	//获得深度差
        reAlpha *= (depthDelta - _SoftParticlesDistance) / _SoftParticlesRange;	//进行透明控制
    #endif
    return saturate( reAlpha );
}

#endif