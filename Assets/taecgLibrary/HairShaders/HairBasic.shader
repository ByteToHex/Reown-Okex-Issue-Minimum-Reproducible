//by taecg@qq.com
//created 2018/01/20	
//updated 2024/04/09
//受光照影响的头发,各向异性效果
Shader "taecg/Hair/Hair Basic"
{
	Properties
	{
		[Header(Base)]
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 0
		_Color ("Color",Color) = (1,1,1,1)
		_Emiss("Emission", Range(0, 3)) = 1
		_MainTex("MainTex(RGB),Cutoff(A)", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0, 1)) = 0.35

		[Header(Normal)]
		_NormalTex("NormalTex", 2D) = "bump" {}
		_NormalIntensity("NormalIntensity", Range(0 , 2)) = 0

		[Header(Specular)]
		_AnisoDir("SpecShift(R)", 2D) = "white" {}
		_SpecularIntenstiy("Specular Intensity", Range(0, 5)) = 1.0
		_PrimarySpecularColor("Primary Specular Color", Color) = (1,1,1,1)
		_PrimarySpecularMultiplier("Primary Specular Multiplier", float) = 100.0
		_PrimaryShift("Primary Specular Shift", float) = 0.0
		_SecondarySpecularColor("Secondary Specular Color", Color) = (0.5,0.5,0.5,1)
		_SecondarySpecularMultiplier("Secondary Specular Multiplier", float) = 100.0
		_SecondaryShift("Secondary Specular Shift", float) = 0.7
	}

	SubShader
	{
		Tags { "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline"}
		Cull [_Cull]

		Pass
		{
			Tags { "LightMode" = "SRPDefaultUnlit" }

			HLSLPROGRAM
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.5

			#pragma multi_compile_fog
			#pragma multi_compile _ _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

			CBUFFER_START(UnityPerMaterial)
				float4 _MainTex_ST,_AnisoDir_ST,_NormalTex_ST;
				half4 _Color;
				half4 _PrimarySpecularColor,_SecondarySpecularColor;
				half _Cutoff;
				half _Emiss;
				half _NormalIntensity;
				half _PrimarySpecularMultiplier, _PrimaryShift,_SpecularIntenstiy,_SecondaryShift,_SecondarySpecularMultiplier;
			CBUFFER_END
			TEXTURE2D(_MainTex);		SAMPLER(sampler_MainTex);
			TEXTURE2D(_AnisoDir);		SAMPLER(sampler_AnisoDir);
			TEXTURE2D(_NormalTex);		SAMPLER(sampler_NormalTex);

			struct Attributes 
			{
				float4 positionOS 	: POSITION;
				float2 texcoord 	: TEXCOORD0;
				half4 tangentOS 	: TANGENT;
				half3 normalOS 		: NORMAL;
			};

			struct Varyings 
			{
				float4 positionCS	: SV_POSITION;
				half4 uv 			: TEXCOORD0;
				float3 positionWS	: TEXCOORD1;
				half3 normalWS 		: TEXCOORD2;
				half3 tangentWS 	: TEXCOORD3;
				half3 bitangentWS 	: TEXCOORD4;
				half3 vertexSH 		: TEXCOORD5;
				float4 shadowCoord	: TEXCOORD6;
				half fog	 		: TEXCOORD7;
			};

			Varyings vert(Attributes v)
			{
				Varyings o = (Varyings)0;

				o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
				o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
				o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.texcoord.xy, _NormalTex);

				o.normalWS = TransformObjectToWorldNormal(v.normalOS.xyz);
				o.tangentWS = TransformObjectToWorldDir(v.tangentOS.xyz);
				half sign = v.tangentOS.w * GetOddNegativeScale();
				o.bitangentWS.xyz = cross(o.normalWS, o.tangentWS) * sign;

				o.shadowCoord = TransformWorldToShadowCoord(o.positionWS);
				o.vertexSH = SampleSHVertex(o.normalWS);
				o.fog = ComputeFogFactor(o.positionCS.z);

				return o;
			}

			half4 frag(Varyings i) : SV_Target 
			{
				half4 c = 1;

				half4 baseMap = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv.xy);
				clip(baseMap.a - _Cutoff);

				half3 normalMap = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalTex,sampler_NormalTex,i.uv.xy),_NormalIntensity);
				half3 normalWS = mul(normalMap,half3x3(i.tangentWS.xyz, i.bitangentWS.xyz, i.normalWS.xyz));

				Light mainLight = GetMainLight(i.shadowCoord);

				half3 attenuatedLightColor = mainLight.color * (mainLight.distanceAttenuation * mainLight.shadowAttenuation);
				half3 diffuseColor = i.vertexSH + LightingLambert(attenuatedLightColor, mainLight.direction, normalWS);
				#ifdef _ADDITIONAL_LIGHTS
					uint pixelLightCount = GetAdditionalLightsCount();
					for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
					{
						Light light = GetAdditionalLight(lightIndex, i.positionWS);
						half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
						diffuseColor += LightingLambert(attenuatedLightColor, light.direction, normalWS);
						// specularColor += LightingSpecular(attenuatedLightColor, light.direction, normalWS, inputData.viewDirectionWS, specularGloss, smoothness);
					}
				#endif

				c.rgb = diffuseColor * baseMap.rgb * _Color.rgb * _Emiss;

				c.rgb = MixFog(c.rgb, i.fog);
				return c;
			}
			ENDHLSL
		}

		Pass
		{
			Tags { "LightMode" = "UniversalForward" }
			ZWrite Off
			Cull[_Cull]
			Blend SrcAlpha OneMinusSrcAlpha

			HLSLPROGRAM
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.5

			#pragma multi_compile_fog
			#pragma multi_compile _ _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

			CBUFFER_START(UnityPerMaterial)
				float4 _MainTex_ST,_AnisoDir_ST,_NormalTex_ST;
				half4 _Color;
				half4 _PrimarySpecularColor,_SecondarySpecularColor;
				half _Cutoff;
				half _Emiss;
				half _NormalIntensity;
				half _PrimarySpecularMultiplier, _PrimaryShift,_SpecularIntenstiy,_SecondaryShift,_SecondarySpecularMultiplier;
			CBUFFER_END
			TEXTURE2D(_MainTex);		SAMPLER(sampler_MainTex);
			TEXTURE2D(_AnisoDir);		SAMPLER(sampler_AnisoDir);
			TEXTURE2D(_NormalTex);		SAMPLER(sampler_NormalTex);

			struct Attributes 
			{
				float4 positionOS 	: POSITION;
				float2 texcoord 	: TEXCOORD0;
				half4 tangentOS 	: TANGENT;
				half3 normalOS 		: NORMAL;
			};

			struct Varyings 
			{
				float4 positionCS	: SV_POSITION;
				half4 uv 			: TEXCOORD0;
				float3 positionWS	: TEXCOORD1;
				half3 normalWS 		: TEXCOORD2;
				half3 tangentWS 	: TEXCOORD3;
				half3 bitangentWS 	: TEXCOORD4;
				half3 vertexSH 		: TEXCOORD5;
				float4 shadowCoord	: TEXCOORD6;
				half fog	 		: TEXCOORD7;
			};

			Varyings vert(Attributes v)
			{
				Varyings o = (Varyings)0;

				o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
				o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
				o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.texcoord.xy, _NormalTex);

				o.normalWS = TransformObjectToWorldNormal(v.normalOS.xyz);
				o.tangentWS = TransformObjectToWorldDir(v.tangentOS.xyz);
				half sign = v.tangentOS.w * GetOddNegativeScale();
				o.bitangentWS.xyz = cross(o.normalWS, o.tangentWS) * sign;

				o.shadowCoord = TransformWorldToShadowCoord(o.positionWS);
				o.vertexSH = SampleSHVertex(o.normalWS);
				o.fog = ComputeFogFactor(o.positionCS.z);

				return o;
			}

			//发丝高光
			half StrandSpecular(half3 T, half3 V, half3 L, half exponent)
			{
				half3 H = normalize(L + V);
				half dotTH = dot(T, H);
				half sinTH = sqrt(1 - dotTH * dotTH);
				half dirAtten = smoothstep(-1, 0, dotTH);
				//return dirAtten * pow(sinTH, exponent);
				return pow(sinTH, exponent);
			}

			//切线扰动
			half3 TangentShift(half3 T, half3 N, half shift)
			{
				return normalize(T + shift * N);
			}

			half4 frag(Varyings i) : SV_Target 
			{
				half4 c = 1;

				half4 baseMap = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv.xy);
				// clip(baseMap.a - _Cutoff);

				half3 normalMap = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalTex,sampler_NormalTex,i.uv.xy),_NormalIntensity);
				half3 normalWS = mul(normalMap,half3x3(i.tangentWS.xyz, i.bitangentWS.xyz, i.normalWS.xyz));

				Light mainLight = GetMainLight(i.shadowCoord);

				half3 attenuatedLightColor = mainLight.color * (mainLight.distanceAttenuation * mainLight.shadowAttenuation);
				half3 diffuseColor = i.vertexSH + LightingLambert(attenuatedLightColor, mainLight.direction, normalWS);
				#ifdef _ADDITIONAL_LIGHTS
					uint pixelLightCount = GetAdditionalLightsCount();
					for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
					{
						Light light = GetAdditionalLight(lightIndex, i.positionWS);
						half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
						diffuseColor += LightingLambert(attenuatedLightColor, light.direction, normalWS);
						// specularColor += LightingSpecular(attenuatedLightColor, light.direction, normalWS, inputData.viewDirectionWS, specularGloss, smoothness);
					}
				#endif
				c.rgb = diffuseColor * baseMap.rgb * _Color.rgb * _Emiss;

				//获取视角方向、灯光方向在世界空间下的坐标
				half3 viewWS = normalize(_WorldSpaceCameraPos - i.positionWS.xyz);

				//采样高光纹理
				half3 spec = SAMPLE_TEXTURE2D(_AnisoDir,sampler_AnisoDir, i.uv.xy).rgb;
				half shiftTex = spec.r;

				//切线扰动
				half3 t1 = TangentShift(i.bitangentWS, normalWS, _PrimaryShift + shiftTex);
				half3 t2 = TangentShift(i.bitangentWS, normalWS, _SecondaryShift + shiftTex);

				//高光计算
				half3 spec1 = StrandSpecular(t1, viewWS, mainLight.direction, _PrimarySpecularMultiplier) * _PrimarySpecularColor.rgb;
				half3 spec2 = StrandSpecular(t2, viewWS, mainLight.direction, _SecondarySpecularMultiplier) * _SecondarySpecularColor.rgb;

				//最终混合
				c.rgb += spec1 * _SpecularIntenstiy;
				c.rgb += spec2 * _SpecularIntenstiy;

				c.rgb = MixFog(c.rgb, i.fog);
				c.a = baseMap.a;

				return c;
			}
			ENDHLSL
		}

		Pass
		{
			Name "ShadowCaster"
			Tags{"LightMode" = "ShadowCaster"}

			ZWrite On
			ZTest LEqual
			ColorMask 0

			HLSLPROGRAM
			#pragma target 2.0
			#pragma vertex ShadowPassVertex
			#pragma fragment ShadowPassFragment

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

			CBUFFER_START(UnityPerMaterial)
				float4 _MainTex_ST,_AnisoDir_ST,_NormalTex_ST;
				half4 _Color;
				half4 _PrimarySpecularColor,_SecondarySpecularColor;
				half _Cutoff;
				half _Emiss;
				half _NormalIntensity;
				half _PrimarySpecularMultiplier, _PrimaryShift,_SpecularIntenstiy,_SecondaryShift,_SecondarySpecularMultiplier;
			CBUFFER_END
			TEXTURE2D(_MainTex);		SAMPLER(sampler_MainTex);
			TEXTURE2D(_AnisoDir);		SAMPLER(sampler_AnisoDir);
			TEXTURE2D(_NormalTex);		SAMPLER(sampler_NormalTex);
			float3 _LightDirection;

			struct Attributes
			{
				float4 positionOS   : POSITION;
				float3 normalOS     : NORMAL;
				float2 texcoord     : TEXCOORD0;
			};

			struct Varyings
			{
				float2 uv           : TEXCOORD0;
				float4 positionCS   : SV_POSITION;
			};

			Varyings ShadowPassVertex(Attributes v)
			{
				Varyings o = (Varyings)0;

				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				float3 positionWS = TransformObjectToWorld(v.positionOS.xyz);
				float3 normalWS = TransformObjectToWorldNormal(v.normalOS.xyz);
				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
				#if UNITY_REVERSED_Z
					positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
				#else
					positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
				#endif
				o.positionCS = positionCS;
				return o;
			}

			half4 ShadowPassFragment(Varyings i) : SV_TARGET
			{
				half4 baseMap = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv.xy);
				clip(baseMap.a - _Cutoff);
				return 0;
			}
			ENDHLSL
		}
	}

	SubShader
	{
		Tags
		{ 
			"Queue" = "Transparent" 
			"RenderType" = "TransparentCutout"  
		}
		Cull [_Cull]

		//不透明Pass,目的在于渲染基本纹理及基本光照，同时做AlphaTest
		Pass
		{
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma skip_variants DIRLIGHTMAP_COMBINED DYNAMICLIGHTMAP_ON LIGHTMAP_ON LIGHTMAP_SHADOW_MIXING SHADOWS_SCREEN SHADOWS_SHADOWMASK  
			//#define UNITY_PASS_FORWARDBASE
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			sampler2D _NormalTex;float4 _NormalTex_ST;
			half _NormalIntensity;
			sampler2D _MainTex;float4 _MainTex_ST;
			half _Cutoff;
			fixed4 _Color;
			half _Emiss;

			struct appdata 
			{
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f 
			{
				UNITY_POSITION(pos);
				half4 uv : TEXCOORD0;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;

				#if UNITY_SHOULD_SAMPLE_SH
					half3 sh : TEXCOORD4;
				#endif

				UNITY_SHADOW_COORDS(5)
				UNITY_FOG_COORDS(6)
			};

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f,o);

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.texcoord.zw, _NormalTex);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
				o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

				#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
					o.sh = 0;
					// Approximated illumination from non-important point lights
					#ifdef VERTEXLIGHT_ON
						o.sh += Shade4PointLights(
						unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
						unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
						unity_4LightAtten0, worldPos, worldNormal);
					#endif
					o.sh = ShadeSHPerVertex(worldNormal, o.sh);
				#endif

				UNITY_TRANSFER_SHADOW(o,v.texcoord1.xy); // pass shadow coordinates to pixel shader
				UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
				return o;
			}

			fixed4 frag(v2f i) : SV_Target 
			{
				float3 worldPos = float3(i.tSpace0.w, i.tSpace1.w, i.tSpace2.w);
				#ifndef USING_DIRECTIONAL_LIGHT
					half3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				#else
					half3 lightDir = _WorldSpaceLightPos0.xyz;
				#endif

				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT(SurfaceOutput, o);
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Specular = 0.0;
				o.Alpha = 0.0;
				o.Gloss = 0.0;
				o.Normal = half3(0,0,1);

				o.Normal = UnpackScaleNormal(tex2D(_NormalTex, i.uv.zw), _NormalIntensity);
				fixed4 mainTex = tex2D(_MainTex,i.uv.xy);
				o.Albedo = mainTex.rgb * _Color * _Emiss;
				o.Alpha = 1;
				clip(mainTex.a - _Cutoff);

				UNITY_LIGHT_ATTENUATION(atten, i, worldPos)

				fixed4 c = 0;
				half3 worldN;
				worldN.x = dot(i.tSpace0.xyz, o.Normal);
				worldN.y = dot(i.tSpace1.xyz, o.Normal);
				worldN.z = dot(i.tSpace2.xyz, o.Normal);
				worldN = normalize(worldN);
				o.Normal = worldN;

				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
				gi.indirect.diffuse = 0;
				gi.indirect.specular = 0;
				gi.light.color = _LightColor0.rgb;
				gi.light.dir = lightDir;

				UnityGIInput giInput;
				UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
				giInput.light = gi.light;
				giInput.worldPos = worldPos;
				giInput.atten = atten;
				#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
					giInput.ambient = i.sh;
				#else
					giInput.ambient.rgb = 0.0;
				#endif
				LightingLambert_GI(o, giInput, gi);

				c += LightingLambert(o, gi);

				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}
			ENDCG
		}

		//头发主要效果Pass，包括各向异性高光的实现
		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			ZWrite Off
			Cull[_Cull]
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma skip_variants DIRLIGHTMAP_COMBINED DYNAMICLIGHTMAP_ON LIGHTMAP_ON LIGHTMAP_SHADOW_MIXING SHADOWS_SCREEN SHADOWS_SHADOWMASK  
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			sampler2D _MainTex, _AnisoDir,_NormalTex;
			float4 _MainTex_ST, _AnisoDir_ST,_NormalTex_ST;
			fixed4 _Color;
			half _Emiss;
			half _PrimarySpecularMultiplier, _PrimaryShift,_SpecularIntenstiy,_SecondaryShift,_SecondarySpecularMultiplier;
			half4 _PrimarySpecularColor,_SecondarySpecularColor;
			half _Cutoff;
			half _NormalIntensity;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				UNITY_POSITION(pos);
				half4 uv : TEXCOORD0;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;

				#if UNITY_SHOULD_SAMPLE_SH
					half3 sh : TEXCOORD4;
				#endif

				UNITY_SHADOW_COORDS(5)
				UNITY_FOG_COORDS(6)
			};

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f,o);

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.texcoord.zw, _NormalTex);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
				o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

				#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
					o.sh = 0;
					// Approximated illumination from non-important point lights
					#ifdef VERTEXLIGHT_ON
						o.sh += Shade4PointLights(
						unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
						unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
						unity_4LightAtten0, worldPos, worldNormal);
					#endif
					o.sh = ShadeSHPerVertex(worldNormal, o.sh);
				#endif

				UNITY_TRANSFER_SHADOW(o,v.texcoord1.xy); // pass shadow coordinates to pixel shader
				UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
				return o;
			}

			//发丝高光
			half StrandSpecular(half3 T, half3 V, half3 L, half exponent)
			{
				half3 H = normalize(L + V);
				half dotTH = dot(T, H);
				half sinTH = sqrt(1 - dotTH * dotTH);
				half dirAtten = smoothstep(-1, 0, dotTH);
				//return dirAtten * pow(sinTH, exponent);
				return pow(sinTH, exponent);
			}

			//切线扰动
			half3 TangentShift(half3 T, half3 N, half shift)
			{
				return normalize(T + shift * N);
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 worldPos = float3(i.tSpace0.w, i.tSpace1.w, i.tSpace2.w);
				#ifndef USING_DIRECTIONAL_LIGHT
					half3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				#else
					half3 lightDir = _WorldSpaceLightPos0.xyz;
				#endif

				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT(SurfaceOutput, o);
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Specular = 0.0;
				o.Alpha = 0.0;
				o.Gloss = 0.0;
				o.Normal = half3(0,0,1);

				o.Normal = UnpackScaleNormal(tex2D(_NormalTex, i.uv.zw), _NormalIntensity);
				//基础纹理
				fixed4 mainTex = tex2D(_MainTex, i.uv.xy);
				o.Albedo = mainTex.rgb;
				o.Alpha = 1;

				// compute lighting & shadowing factor
				UNITY_LIGHT_ATTENUATION(atten, i, worldPos)

				half3 worldN;
				worldN.x = dot(i.tSpace0.xyz, o.Normal);
				worldN.y = dot(i.tSpace1.xyz, o.Normal);
				worldN.z = dot(i.tSpace2.xyz, o.Normal);
				worldN = normalize(worldN);
				o.Normal = worldN;

				// Setup lighting environment
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
				gi.indirect.diffuse = 0;
				gi.indirect.specular = 0;
				gi.light.color = _LightColor0.rgb;
				gi.light.dir = lightDir;

				// Call GI (lightmaps/SH/reflections) lighting function
				UnityGIInput giInput;
				UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
				giInput.light = gi.light;
				giInput.worldPos = worldPos;
				giInput.atten = atten;
				//#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
				//	giInput.lightmapUV = i.lmap;
				//#else
				//	giInput.lightmapUV = 0.0;
				//#endif
				#if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
					giInput.ambient = i.sh;
				#else
					giInput.ambient.rgb = 0.0;
				#endif
				LightingLambert_GI(o, giInput, gi);

				fixed4 diffuse = LightingLambert(o, gi);
				diffuse *= _Color * _Emiss;
				diffuse.a = mainTex.a;

				//采样法线纹理
				half3 normalTex = UnpackScaleNormal(tex2D(_NormalTex, i.uv.zw),_NormalIntensity);
				//最终法线的计算，将TBN与法线纹理点乘
				half3 worldNormal = normalize(float3(dot(i.tSpace0.xyz, normalTex), dot(i.tSpace1.xyz, normalTex), dot(i.tSpace2.xyz, normalTex)));

				//重新通过TBN获取世界位置、切线、副切线
				half3 worldTangent = normalize(float3(i.tSpace0.x, i.tSpace1.x, i.tSpace2.x));
				half3 worldBinormal = normalize(float3(i.tSpace0.y, i.tSpace1.y, i.tSpace2.y));

				//获取视角方向、灯光方向在世界空间下的坐标
				half3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				half3 worldLightDir = normalize(UnityWorldSpaceLightDir(worldPos));

				//采样高光纹理
				half3 spec = tex2D(_AnisoDir, i.uv).rgb;

				half shiftTex = spec.r;
				//half shiftTex = 0.5;
				//切线扰动
				half3 t1 = TangentShift(worldBinormal, worldNormal, _PrimaryShift + shiftTex);
				half3 t2 = TangentShift(worldBinormal, worldNormal, _SecondaryShift + shiftTex);

				//高光计算
				half3 spec1 = StrandSpecular(t1, worldViewDir, worldLightDir, _PrimarySpecularMultiplier) * _PrimarySpecularColor;
				half3 spec2 = StrandSpecular(t2, worldViewDir, worldLightDir, _SecondarySpecularMultiplier) * _SecondarySpecularColor;
				//return fixed4(diffuseColor,1);

				//最终混合
				fixed4 finalColor = 0;
				finalColor += diffuse;
				finalColor.rgb += spec1 * _SpecularIntenstiy;
				finalColor.rgb += spec2 * _SpecularIntenstiy;
				finalColor.rgb *= _LightColor0.rgb;

				UNITY_APPLY_FOG(i.fogCoord, finalColor);
				return finalColor;
			}
			ENDCG
		}

		//ForwardAdd Pass
		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardAdd" }
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_fog
			#pragma multi_compile_fwdadd_fullshadows novertexlight noshadowmask nodynlightmap nodirlightmap nolightmap
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			sampler2D _NormalTex; float4 _NormalTex_ST;
			half _NormalIntensity;
			sampler2D _MainTex; float4 _MainTex_ST;
			half _Cutoff;
			fixed4 _Color;
			half _Emiss;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				UNITY_POSITION(pos);
				half4 uv : TEXCOORD0;
				half3 tSpace0 : TEXCOORD1;
				half3 tSpace1 : TEXCOORD2;
				half3 tSpace2 : TEXCOORD3;
				float3 worldPos : TEXCOORD4;
				UNITY_SHADOW_COORDS(5)
				UNITY_FOG_COORDS(6)
			};

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f,o);

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.texcoord.zw, _NormalTex);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
				o.tSpace0 = half3(worldTangent.x, worldBinormal.x, worldNormal.x);
				o.tSpace1 = half3(worldTangent.y, worldBinormal.y, worldNormal.y);
				o.tSpace2 = half3(worldTangent.z, worldBinormal.z, worldNormal.z);
				o.worldPos = worldPos;

				UNITY_TRANSFER_SHADOW(o,v.texcoord1.xy);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 worldPos = i.worldPos;
				#ifndef USING_DIRECTIONAL_LIGHT
					half3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				#else
					half3 lightDir = _WorldSpaceLightPos0.xyz;
				#endif
				#ifdef UNITY_COMPILER_HLSL
					SurfaceOutput o = (SurfaceOutput)0;
				#else
					SurfaceOutput o;
				#endif
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Specular = 0.0;
				o.Alpha = 0.0;
				o.Gloss = 0.0;
				half3 normalWorldVertex = half3(0,0,1);
				o.Normal = half3(0,0,1);

				o.Normal = UnpackScaleNormal(tex2D(_NormalTex, i.uv.zw), _NormalIntensity);
				fixed4 mainTex = tex2D(_MainTex, i.uv.xy);
				o.Albedo = mainTex.rgb * _Color * _Emiss;
				o.Alpha = 1;
				clip(mainTex.a - _Cutoff);

				UNITY_LIGHT_ATTENUATION(atten, i, worldPos)
				fixed4 c = 0;
				half3 worldN;
				worldN.x = dot(i.tSpace0.xyz, o.Normal);
				worldN.y = dot(i.tSpace1.xyz, o.Normal);
				worldN.z = dot(i.tSpace2.xyz, o.Normal);
				worldN = normalize(worldN);
				o.Normal = worldN;

				// Setup lighting environment
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
				gi.indirect.diffuse = 0;
				gi.indirect.specular = 0;
				gi.light.color = _LightColor0.rgb;
				gi.light.dir = lightDir;
				gi.light.color *= atten;
				c += LightingLambert(o, gi);
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}
			ENDCG
		}

		//阴影Pass
		Pass
		{
			Name "Caster"
			Tags { "LightMode" = "ShadowCaster" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"

			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2  uv : TEXCOORD1;
			};

			sampler2D _MainTex; float4 _MainTex_ST;
			half _Cutoff;

			v2f vert(appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				fixed4 mainTex = tex2D(_MainTex, i.uv);
				clip(mainTex.a - _Cutoff);

				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}

	FallBack "Legacy Shaders/Transparent/Cutout/Soft Edge Unlit"
}