#ifndef DEFFER_POST_INPUT
#define DEFFER_POST_INPUT


//获得主纹理的采用模式，因为需要采集雾效
SAMPLER(sampler_PostFXSource);
TEXTURE2D(_PostFXSource);
TEXTURE2D(_PostFXSource2);

TEXTURE2D(_CameraNormalTexture);
SAMPLER(sampler_CameraNormalTexture);

TEXTURE2D(_GBufferColorTex);
SAMPLER(sampler_GBufferColorTex);
TEXTURE2D(_GBufferNormalTex);
SAMPLER(sampler_GBufferNormalTex);
TEXTURE2D(_GBufferDepthTex);
TEXTURE2D(_GBufferSpecularTex);
TEXTURE2D(_GBufferBakeTex);


float4x4 _FrustumCornersRay;
float4x4 _InverseProjectionMatrix;
float4x4 _WorldToCamera;
float4x4 _ViewToScreenMatrix;
float4x4 _InverseVPMatrix;

float4 _ScreenSize;
int _MaxRayMarchingStep;
float _RayMarchingStepSize;
float _MaxRayMarchingDistance;
float _DepthThickness;

float4 _PostFXSource_TexelSize;
bool _BloomBicubicUpsampling;
float _BloomIntensity;
float4 _BloomThreshold;

float _BulkLightCheckMaxDistance;
float _BulkSampleCount;
float _BulkLightShrinkRadio;
float _BulkLightScatterRadio;

float _BilaterFilterFactor;
float4 _BlurRadius;

#define _COUNT 6

float _FogMaxDepth;
float _FogMinDepth;
float _FogDepthFallOff;
float _FogMaxHight;
float _FogMinHight;
float _FogPosYFallOff;

float4 _Colors[_COUNT];  //颜色计算用的数据

float4 GetSourceTexelSize () {
	return _PostFXSource_TexelSize;
}

float4 GetSource(float2 screenUV) {
	return SAMPLE_TEXTURE2D_LOD(_PostFXSource, sampler_linear_clamp, screenUV, 0);
}

float4 GetSourceBicubic (float2 screenUV) {
	return SampleTexture2DBicubic(
		TEXTURE2D_ARGS(_PostFXSource, sampler_linear_clamp), screenUV,
		_PostFXSource_TexelSize.zwxy, 1.0, 0.0
	);
}

float4 GetSource2(float2 screenUV) {
	return SAMPLE_TEXTURE2D_LOD(_PostFXSource2, sampler_linear_clamp, screenUV, 0);
}

float3 GetWorldPos(float depth, float2 uv){
    #if defined(UNITY_REVERSED_Z)
        depth = 1 - depth;
    #endif
	float4 ndc = float4(uv.x * 2 - 1, uv.y * 2 - 1, depth * 2 - 1, 1);

	float4 worldPos = mul(_InverseVPMatrix, ndc);
	worldPos /= worldPos.w;
	return worldPos.xyz;
}


#define random(seed) sin(seed * 641.5467987313875 + 1.943856175)


void swap(inout float v0, inout float v1)
{
    float temp = v0;
    v0 = v1;
    v1 = temp;
}

float distanceSquared(float2 A, float2 B)
{
    A -= B;
    return dot(A, A);
}

bool screenSpaceRayMarching(float3 rayOri, float3 rayDir, inout float2 hitScreenPos)
{
    //反方向反射的，本身也看不见，索性直接干掉
     if (rayDir.z > 0.0)
         return false;
    //首先求得视空间终点位置，不超过最大距离
    float magnitude = _MaxRayMarchingDistance;
    float end = rayOri.z + rayDir.z * magnitude;
    //如果光线反过来超过了近裁剪面，需要截取到近裁剪面
    if (end > -_ProjectionParams.y)
        magnitude = (-_ProjectionParams.y - rayOri.z) / rayDir.z;
    float3 rayEnd = rayOri + rayDir * magnitude;
    //直接把cliptoscreen与projection矩阵结合，得到齐次坐标系下屏幕位置
    float4 homoRayOri = mul(_ViewToScreenMatrix, float4(rayOri, 1.0));
    float4 homoRayEnd = mul(_ViewToScreenMatrix, float4(rayEnd, 1.0));
    //w
    float kOri = 1.0 / homoRayOri.w;
    float kEnd = 1.0 / homoRayEnd.w;
    //屏幕空间位置
    float2 screenRayOri = homoRayOri.xy * kOri;
    float2 screenRayEnd = homoRayEnd.xy * kEnd;
    screenRayEnd = (distanceSquared(screenRayEnd, screenRayOri) < 0.0001) ? screenRayOri + float2(0.01, 0.01) : screenRayEnd;
    
    float3 QOri = rayOri * kOri;
    float3 QEnd = rayEnd * kEnd;
    
    float2 displacement = screenRayEnd - screenRayOri;
    bool permute = false;
    if (abs(displacement.x) < abs(displacement.y))
    {
        permute = true;
        
        displacement = displacement.yx;
        screenRayOri.xy = screenRayOri.yx;
        screenRayEnd.xy = screenRayEnd.yx;
    }
    float dir = sign(displacement.x);
    float invdx = dir / displacement.x;
    float2 dp = float2(dir, invdx * displacement.y) * _RayMarchingStepSize;
    float3 dq = (QEnd - QOri) * invdx * _RayMarchingStepSize;
    float  dk = (kEnd - kOri) * invdx * _RayMarchingStepSize;
    float rayZmin = rayOri.z;
    float rayZmax = rayOri.z;
    float preZ = rayOri.z;
    
    float2 screenPoint = screenRayOri;
    float3 Q = QOri;
    float k = kOri;

    float random = random((rayDir.y + rayDir.x) * _ScreenParams.x * _ScreenParams.y + 0.2312312);

    dq *= lerp(0.9, 1, random);
    dk *= lerp(0.9, 1, random);

    for(int i = 0; i < _MaxRayMarchingStep; i++)
    {
        //向前步进一个单位
        screenPoint += dp;
        Q.z += dq.z;
        k += dk;
        
        //得到步进前后两点的深度
        rayZmin = preZ;
        rayZmax = (dq.z * 0.5 + Q.z) / (dk * 0.5 + k);
        preZ = rayZmax;
        if (rayZmin > rayZmax)
        {
            swap(rayZmin, rayZmax);
        }
        
        //得到当前屏幕空间位置，交换过的xy换回来，并且根据像素宽度还原回（0,1）区间而不是屏幕区间
        hitScreenPos = permute ? screenPoint.yx : screenPoint;
        hitScreenPos *= _ScreenSize.xy;
        
        //转换回屏幕（0,1）区间，剔除出屏幕的反射
        if (any(hitScreenPos.xy < 0.0) || any(hitScreenPos.xy > 1.0))
            return false;
        
        //采样当前点深度图，转化为视空间的深度（负值）
        float bufferDepth = SAMPLE_DEPTH_TEXTURE_LOD(_GBufferDepthTex, sampler_point_clamp, hitScreenPos, 0);
        float depth = -LinearEyeDepth(bufferDepth, _ZBufferParams);
        
        bool isBehand = (rayZmin <= depth);
        bool intersecting = isBehand && (rayZmax >= depth - _DepthThickness);
        
        if (intersecting)
            return true;
    }
    return false;
}

float3 GetBulkLight(float depth, float2 screenUV, float3 interpolatedRay){
    float bufferDepth = IsOrthographicCamera() ? OrthographicDepthBufferToLinear(depth) 
		: LinearEyeDepth(depth, _ZBufferParams);

    float3 worldPos = _WorldSpaceCameraPos + bufferDepth * interpolatedRay;
    float3 startPos = _WorldSpaceCameraPos + _ProjectionParams.y * interpolatedRay;

    float3 direction = normalize(worldPos - startPos);
    float dis = length(worldPos - startPos);

    float m_length = min(_BulkLightCheckMaxDistance, dis);
    float perNodeLength = m_length / _BulkSampleCount;
    float perDepthLength = bufferDepth / _BulkSampleCount;
    float3 currentPoint = startPos;
    float3 viewDirection = normalize(_WorldSpaceCameraPos - worldPos);

    float3 color = 0;
    float seed = random((screenUV.y + screenUV.x) * _ScreenParams.x * _ScreenParams.y + 0.2312312);
    float currentDepth = 0;

    for(int i=0; i<_BulkSampleCount; i++){
        currentPoint += direction * perNodeLength;
        currentDepth += perDepthLength;
        float3 tempPosition = lerp(currentPoint, currentPoint + direction * perNodeLength, seed);
        color += GetBulkLighting(tempPosition, viewDirection, screenUV, _BulkLightScatterRadio, currentDepth);
    }
    color *= m_length * _BulkLightShrinkRadio ;

    return color;
}

half LinearRgbToLuminance(half3 linearRgb)
{
    return dot(linearRgb, half3(0.2126729f,  0.7151522f, 0.0721750f));
}

float CompareColor(float4 col1, float4 col2)
{
	float l1 = LinearRgbToLuminance(col1.rgb);
	float l2 = LinearRgbToLuminance(col2.rgb);
	return smoothstep(_BilaterFilterFactor, 1.0, 1.0 - abs(l1 - l2));
}

float3 LoadColor(float time_01) {
    for (int i = 1; i < _COUNT; i++) {
        if (time_01 <= _Colors[i].w) {
            float radio = smoothstep(_Colors[i - 1].w, _Colors[i].w, time_01);
            return lerp(_Colors[i - 1].xyz, _Colors[i].xyz, radio);
        }
    }
    return 0;
}



#endif