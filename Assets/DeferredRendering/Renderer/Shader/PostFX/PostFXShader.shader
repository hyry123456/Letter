Shader "Unlit/PostFXShader"
{
    SubShader
    {
        Cull Off
		ZTest Always
		ZWrite Off
		
		HLSLINCLUDE
            #include "../../ShaderLibrary/Common.hlsl"
            #include "HLSL/PostFXPass.hlsl"
		ENDHLSL

		Pass {	//0
			Name "Bloom Add"
			
			HLSLPROGRAM
				#pragma target 3.5
				#pragma vertex BlitPassSimpleVertex
				#pragma fragment BloomAddPassFragment
			ENDHLSL
		}
		
		Pass {	//1
			Name "Bloom Horizontal"
			
			HLSLPROGRAM
				#pragma target 3.5
				#pragma vertex BlitPassSimpleVertex
				#pragma fragment BloomHorizontalPassFragment
			ENDHLSL
		}

		Pass {	//2
			Name "Bloom Prefilter"
			
			HLSLPROGRAM
				#pragma target 3.5
				#pragma vertex BlitPassSimpleVertex
				#pragma fragment BloomPrefilterPassFragment
			ENDHLSL
		}
		
		Pass {	//3
			Name "Bloom Prefilter Fireflies"
			
			HLSLPROGRAM
				#pragma target 3.5
				#pragma vertex BlitPassSimpleVertex
				#pragma fragment BloomPrefilterFirefliesPassFragment
			ENDHLSL
		}
		
		Pass {	//4
			Name "Bloom Scatter"
			
			HLSLPROGRAM
				#pragma target 3.5
				#pragma vertex BlitPassSimpleVertex
				#pragma fragment BloomScatterPassFragment
			ENDHLSL
		}
		
		Pass {	//5
			Name "Bloom Scatter Final"
			
			HLSLPROGRAM
				#pragma target 3.5
				#pragma vertex BlitPassSimpleVertex
				#pragma fragment BloomScatterFinalPassFragment
			ENDHLSL
		}
		
		Pass {	//6
			Name "Bloom Vertical"
			
			HLSLPROGRAM
				#pragma target 3.5
				#pragma vertex BlitPassSimpleVertex
				#pragma fragment BloomVerticalPassFragment
			ENDHLSL
		}
		
		Pass {	//7
			Name "Copy"
			
			HLSLPROGRAM
				#pragma target 3.5
				#pragma vertex BlitPassSimpleVertex
				#pragma fragment CopyPassFragment
			ENDHLSL
		}
		
		Pass {	//8
			Name "Color Grading None"
			
			HLSLPROGRAM
				#pragma target 3.5
				#pragma vertex BlitPassSimpleVertex
				#pragma fragment ColorGradingNonePassFragment
			ENDHLSL
		}

		Pass {	//9
			Name "Color Grading ACES"
			
			HLSLPROGRAM
				#pragma target 3.5
				#pragma vertex BlitPassSimpleVertex
				#pragma fragment ColorGradingACESPassFragment
			ENDHLSL
		}

		Pass {	//10
			Name "Color Grading Neutral"
			
			HLSLPROGRAM
				#pragma target 3.5
				#pragma vertex BlitPassSimpleVertex
				#pragma fragment ColorGradingNeutralPassFragment
			ENDHLSL
		}
		
		Pass {	//11
			Name "Color Grading Reinhard"
			
			HLSLPROGRAM
				#pragma target 3.5
				#pragma vertex BlitPassSimpleVertex
				#pragma fragment ColorGradingReinhardPassFragment
			ENDHLSL
		}

		Pass {	//12
			Name "Final"
			
			HLSLPROGRAM
				#pragma target 3.5
				#pragma vertex BlitPassSimpleVertex
				#pragma fragment FinalPassFragment
			ENDHLSL
		}

		Pass{	//13
            Name "SSR"

            HLSLPROGRAM
                #pragma target 3.5
				#pragma vertex SSRPassVertex
				#pragma fragment SSS_Fragment
            ENDHLSL
        }

		Pass	//14
        {
			Name "GBuffer Draw"

            HLSLPROGRAM
            #pragma vertex BlitPassRayVertex
            #pragma fragment DrawGBufferColorFragment
			#pragma multi_compile _ _USE_CLUSTER


            ENDHLSL
        }

		Pass	//15
        {
			Name "BulkLight"

            HLSLPROGRAM
            #pragma vertex BlitPassRayVertex
            #pragma fragment BulkLightFragment
			#pragma multi_compile _ _USE_CLUSTER

            ENDHLSL
        }

		Pass	//16
        {
			Name "BilateralFilter"

            HLSLPROGRAM
            #pragma vertex BlitPassSimpleVertex
            #pragma fragment BilateralFilterFragment

            ENDHLSL
        }

		Pass	//17
        {
			Name "Blend Bulk"

            HLSLPROGRAM
            #pragma vertex BlitPassSimpleVertex
            #pragma fragment BlendBulkLightFragment

            ENDHLSL
        }

		Pass	//18
		{
			Name "Fog"

            HLSLPROGRAM
            #pragma vertex BlitPassRayVertex
            #pragma fragment FogPassFragment

            ENDHLSL
		}
    }
}
