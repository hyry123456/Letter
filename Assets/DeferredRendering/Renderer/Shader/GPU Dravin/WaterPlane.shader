Shader "Unlit/WaterPlane"
{
    Properties
    {
        _Steepness("Steepness", Range(0, 0.5)) = 0.1
        _WaveLength ("Wave Length", float) = 5
        _Speed("Speed", float) = 1
        _WaveDir0("Direction0", Vector) = (1, -1, 1, 1)
        _WaveDir1("Direction1", Vector) = (1, 2, 3, -1)

	    _WaveScale ("Wave scale", Range (0.02,0.15)) = 0.063
        _RefrDistort ("Refraction distort", Range (0,1.5)) = 0.40
        _RefrColor ("Refraction color", COLOR)  = ( .34, .85, .92, 1)
        _SpecularColor ("Specular Color", COLOR) = (1, 1, 1, 1)
        _NearColor ("Near Side Color", Color) = (1, 1, 1, 1)
        [NoScaleOffset] _BumpMap ("Normalmap ", 2D) = "bump" {}
        WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
        [NoScaleOffset] _ReflectiveColor ("Reflective color (RGB) fresnel (A) ", 2D) = "" {}
        _HorizonColor ("Water color", COLOR)  = ( .172, .463, .435, 1)

        _Gloss("Specular Scale", Float) = 50
        _NearDistance("Near Distance", Range(0.0, 10.0)) = 0.01
        _NearRange ("Near Range", Range(0.01, 10.0)) = 1

		[Toggle(_IS_REFR)] _Is_Refr ("Use Refrect Color", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
    }
    SubShader
    {
        Pass
        {
            ZWrite Off
            Cull Off
            HLSLPROGRAM

            #pragma target 4.6

            #pragma vertex vert
            #pragma fragment frag
            #pragma require geometry
            #pragma geometry geom

            #include "HLSL/WaterPlanePass.hlsl"


            ENDHLSL
        }

        Pass
        {
            Blend [_SrcBlend] [_DstBlend]
			ZWrite Off
            Cull Off
            HLSLPROGRAM

            #pragma target 4.6

            #pragma vertex vert
            #pragma fragment TransferFrag
            #pragma require geometry
            #pragma geometry geom
			#pragma shader_feature _IS_REFR

            #include "HLSL/WaterPlanePass.hlsl"


            ENDHLSL
        }
    }
}
