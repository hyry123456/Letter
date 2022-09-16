#ifndef CS_PARTICLE_NOISE_PASS
#define CS_PARTICLE_NOISE_PASS


#include "CS_ParticleInput.hlsl"
#include "../../../ShaderLibrary/Fragment.hlsl"

struct ToGeom {
    float3 worldPos : VAR_POSITION;
    bool isUse : CHECK;
    float4 transferData : UV_TRANSFER;
    float interplation : UV_INTERPELATION;
    float4 color : COLOR;
    float size : SIZE;
    float3 speed : SPEED;
};

//噪声运行模式的输入结构体
struct NoisePointToQuad {
    float3 worldPos;
    float4 uvTransfer;
    float uvInterplation;
    float size;
    float4 color;
    float3 speed;
};

CBUFFER_START(NoiseMaterial)
    float _TexAspectRatio;      //主纹理的宽高比
CBUFFER_END

ToGeom vert(uint id : SV_InstanceID)
{
    ToGeom o = (ToGeom)0;
    o.worldPos = _ParticleNoiseBuffer[id].worldPos;
    if (_ParticleNoiseBuffer[id].index.y < 0)
        o.isUse = false;
    else o.isUse = true;
    o.interplation = _ParticleNoiseBuffer[id].interpolation;
    o.transferData = _ParticleNoiseBuffer[id].uvTransData;
    o.color = _ParticleNoiseBuffer[id].color;
    o.size = _ParticleNoiseBuffer[id].size;
    o.speed = _ParticleNoiseBuffer[id].nowSpeed;
    return o;
}

//噪声的点到面
void NoiseOutOnePoint(inout TriangleStream<FragInput> tristream, NoisePointToQuad i) 
{
    FragInput o[4] = (FragInput[4])0;

    float3 worldVer = i.worldPos;
    float paritcleLen = i.size * _ParticleSize;

    float3 viewDir = normalize( _WorldSpaceCameraPos - worldVer );
    float3 particleNormal = cross(viewDir, i.speed);

    //左下
    float3 worldPos = worldVer + -i.speed * paritcleLen + -particleNormal * paritcleLen * _TexAspectRatio;
    o[0].pos = mul(UNITY_MATRIX_VP, float4(worldPos, 1));
    o[0].color = i.color;
    o[0].uv = GetUV(float2(0, 0), i.uvTransfer);
    o[0].interpolation = i.uvInterplation;

    worldPos = worldVer + -i.speed * paritcleLen + particleNormal * paritcleLen * _TexAspectRatio;
    o[1].pos = mul(UNITY_MATRIX_VP, float4(worldPos, 1));
    o[1].color = i.color;
    o[1].uv = GetUV(float2(1, 0), i.uvTransfer);
    o[1].interpolation = i.uvInterplation;

    worldPos = worldVer + i.speed * paritcleLen + -particleNormal * paritcleLen * _TexAspectRatio;
    o[2].pos = mul(UNITY_MATRIX_VP, float4(worldPos, 1));
    o[2].color = i.color;
    o[2].uv = GetUV(float2(0, 1), i.uvTransfer);
    o[2].interpolation = i.uvInterplation;

    worldPos = worldVer + i.speed * paritcleLen + particleNormal * paritcleLen * _TexAspectRatio;
    o[3].pos = mul(UNITY_MATRIX_VP, float4(worldPos, 1));
    o[3].color = i.color;
    o[3].uv = GetUV(float2(1, 1), i.uvTransfer);
    o[3].interpolation = i.uvInterplation;

    tristream.Append(o[1]);
    tristream.Append(o[2]);
    tristream.Append(o[0]);
    tristream.RestartStrip();

    tristream.Append(o[1]);
    tristream.Append(o[3]);
    tristream.Append(o[2]);
    tristream.RestartStrip();
}


void LoadOnePoint(ToGeom IN, inout TriangleStream<FragInput> tristream) {

    #ifdef _FELLOW_SPEED
        NoisePointToQuad o;
        o.worldPos = IN.worldPos;
        o.uvTransfer = IN.transferData;
        o.uvInterplation = IN.interplation;
        o.color = IN.color;
        o.size = IN.size;
        o.speed = normalize(IN.speed);
        NoiseOutOnePoint(tristream, o);
    #else
        PointToQuad o;
        o.worldPos = IN.worldPos;
        o.uvTransfer = IN.transferData;
        o.uvInterplation = IN.interplation;
        o.color = IN.color;
        o.size = IN.size;
        outOnePoint(tristream, o);
    #endif
}

[maxvertexcount(15)]
void geom(point ToGeom IN[1], inout TriangleStream<FragInput> tristream)
{
    if (!IN[0].isUse) return;
    LoadOnePoint(IN[0], tristream);
}

float4 frag(FragInput i) : SV_Target
{
    Fragment fragment = GetFragment(i.pos);
    float4 color = GetBaseColor(i);

    color.a *= ChangeAlpha(fragment);

    #ifdef _DISTORTION
        float4 bufferColor = GetBufferColor(fragment, GetDistortion(i, color.a));
        color.rgb = lerp( bufferColor.rgb, 
            color.rgb, saturate(color.a - GetDistortionBlend()));
    #endif

    return color;
}

#endif