Shader "Custom/Neon Glow/Color_Additive_AlphaMap_Dual"
{
	Properties
	{
		_GlowColor ("Color", Color) = (1,1,1,1)
		_GlowStr("Glow Strength", Range(0,1)) = 0.75
		_AlphaMap_0("AlphaMap_0", 2D) = "white"{}
		_AlphaMap_1("AlphaMap_1", 2D) = "white"{}
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType" = "Transparent"
		}

		Blend One One
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex 	: SV_POSITION;
				float2 uv 		: TEXCOORD0;
				float2 uv2		: TEXCOORD1;
			};

			float4 _GlowColor;
			float _GlowStr;
			sampler2D _AlphaMap_0;
			sampler2D _AlphaMap_1;
			float4 _AlphaMap_1_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv2 = TRANSFORM_TEX(v.uv,_AlphaMap_1);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float alpha_0 = tex2D(_AlphaMap_0, i.uv);
				float alpha_1 = tex2D(_AlphaMap_1, i.uv2);
				return _GlowColor*_GlowStr*alpha_0*alpha_1;
			}
			ENDCG
		}
	}
}
