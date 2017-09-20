Shader "Custom/VFX"
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


		[Header(Texture Settings)]

		[Toggle(TEX0)]_UseTex0("Add Texture Slot",Float) = 0
		[ConditionalVar(_UseTex0)]_tex0("Texture 0",2D) = "white" {}

		[Toggle(TEX1)]_UseTex1("Add Texture Slot",Float) = 0
		[ConditionalVar(_UseTex1)]_tex1("Texture 1",2D) = "white" {}

		[Toggle(TEX2)]_UseTex2("Add Texture Slot",Float) = 0
		[ConditionalVar(_UseTex2)]_tex2("Texture 2",2D) = "white" {}

		[Toggle(TEX3)]_UseTex3("Add Texture Slot",Float) = 0
		[ConditionalVar(_UseTex3)]_tex3("Texture 3",2D) = "white" {}


		[Header(Color Settings)]
		//way of display the  color: just a color, display a texture, use a correspondence beteween a texture and a ramp
		[KeywordEnum(Color,Texture,Ramp)]DISPLAYCOLORS("DisplayColors", Float) = 0
		_ColorTint("Color",Color) = (1,1,1,1)
		_ColorSrt("Color Strength",Range(0,1)) = 1//easier animation with an additional slider
		[KeywordEnum(TEX0,TEX1,TEX2,TEX3)]COLORMAP("TakeColorMapFrom",Float) = 0//if using a texture map select sourceTexture
		[ConditionalVar(DISPLAYCOLORS,2)]_Ramp("Ramp Texture",2D) = "white" {}//the correspondence ramp
		//alpha mask
		[KeywordEnum(OFF,RANGE,MASK)]ALPHA("UseColorMask",Float) = 0
		[ConditionalVar(ALPHA)]_alphaRange("AlphaRange",Range(0,1)) = 1
		[EnumKeyword(TEX0,TEX1,TEX2,TEX3)]ALPHAMASKSRC("TakeFromTex0",Float) = 0

		[EnumKeyword(R,G,B,A)]ALPHACHANEL("UseChanelForAlpha",Float) = 0
		[ConditionalToggle(ALPHA,2)]ALPHAFLOWMAP("Use FlowMap",Float) = 0
		[EnumKeyword(TEX0,TEX1,TEX2,TEX3)]FLOWMAPSRC("TakeFromTex0, Allways use RG chanels",Float) = 0



		[Header(Main Settings)]
		_MainTex("Main Texture", 2D) = "white" {}
		[Toggle(MAINANIM)] _mainAnim("Animate Texture Offset", Float) = 0
		[ConditionalVar(_mainAnim)]_texAnimVector("Texture Offset Animation Vector",Vector) = (0,0,0,0)
		[Toggle(MAINTEXFLOWMAP)] _mainUseFlowMap("Use Flow Map",Float) = 0
		[ConditionalVar(_mainUseFlowMap)]_mainTexFlowMap("FlowMap",2D) = "white" {}

		[Header(TexturesBuffer)]
		[Toggle(TEX0)]_useTex0("Use Texture Slot 0",Float) = 0

		[Header(Alpha Settings)]

		[KeywordEnum(OFF, VALUE, MAP)] ALPHA("Use Alpha",Float) = 0
		[ConditionalVar(ALPHA)]_alphaRange("Alpha Value", Range(0,1)) = 0.5
		[ConditionalVar(ALPHA,2)]_alphaMap("Alpha Map", 2D) = "white" {}
		[ConditionalToggle(ALPHA,2,ALPHAANIM)]_alphaAnim("Animate Alpha Texture Offset", Float) = 0
		[ConditionalVar(_alphaAnim)]_alphaAnimVector("Alpha Offset Animation Vector",Vector) = (0,0,0,0)
		[ConditionalToggle(ALPHA,2,ALPHAFLOWMAP)]_alphaUseFlowMap("UseFlowMap", Float) = 0

		[Space(15)]

		[Header(Bump Settings)]

		[Toggle(FLOW)]_useFlow("Use Flow Map",Float) = 0
		[ConditionalVar(_useFlow)]_flowMap ("Flow Map", 2D) = "white" {}
		[ConditionalToggle(_useBump,BUMPANIM)]_bumpAnim("Animate Bump Texture Offset", Float) = 0
		[ConditionalVar(_bumpAnim)]_bumpAnimVector("Bump Offset Animation Vector",Vector) = (0,0,0,0)
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
		Tags{"RenderType" = "VFX"}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#pragma shader_feature ALPHA_OFF ALPHA_VALUE ALPHA_MAP
			#pragma shader_feature ALPHAANIM

			#pragma shader_feature ALPHA
			#pragma shader_feature FLOWMAP

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
					half3x3 tangToViewMatrix: TEXCOORD1;
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
				float3 viewNormal = mul(UNITY_MATRIX_MV, v.normal);

				#if BUMP
					//bump texture settings
					float2 bumpUV = TRANSFORM_TEX(v.uv,_bumpMap);

					#if BUMPANIM
						bumpUV += _bumpAnimVector*_Time.y;
					#endif

					o.bumpUV = bumpUV;

					float3 viewTangent = mul(UNITY_MATRIX_MV,v.tangent);

					//TODO: Encontrar una forma correcta de calcular los parametros de proyeccion y aplicarselos al signo de la tangente 
					half tangentSign = v.tangent.w*_ProjectionParams.w;

					half3 viewBitangent = cross(viewNormal, viewTangent);// * tangentSign;
					//output matrix
					o.tangToViewMatrix[0] = half3(viewTangent.x,viewBitangent.x,viewNormal.x);
					o.tangToViewMatrix[1] = half3(viewTangent.y,viewBitangent.y,viewNormal.y);
					o.tangToViewMatrix[2] = half3(viewTangent.z,viewBitangent.z,viewNormal.z);
				#else
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
					viewBumpedNormal.x = dot(i.tangToViewMatrix[0],bumpSample);
					viewBumpedNormal.y = dot(i.tangToViewMatrix[1],bumpSample);
					viewBumpedNormal.z = dot(i.tangToViewMatrix[2],bumpSample);
					col = half4(viewBumpedNormal.xyz,1);
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

				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}