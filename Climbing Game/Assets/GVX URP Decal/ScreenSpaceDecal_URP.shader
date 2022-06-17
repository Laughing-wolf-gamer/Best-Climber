Shader "GVX/ScreenSpaceDecal_URP"
{

	//Limitation needs a depth camera .... 
	// needs normal to disable rendering on the other side of models 
	// no preview in scene view for normals 
	// if the camera is inside the box it won't render


	Properties
	{
		// Specular vs Metallic workflow

		[Toggle(_NORMALMAP)] _USENORMALMAP("Use Normal Map", Float) = 0
		[HDR] _Color("Color", Color) = (0.5,0.5,0.5,1)
		[MainTexture] _MainTex("Albedo", 2D) = "white" {}
		_ProgressNoise("Progress Noise", 2D) = "white" {}
		_BumpScale("Scale", Float) = 1.0
		_NormalMap("Normal Map", 2D) = "bump" {}
		_Progress("Simulation Factor",Range(0,1)) = 1
	}

		SubShader
		{
			Tags{"RenderType" = "Transparent"  "Queue" = "Geometry+100" "RenderPipeline" = "UniversalRenderPipeline" "IgnoreProjector" = "True"}
			LOD 100

			Pass
			{
				Name "StandardLit"
				Tags{"LightMode" = "UniversalForward"}


				Blend SrcAlpha OneMinusSrcAlpha
				ZWrite Off
				Cull Back

				HLSLPROGRAM
				#pragma prefer_hlslcc gles
				#pragma exclude_renderers d3d11_9x
				#pragma target 2.0


#define _BaseMap _MainTex
#define sampler_BaseMap sampler_MainTex
#define _BaseColor _Color
#define _BumpMap _NormalMap
#define sampler_BumpMap sampler_NormalMap

				// -------------------------------------
				// Material Keywords
				// unused shader_feature variants are stripped from build automatically
				#pragma shader_feature _NORMALMAP
				#pragma shader_feature _RECEIVE_SHADOWS_OFF

				// -------------------------------------
				// Universal Render Pipeline keywords
				#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
				#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
				#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
				#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
				#pragma multi_compile _ _SHADOWS_SOFT
				#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

				// -------------------------------------
				// Unity defined keywords
				#pragma multi_compile _ DIRLIGHTMAP_COMBINED
				#pragma multi_compile _ LIGHTMAP_ON
				#pragma multi_compile_fog

				//--------------------------------------
				// GPU Instancing
				#pragma multi_compile_instancing

				#pragma vertex LitPassVertex
				#pragma fragment LitPassFragment

				
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"


				struct Attributes
				{
					float4 positionOS   : POSITION;
					float3 normalOS     : NORMAL;
					float4 tangentOS    : TANGENT;
					float2 uv           : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct Varyings
				{
					float4 uv                       : TEXCOORD0;
					float4 positionWSAndFogFactor   : TEXCOORD2; // xyz: positionWS, w: vertex fog factor
					half3  normalWS                 : TEXCOORD3;
					float3 worldDirection			: TEXCOORD4;

#if _NORMALMAP
					half3 tangentWS                 : TEXCOORD5;
					half3 bitangentWS               : TEXCOORD6;
#endif
					float4 positionCS               : SV_POSITION;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO //Insert

				};

				Varyings LitPassVertex(Attributes input)
				{
					Varyings output;

					//UNITY_SETUP_INSTANCE_ID(input);
					//UNITY_TRANSFER_INSTANCE_ID(input, output);
					UNITY_SETUP_INSTANCE_ID(input);
					#if defined(UNITY_COMPILER_HLSL)
					#define UNITY_INITIALIZE_OUTPUT(type,name) name = (type)0;
					#else
					#define UNITY_INITIALIZE_OUTPUT(type,name)
					#endif
					UNITY_INITIALIZE_OUTPUT(Varyings, output); //Insert
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output); //Insert
					//UNITY_SETUP_INSTANCE_ID(input); //Insert
					//UNITY_INITIALIZE_OUTPUT(Varyings, output); //Insert
					//UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output); //Insert

					VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
					float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
					output.uv = vertexInput.positionNDC;
					output.positionWSAndFogFactor = float4(vertexInput.positionWS, fogFactor);
					output.normalWS = TransformObjectToWorldDir(float3(0, 1, 0));
#ifdef _NORMALMAP
					output.tangentWS = TransformObjectToWorldDir(float3(1, 0, 0));
					output.bitangentWS = TransformObjectToWorldDir(float3(0, 0, 1));
#endif
					output.positionCS = vertexInput.positionCS;
					output.worldDirection = vertexInput.positionWS.xyz - _WorldSpaceCameraPos;
					return output;
				}

				TEXTURE2D_X(_CameraDepthTexture);
				SAMPLER(sampler_CameraDepthTexture);
				TEXTURE2D(_ProgressNoise);
				SAMPLER(sampler_ProgressNoise);



				UNITY_INSTANCING_BUFFER_START(Props)
					UNITY_DEFINE_INSTANCED_PROP(half, _Progress)
				UNITY_INSTANCING_BUFFER_END(Props)



				float SampleSceneDepth(float4 uv)
				{
					//divide by W to properly interpolate by depth ... 
					return SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(uv.xy / uv.w)).r;
				}

				//UNITY_DECLARE_SCREENSPACE_TEXTURE(_CameraDepthTexture); //Insert

				half4 LitPassFragment(Varyings input) : SV_Target
				{

					UNITY_SETUP_INSTANCE_ID(input);

					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input); //Insert

					float3 positionWS = input.positionWSAndFogFactor.xyz;
					float perspectiveDivide = 1.0f / input.uv.w;
					float3 direction = input.worldDirection * perspectiveDivide;
					float depth = SampleSceneDepth(input.uv);
					float sceneZ = LinearEyeDepth(depth, _ZBufferParams);
					float3 wpos = direction * sceneZ + _WorldSpaceCameraPos;
					float3 opos = TransformWorldToObject(wpos);
					input.uv = float4(opos.xz + 0.5,0,0);
					SurfaceData surfaceData;

					half4 albedoAlpha = SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
					surfaceData.alpha = albedoAlpha.a * _BaseColor.a;
					surfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;
					surfaceData.normalTS = SampleNormal(input.uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), 1);
					surfaceData.occlusion = 1;
					surfaceData.specular = 0.2;
					surfaceData.metallic = 0;
					surfaceData.smoothness = 0.5;
#if _NORMALMAP
					half3 normalWS = TransformTangentToWorld(surfaceData.normalTS,
					half3x3(input.tangentWS, input.bitangentWS, input.normalWS));
#else
					half3 normalWS = input.normalWS;
#endif
					normalWS = normalize(normalWS);
					half3 bakedGI = SampleSH(normalWS);
					BRDFData brdfData;
					InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.alpha, brdfData);


#ifdef _MAIN_LIGHT_SHADOWS
					Light mainLight = GetMainLight(TransformWorldToShadowCoord(wpos));
#else
					Light mainLight = GetMainLight();
	#endif
					half3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - wpos);
					half3 color = GlobalIllumination(brdfData, bakedGI, surfaceData.occlusion, normalWS, viewDirectionWS);

					color += LightingPhysicallyBased(brdfData, mainLight, normalWS, viewDirectionWS);
#ifdef _ADDITIONAL_LIGHTS
					int additionalLightsCount = GetAdditionalLightsCount();
					for (int i = 0; i < additionalLightsCount; ++i)
					{
						Light light = GetAdditionalLight(i, wpos);
						color += LightingPhysicallyBased(brdfData, light, normalWS, viewDirectionWS);
					}
#endif
					float fogFactor = input.positionWSAndFogFactor.w;
					color = MixFog(color, fogFactor);
					float3 absOpos = abs(opos);
					half progress = UNITY_ACCESS_INSTANCED_PROP(Props, _Progress);
					progress = saturate((progress*1.2 - SAMPLE_TEXTURE2D(_ProgressNoise, sampler_ProgressNoise, input.uv).r) / 0.2);
					surfaceData.alpha *=step(max(absOpos.x, max(absOpos.y, absOpos.z)), 0.5)* progress;
					return half4(color, surfaceData.alpha);// surfaceData.alpha);
			}
			ENDHLSL
		}


	}
}
