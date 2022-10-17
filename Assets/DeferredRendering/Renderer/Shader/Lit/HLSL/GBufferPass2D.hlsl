#ifndef DEFFER_GBUFFER_READY_PASS_2D
#define DEFFER_GBUFFER_READY_PASS_2D

#include "../../ShaderLibrary/Surface.hlsl"
#include "../../ShaderLibrary/Shadows.hlsl"
#include "../../ShaderLibrary/Light.hlsl"
#include "../../ShaderLibrary/BRDF.hlsl"
#include "../../ShaderLibrary/GI.hlsl"
#include "../../ShaderLibrary/Lighting.hlsl"

struct Attributes2D
{
    float4 vertex   : POSITION;
    float2 texcoord : TEXCOORD0;
	float4 color : COLOR;
    float4 tangentOS : TANGENT;
	GI_ATTRIBUTE_DATA
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings2D
{
    float4 positionCS_SS   : SV_POSITION;
    float2 baseUV  : TEXCOORD0;
    float3 TtoW0 : NORMAL_TO_WORLD0;
    float3 TtoW1 : NORMAL_TO_WORLD1;
    float3 TtoW2 : NORMAL_TO_WORLD2;
    float3 positionWS : WORLDPOS;
	float4 color : COLOR;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

#define _2D_Normal float3(0, 0, -1)

Varyings2D LitPassVertex (Attributes2D input) {
	Varyings2D output = (Varyings2D)0;
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_TRANSFER_INSTANCE_ID(input, output);
	TRANSFER_GI_DATA(input, output);
	output.positionWS = TransformObjectToWorld(input.vertex.xyz);
	output.positionCS_SS = TransformWorldToHClip(output.positionWS);
	output.baseUV = TransformBaseUV(input.texcoord);
	output.color = input.color;

	#if defined(_NORMAL_MAP)
        float3 worldNormal = _2D_Normal;
        float3 worldTangent = TransformObjectToWorldDir(input.tangentOS.xyz);
        float3 worldBinormal = cross(worldNormal, worldTangent) * input.tangentOS.w;
        //按列排序获得切线空间转世界空间的矩阵,顺便加一个世界坐标位置
        output.TtoW0 = float3(worldTangent.x, worldBinormal.x, worldNormal.x);
        output.TtoW1 = float3(worldTangent.y, worldBinormal.y, worldNormal.y);
        output.TtoW2 = float3(worldTangent.z, worldBinormal.z, worldNormal.z);
	#endif

	#if defined(_DETAIL_MAP)
		output.detailUV = TransformDetailUV(input.baseUV);
	#endif
	return output;
}

void LitPassFragment (Varyings2D input,
        out float4 _GBufferColorTex : SV_Target0,
        out float4 _GBufferNormalTex : SV_Target1,
        out float4 _GBufferSpecularTex : SV_Target2,
        out float4 _GBufferBakeTex : SV_Target3
    ) {
	UNITY_SETUP_INSTANCE_ID(input);
	InputConfig config = GetInputConfig(input.baseUV);
	// ClipLOD(input.positionCS_SS.xy, unity_LODFade.x);
	
	#if defined(_MASK_MAP)
		config.useMask = true;
	#endif
	#if defined(_DETAIL_MAP)
		config.detailUV = input.detailUV;
		config.useDetail = true;
	#endif
	
	//纹理颜色
	float4 base = GetBase(config) * input.color;
	float3 positionWS = input.positionWS;

	#if defined(_CLIPPING)
		clip(base.a - GetCutoff(config));
	#endif
	
	float3 normal = _2D_Normal;    //默认向前
	// float3 perNormal = input.normalWS;
	#if defined(_NORMAL_MAP)
        float3 bump = GetNormalMap(config); //获取法线
        normal = normalize(float3(dot(input.TtoW0, bump), dot(input.TtoW1, bump), dot(input.TtoW2, bump)));
	#else
		normal = _2D_Normal;
	#endif

	float4 specularData = float4(GetMetallic(config), GetSmoothness(config), GetFresnel(config), 1);		//w赋值为1表示开启PBR

	//烘焙灯光，只处理了烘焙贴图，没有处理阴影烘焙，需要注意
	// float3 bakeColor = GetBakeDate(GI_FRAGMENT_DATA(input), positionWS, perNormal);
	float oneMinusReflectivity = OneMinusReflectivity(specularData.r);
	float3 diffuse = base.rgb * oneMinusReflectivity;
	float3 bakeColor = GetEmission(config);				//通过金属度缩减烘焙光，再加上自发光，之后会在着色时直接加到最后的结果上
	// float4 shiftColor = GetShiftColor(config);			//分别使用三张图的透明通道写入

	_GBufferColorTex = float4(base.xyz, 1);
	_GBufferNormalTex = float4(normal * 0.5 + 0.5, 1);
	_GBufferSpecularTex = specularData;

	_GBufferBakeTex = float4(bakeColor, 1);
}

#endif