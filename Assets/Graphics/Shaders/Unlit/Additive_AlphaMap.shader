Shader "Custom/Unlit/Fade/Additive_AlphaMap"
{
	Properties
	{
		_GlowColor ("Color", Color) = (1,1,1,1)
		_GlowStr("Glow Strength", Range(0,1)) = 0.75
		_AlphaMap("AlphaMap", 2D) = "white"{}
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
		Cull Off
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
			};

			float4 _GlowColor;
			float _GlowStr;
			sampler2D _AlphaMap;
			float4 _AlphaMap_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _AlphaMap);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float alpha = tex2D(_AlphaMap, i.uv);
				return _GlowColor*_GlowStr*alpha;
			}
			ENDCG
		}
	}
}
