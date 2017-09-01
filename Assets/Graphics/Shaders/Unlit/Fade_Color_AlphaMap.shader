Shader "Custom/Unlit/Fade/AlphaMap"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
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
		Blend SrcAlpha OneMinusSrcAlpha
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
			};

			float4 _Color;
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
				float alpha = tex2D(_AlphaMap, i.uv).a;
				return _Color*alpha;
			}
			ENDCG
		}
	}
}
