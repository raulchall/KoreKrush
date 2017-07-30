Shader "Custom/Neon Glow/Color"
{
	Properties
	{
		_GlowColor ("Color", Color) = (1,1,1,1)
		_GlowStr("Glow Strength", Range(0,1)) = 0.75
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			float4 _GlowColor;
			float _GlowStr;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return _GlowColor*_GlowStr;
			}
			ENDCG
		}
	}
}
