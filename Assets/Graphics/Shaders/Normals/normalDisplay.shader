Shader "Custom/Normals"
{
	Properties
	{
		[Header(HardwareSettings)]
		[Enum(UnityEngine.Rendering.CullMode)] HARWARE_CullMode ("Cull Faces",Float) = 2
		[Enum(UnityEngine.Rendering.BlendMode)] HARWARE_BlendSrc ("Blend Source", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] HARWARE_BlendDst ("Blend Destination", Float) = 0
		[Enum(On,1,Off,0)] HARDWARE_Zwrite ("Depth Write", Float) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)] HARDWARE_ZTest ("Depth Test",Float) = 4
		[Space(15)]



		[Header(Bump Settings)]

		[Toggle(BUMP)]_useBump("Use Bump Map",Float) = 0
		[ConditionalVar(_useBump)]_bumpMap ("Bump Map", 2D) = "white" {}
		[ConditionalToggle(_useBump,BUMPANIM)]_bumpAnim("Animate Bump Texture Offset", Float) = 0
		[ConditionalVar(_bumpAnim)]_bumpAnimVector("Bump Offset Animation Vector",Vector) = (0,0,0,0)
		[Space(15)]



		[Header(Alpha Settings)]

		[KeywordEnum(OFF, VALUE, MAP)] ALPHA("Use Alpha",Float) = 0
		[ConditionalVar(ALPHA)]_alphaRange("Alpha Value", Range(0,1)) = 0.5
		[ConditionalVar(ALPHA,2)]_alphaMap("Alpha Map", 2D) = "white" {}
		[ConditionalToggle(ALPHA,2,ALPHAANIM)]_alphaAnim("Animate Alpha Texture Offset", Float) = 0
		[ConditionalVar(_alphaAnim)]_alphaAnimVector("Alpha Offset Animation Vector",Vector) = (0,0,0,0)
		[Space(15)]

		[Header(Fresnel Settings)]

		[KeywordEnum(OFF,POSITIVE,NEGATIVE)] FRESNEL("Fresnel as Alpha",Float) = 0
		_fresnelPow("Presnel Power",Range(1,3)) = 2
	}

	SubShader
	{
		Cull[HARWARE_CullMode]
		ZWrite[HARDWARE_Zwrite]
		ZTest[HARDWARE_ZTest]
		Blend[HARWARE_BlendSrc] [HARWARE_BlendDst]
		Tags{"RenderType" = "NORMALS"}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#pragma shader_feature BUMP
			#pragma shader_feature BUMPANIM

			#pragma shader_feature ALPHA_OFF ALPHA_VALUE ALPHA_MAP
			#pragma shader_feature ALPHAANIM

			#pragma shader_feature FRESNEL_OFF FRESNEL_POSITIVE FRESNEL_NEGATIVE

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				#if(BUMP || ALPHA_MAP)
				float2 uv : TEXCOORD0;
				#endif
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;

				#if BUMP
					float2 bumpUV : TEXCOORD0;
					half3x3 tangToWorldMatrix: TEXCOORD1;
					//half3x3 tangToViewMatrix:TEXCOORD3;
				#else
					float3 normal:NORMAL;
				#endif

				#if(ALPHA_MAP)
					float2 alphaUV : TEXCOORD4;
				#endif

				#if(FRESNEL_POSITIVE || FRESNEL_NEGATIVE)
					float fresnel:TEXCOORD5;
				#endif
			};

			sampler2D _bumpMap;
			float4 _bumpMap_ST;
			float4 _bumpAnimVector;

			sampler2D _alphaMap;
			float4 _alphaMap_ST;
			float _alphaRange;
			float4 _alphaAnimVector;
			float _fresnelPow;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				#if BUMP
					//bump texture settings
					float2 bumpUV = TRANSFORM_TEX(v.uv,_bumpMap);

					#if BUMPANIM
						bumpUV += _bumpAnimVector*_Time.y;
					#endif

					o.bumpUV = bumpUV;

					float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);

					//TODO: Encontrar una forma correcta de calcular los parametros de proyeccion y aplicarselos al signo de la tangente 
					half tangentSign = v.tangent.w*unity_WorldTransformParams.w;

					half3 wBitangent = cross(worldNormal, worldTangent) * tangentSign;
					//output matrix
					o.tangToWorldMatrix[0] = half3(worldTangent.x,wBitangent.x,worldNormal.x);
					o.tangToWorldMatrix[1] = half3(worldTangent.y,wBitangent.y,worldNormal.y);
					o.tangToWorldMatrix[2] = half3(worldTangent.z,wBitangent.z,worldNormal.z);
				#else
					float3 viewNormal = mul(UNITY_MATRIX_MV, v.normal*0.5f);
					o.normal = viewNormal;
				#endif

				#if(ALPHA_MAP)
				float2 alphaUV = TRANSFORM_TEX(v.uv, _alphaMap);
					#if BUMPANIM
					alphaUV += _alphaAnimVector*_Time.y;
					#endif
				o.alphaUV = alphaUV;
				#endif

				#if FRESNEL_POSITIVE || FRESNEL_NEGATIVE
					half3 posWorld = mul(unity_ObjectToWorld,v.vertex).xyz;
					half3 normalWorld = mul(unity_ObjectToWorld,v.normal).xyz;
					half3 V = normalize(WorldSpaceViewDir(v.vertex));
					half3 N = normalize(normalWorld);
					half fresnel = pow(dot(N,V),_fresnelPow);

					#if FRESNEL_POSITIVE
						o.fresnel = fresnel;
					#else
						o.fresnel = 1-fresnel;
					#endif
				#endif

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half4 col;

				#if BUMP
					half3 bumpSample = UnpackNormal(tex2D(_bumpMap,i.bumpUV));
					half3 viewBumpedNormal;
					viewBumpedNormal.x = dot(i.tangToWorldMatrix[0],bumpSample);
					viewBumpedNormal.y = dot(i.tangToWorldMatrix[1],bumpSample);
					viewBumpedNormal.z = dot(i.tangToWorldMatrix[2],bumpSample);
					col = half4(viewBumpedNormal.xyz,1);
					col = fixed4(mul(UNITY_MATRIX_V,viewBumpedNormal).xyz,1);
				#else
					col = fixed4(i.normal,1);
				#endif

				#if ALPHA_MAP
					col.a = tex2D(_alphaMap,i.alphaUV);
				#endif

				#if ALPHA_VALUE || ALPHA_MAP
					col.a *= _alphaRange;
				#endif

				#if FRESNEL_POSITIVE || FRESNEL_NEGATIVE
					col.a *= i.fresnel;
				#endif
				col.xyz+=0.5f;
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}