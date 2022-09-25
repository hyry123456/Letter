#ifndef DEFFER_POST_PASS
#define DEFFER_POST_PASS

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
#include "../../ShaderLibrary/Surface.hlsl"
#include "../../ShaderLibrary/Shadows.hlsl"
#include "../../ShaderLibrary/Light.hlsl"
#include "../../ShaderLibrary/BRDF.hlsl"
#include "../../ShaderLibrary/GI.hlsl"
#include "../../ShaderLibrary/Lighting.hlsl"
#include "PostFXInput.hlsl"

struct Attributes{
	float3 positionOS : POSITION;
	float2 baseUV : TEXCOORD0;
};

struct Varyings {
    float4 positionCS_SS : SV_POSITION;
    float2 screenUV : VAR_SCREEN_UV;
    float2 screenUV_depth : VAR_SCREEN_UV_DEPTH;
    float4 interpolatedRay : TEXCOORD2;
    float3 viewRay : VAR_VIEWRAY;
};

Varyings BlitPassSimpleVertex(Attributes input){
    Varyings output = (Varyings)0;
    output.positionCS_SS = TransformObjectToHClip(input.positionOS);
    output.screenUV = input.baseUV;
    return output;
}

Varyings BlitPassRayVertex(Attributes input){
    Varyings output = (Varyings)0;
    output.positionCS_SS = TransformObjectToHClip(input.positionOS);
    output.screenUV = input.baseUV;
    output.screenUV_depth = input.baseUV;

    #if UNITY_UV_STARTS_AT_TOP
        if (_PostFXSource_TexelSize.y < 0)
            output.screenUV_depth.y = 1 - output.screenUV_depth.y;
    #endif

    int index = 0;
    if (output.screenUV.x < 0.5 && output.screenUV.y < 0.5) {         //位于左下方区域，使用左下方的方向
        index = 0;
    }
    else if (output.screenUV.x > 0.5 && output.screenUV.y < 0.5) {    //位于右下方
        index = 1;
    }
    else if (output.screenUV.x > 0.5 && output.screenUV.y > 0.5) {    //右上方
        index = 2;
    }
    else {                                                  //左上方
        index = 3;
    }

    #if UNITY_UV_STARTS_AT_TOP
    if (_PostFXSource_TexelSize.y < 0)
        index = 3 - index;
    #endif

    output.interpolatedRay = _FrustumCornersRay[index];
    return output;
}


Varyings SSRPassVertex(Attributes input){
    Varyings output = (Varyings)0;
    output.positionCS_SS = TransformObjectToHClip(input.positionOS);
    output.screenUV = input.baseUV;

    float4 clipPos = float4(input.baseUV * 2 - 1.0, 1.0, 1.0);
    float4 viewRay = mul(_InverseProjectionMatrix, clipPos);
    output.viewRay = viewRay.xyz / viewRay.w;
    return output;
}

float4 SSS_Fragment(Varyings i) : SV_Target{
    float bufferDepth = SAMPLE_DEPTH_TEXTURE_LOD(_GBufferDepthTex, sampler_point_clamp, i.screenUV, 0);
    float linear01Depth = Linear01Depth(bufferDepth, _ZBufferParams);


    float3 normalWS = SAMPLE_TEXTURE2D(_GBufferNormalTex, sampler_GBufferNormalTex, i.screenUV).xyz * 2.0 - 1.0;     //法线
    float3 normalVS = normalize( mul((float3x3)_WorldToCamera, normalWS) );

    float3 positionVS = linear01Depth * i.viewRay;
    float3 viewDir = normalize(positionVS);

    float3 reflectDir = reflect(viewDir, normalVS);

    float2 hitScreenPos = float2(-1, -1);
    float4 reflectTex = 0;
    float4 specular = SAMPLE_TEXTURE2D(_GBufferSpecularTex, sampler_PostFXSource, i.screenUV);                   //PBR数据
    if(specular.w > 0.0){
        if (screenSpaceRayMarching(positionVS, reflectDir, hitScreenPos))
        {
            reflectTex = SAMPLE_TEXTURE2D_LOD(_PostFXSource, sampler_PostFXSource, hitScreenPos, 0);
        }
		else {
			reflectTex = float4( GetSkyBox(reflectDir), 1);
		}
    }
    return reflectTex;
}

float4 DrawGBufferColorFragment(Varyings i) : SV_Target
{
    float bufferDepth = SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_point_clamp, i.screenUV, 0);
	float depth01 = Linear01Depth(bufferDepth, _ZBufferParams);
    bufferDepth = IsOrthographicCamera() ? OrthographicDepthBufferToLinear(bufferDepth)
		: LinearEyeDepth(bufferDepth, _ZBufferParams);
    Surface surface;
    surface.position = _WorldSpaceCameraPos + bufferDepth * i.interpolatedRay.xyz;

	float4 baseNormal = SAMPLE_TEXTURE2D(_CameraNormalTexture, sampler_CameraNormalTexture, i.screenUV);

    surface.normal = baseNormal.xyz * 2.0 - 1.0;     //法线

    surface.viewDirection = normalize(_WorldSpaceCameraPos - surface.position);                                     //视线方向
    surface.depth = bufferDepth;                                                                                    //深度

    float4 baseColor = SAMPLE_TEXTURE2D(_GBufferColorTex, sampler_GBufferColorTex, i.screenUV);                     //颜色
	surface.color = baseColor.rgb;
	surface.alpha = baseColor.a;

    float4 specular = SAMPLE_TEXTURE2D(_GBufferSpecularTex, sampler_GBufferColorTex, i.screenUV);                   //PBR数据
    surface.metallic = specular.x;
    surface.smoothness = specular.y;
    surface.fresnelStrength = specular.z;
    surface.dither = InterleavedGradientNoise(i.positionCS_SS.xy, 0);

    float4 bakeColor = SAMPLE_TEXTURE2D(_GBufferBakeTex, sampler_GBufferColorTex, i.screenUV);

	surface.shiftColor = float3(baseColor.w, baseNormal.w, bakeColor.w);
	surface.width = specular.w;

    BRDF brdf = GetBRDF(surface);


    float3 color;
    if(specular.w > 0.0){
		float3 uv_Depth = float3(i.screenUV, bufferDepth);
        color = GetGBufferLight(surface, brdf, uv_Depth);
		float3 reflect = ReflectLod(i.screenUV, 1.0 - surface.smoothness);
		color.xyz += ReflectBRDF(surface.normal, surface.viewDirection, surface.fresnelStrength, brdf, reflect);
    }
    else{
        color = baseColor.rgb;
    }
    color += bakeColor.rgb;

    return float4(color, 1);
}

float4 BulkLightFragment(Varyings input) : SV_TARGET{
    float bufferDepth = SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_point_clamp, input.screenUV, 0);
	float3 bulkLight = GetBulkLight(bufferDepth, input.screenUV, input.interpolatedRay.xyz);

	return float4(bulkLight, 1);
}

float4 BilateralFilterFragment (Varyings input) : SV_TARGET{
	float2 delta = _PostFXSource_TexelSize.xy * _BlurRadius.xy;
	//采集Normal的颜色值
	float4 col =   SAMPLE_TEXTURE2D(_PostFXSource, sampler_linear_clamp, input.screenUV);
	float4 col0a = SAMPLE_TEXTURE2D(_PostFXSource, sampler_linear_clamp, input.screenUV - delta);
	float4 col0b = SAMPLE_TEXTURE2D(_PostFXSource, sampler_linear_clamp, input.screenUV + delta);
	float4 col1a = SAMPLE_TEXTURE2D(_PostFXSource, sampler_linear_clamp, input.screenUV - 2.0 * delta);
	float4 col1b = SAMPLE_TEXTURE2D(_PostFXSource, sampler_linear_clamp, input.screenUV + 2.0 * delta);
	float4 col2a = SAMPLE_TEXTURE2D(_PostFXSource, sampler_linear_clamp, input.screenUV - 3.0 * delta);
	float4 col2b = SAMPLE_TEXTURE2D(_PostFXSource, sampler_linear_clamp, input.screenUV + 3.0 * delta);

	float w = 0.37004405286;
	float w0a = CompareColor(col, col0a) * 0.31718061674;
	float w0b = CompareColor(col, col0b) * 0.31718061674;
	float w1a = CompareColor(col, col1a) * 0.19823788546;
	float w1b = CompareColor(col, col1b) * 0.19823788546;
	float w2a = CompareColor(col, col2a) * 0.11453744493;
	float w2b = CompareColor(col, col2b) * 0.11453744493;

	float3 result;
	result = w * col.rgb;
	result += w0a * col0a.rgb;
	result += w0b * col0b.rgb;
	result += w1a * col1a.rgb;
	result += w1b * col1b.rgb;
	result += w2a * col2a.rgb;
	result += w2b * col2b.rgb;

	result /= w + w0a + w0b + w1a + w1b + w2a + w2b;

	return float4(result, 1);
}

float4 BlendBulkLightFragment (Varyings input) : SV_TARGET{
	float4 originCol = SAMPLE_TEXTURE2D(_PostFXSource, sampler_linear_clamp, input.screenUV);
	float4 bulkCol = SAMPLE_TEXTURE2D(_PostFXSource2, sampler_linear_clamp, input.screenUV);
	return float4(originCol.xyz + bulkCol.xyz, 1);
}

float4 BloomAddPassFragment (Varyings input) : SV_TARGET {
	float3 lowRes;
	if (_BloomBicubicUpsampling) {
		lowRes = GetSourceBicubic(input.screenUV).rgb;
	}
	else {
		lowRes = GetSource(input.screenUV).rgb;
	}
	float4 highRes = GetSource2(input.screenUV);
	return float4(lowRes * _BloomIntensity + highRes.rgb, highRes.a);
}

float4 BloomHorizontalPassFragment (Varyings input) : SV_TARGET {
	float3 color = 0.0;
	float offsets[] = {
		-4.0, -3.0, -2.0, -1.0, 0.0, 1.0, 2.0, 3.0, 4.0
	};
	float weights[] = {
		0.01621622, 0.05405405, 0.12162162, 0.19459459, 0.22702703,
		0.19459459, 0.12162162, 0.05405405, 0.01621622
	};
	for (int i = 0; i < 9; i++) {
		float offset = offsets[i] * 2.0 * GetSourceTexelSize().x;
		color += GetSource(input.screenUV + float2(offset, 0.0)).rgb * weights[i];
	}
	return float4(color, 1.0);
}

float4 BloomVerticalPassFragment(Varyings input) : SV_TARGET{
	float3 color = 0.0;
	float offsets[] = {
		-3.23076923, -1.38461538, 0.0, 1.38461538, 3.23076923
	};
	float weights[] = {
		0.07027027, 0.31621622, 0.22702703, 0.31621622, 0.07027027
	};
	for (int i = 0; i < 5; i++) {
		float offset = offsets[i] * GetSourceTexelSize().y;
		color += GetSource(input.screenUV + float2(0.0, offset)).rgb * weights[i];
	}
	return float4(color, 1.0);
}


float3 ApplyBloomThreshold (float3 color) {
	float brightness = Max3(color.r, color.g, color.b);
	float soft = brightness + _BloomThreshold.y;
	soft = clamp(soft, 0.0, _BloomThreshold.z);
	soft = soft * soft * _BloomThreshold.w;
	float contribution = max(soft, brightness - _BloomThreshold.x);
	contribution /= max(brightness, 0.00001);
	return color * contribution;
}

float4 BloomPrefilterPassFragment (Varyings input) : SV_TARGET {
	float3 color = ApplyBloomThreshold(GetSource(input.screenUV).rgb);
	return float4(color, 1.0);
}

float4 BloomPrefilterFirefliesPassFragment (Varyings input) : SV_TARGET {
	float3 color = 0.0;
	float weightSum = 0.0;
	float2 offsets[] = {
		float2(0.0, 0.0),
		float2(-1.0, -1.0), float2(-1.0, 1.0), float2(1.0, -1.0), float2(1.0, 1.0)
	};
	for (int i = 0; i < 5; i++) {
		float3 c =
			GetSource(input.screenUV + offsets[i] * GetSourceTexelSize().xy * 2.0).rgb;
		c = ApplyBloomThreshold(c);
		float w = 1.0 / (Luminance(c) + 1.0);
		color += c * w;
		weightSum += w;
	}
	color /= weightSum;
	return float4(color, 1.0);
}

float4 BloomScatterPassFragment (Varyings input) : SV_TARGET {
	float3 lowRes;
	if (_BloomBicubicUpsampling) {
		lowRes = GetSourceBicubic(input.screenUV).rgb;
	}
	else {
		lowRes = GetSource(input.screenUV).rgb;
	}
	float3 highRes = GetSource2(input.screenUV).rgb;
	return float4(lerp(highRes, lowRes, _BloomIntensity), 1.0);
}

float4 BloomScatterFinalPassFragment (Varyings input) : SV_TARGET {
	float3 lowRes;
	if (_BloomBicubicUpsampling) {
		lowRes = GetSourceBicubic(input.screenUV).rgb;
	}
	else {
		lowRes = GetSource(input.screenUV).rgb;
	}
	float4 highRes = GetSource2(input.screenUV);
	lowRes += highRes.rgb - ApplyBloomThreshold(highRes.rgb);
	return float4(lerp(highRes.rgb, lowRes, _BloomIntensity), highRes.a);
}



float4 CopyPassFragment (Varyings input) : SV_TARGET {
	return GetSource(input.screenUV);
}

float4 _ColorAdjustments;
float4 _ColorFilter;
float4 _WhiteBalance;
float4 _SplitToningShadows, _SplitToningHighlights;

float Luminance (float3 color, bool useACES) {
	return useACES ? AcesLuminance(color) : Luminance(color);
}

float3 ColorGradePostExposure (float3 color) {
	return color * _ColorAdjustments.x;
}

float3 ColorGradeWhiteBalance (float3 color) {
	color = LinearToLMS(color);
	color *= _WhiteBalance.rgb;
	return LMSToLinear(color);
}

float3 ColorGradingContrast (float3 color, bool useACES) {
	color = useACES ? ACES_to_ACEScc(unity_to_ACES(color)) : LinearToLogC(color);
	color = (color - ACEScc_MIDGRAY) * _ColorAdjustments.y + ACEScc_MIDGRAY;
	return useACES ? ACES_to_ACEScg(ACEScc_to_ACES(color)) : LogCToLinear(color);
}

float3 ColorGradeColorFilter (float3 color) {
	return color * _ColorFilter.rgb;
}

float3 ColorGradingHueShift (float3 color) {
	color = RgbToHsv(color);
	float hue = color.x + _ColorAdjustments.z;
	color.x = RotateHue(hue, 0.0, 1.0);
	return HsvToRgb(color);
}

float3 ColorGradingSaturation (float3 color, bool useACES) {
	float luminance = Luminance(color, useACES);
	return (color - luminance) * _ColorAdjustments.w + luminance;
}

float3 ColorGradeSplitToning (float3 color, bool useACES) {
	color = PositivePow(color, 1.0 / 2.2);
	float t = saturate(Luminance(saturate(color), useACES) + _SplitToningShadows.w);
	float3 shadows = lerp(0.5, _SplitToningShadows.rgb, 1.0 - t);
	float3 highlights = lerp(0.5, _SplitToningHighlights.rgb, t);
	color = SoftLight(color, shadows);
	color = SoftLight(color, highlights);
	return PositivePow(color, 2.2);
}



float3 ColorGrade (float3 color, bool useACES = false) {
	color = ColorGradePostExposure(color);
	color = ColorGradeWhiteBalance(color);
	color = ColorGradingContrast(color, useACES);
	color = ColorGradeColorFilter(color);
	color = max(color, 0.0);
	color =	ColorGradeSplitToning(color, useACES);
	color = max(color, 0.0);
	color = ColorGradingHueShift(color);
	color = ColorGradingSaturation(color, useACES);
	return max(useACES ? ACEScg_to_ACES(color) : color, 0.0);
}

float4 _ColorGradingLUTParameters;

bool _ColorGradingLUTInLogC;

float3 GetColorGradedLUT (float2 uv, bool useACES = false) {
	float3 color = GetLutStripValue(uv, _ColorGradingLUTParameters);
	return ColorGrade(_ColorGradingLUTInLogC ? LogCToLinear(color) : color, useACES);
}

float4 ColorGradingNonePassFragment (Varyings input) : SV_TARGET {
	float3 color = GetColorGradedLUT(input.screenUV);
	return float4(color, 1.0);
}

float4 ColorGradingACESPassFragment (Varyings input) : SV_TARGET {
	float3 color = GetColorGradedLUT(input.screenUV, true);
	color = AcesTonemap(color);
	return float4(color, 1.0);
}

float4 ColorGradingNeutralPassFragment (Varyings input) : SV_TARGET {
	float3 color = GetColorGradedLUT(input.screenUV);
	color = NeutralTonemap(color);
	return float4(color, 1.0);
}

float4 ColorGradingReinhardPassFragment (Varyings input) : SV_TARGET {
	float3 color = GetColorGradedLUT(input.screenUV);
	color /= color + 1.0;
	return float4(color, 1.0);
}

TEXTURE2D(_ColorGradingLUT);


float3 ApplyColorGradingLUT (float3 color) {
	return ApplyLut2D(
		TEXTURE2D_ARGS(_ColorGradingLUT, sampler_linear_clamp),
		saturate(_ColorGradingLUTInLogC ? LinearToLogC(color) : color),
		_ColorGradingLUTParameters.xyz
	);
}

//最终影响后处理结果进行渲染到主摄像机的函数
float4 FinalPassFragment (Varyings input) : SV_TARGET {
	float4 color = GetSource(input.screenUV);
	float4 color2 = GetSource2(input.screenUV);
	color.rgb += color2.rgb;
	color.rgb = ApplyColorGradingLUT(color.rgb);
	return float4(color.rgb, 1);
}


float4 FogPassFragment (Varyings input) : SV_TARGET {
	float bufferDepth = SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_point_clamp, input.screenUV, 0);
	float lineardDepth = Linear01Depth(bufferDepth, _ZBufferParams);
	bufferDepth = IsOrthographicCamera() ? OrthographicDepthBufferToLinear(bufferDepth)
		: LinearEyeDepth(bufferDepth, _ZBufferParams);
	float3 worldPos =  _WorldSpaceCameraPos + bufferDepth * input.interpolatedRay.xyz;
	//确定在范围中的比例
    float depthX = 1 - saturate( (_FogMaxDepth - lineardDepth) / ( _FogMaxDepth - _FogMinDepth ) );
	//根据平方缩减
    float depthRadio = pow(depthX, _FogDepthFallOff);

	//确定高度比例
    float posX = saturate( (_FogMaxHight - worldPos.y) / ( _FogMaxHight - _FogMinHight ) );
		
	//根据平方缩减
    float posYRadio = pow(posX, _FogPosYFallOff);

	//越趋近0，越接近本来颜色
	float finalRatio = posYRadio * depthRadio;
	float4 bufferColor = SAMPLE_TEXTURE2D(_PostFXSource, sampler_linear_clamp, input.screenUV);

	float time = _DirectionalLightDirectionsAndMasks[0].y;
	time = saturate( time * 0.5 + 0.5 );

	float3 fogColor = LoadColor(time);

	bufferColor.rgb = lerp(bufferColor.rgb, fogColor, finalRatio);
    return bufferColor;
}


float4 CaculateGray(Varyings input) : SV_TARGET{
	float4 bufferColor = SAMPLE_TEXTURE2D(_PostFXSource, sampler_linear_clamp, input.screenUV);
	bufferColor.a = LinearRgbToLuminance(bufferColor.rgb);
	return bufferColor;
}

float4 FXAAFragment(Varyings input) : SV_TARGET{
	return ApplyFXAA(input.screenUV);
}



#endif
