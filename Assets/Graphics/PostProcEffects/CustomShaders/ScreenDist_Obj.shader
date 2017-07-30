// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ScreenDistortionObj"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Magnitude("Magnitude", Range(0,0.1)) = 0.05
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				
				float2 uv : TEXCOORD0;
				float2 invuv : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float _Magnitude;
			sampler2D _NormalsDisplay;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				float val = o.uv.y;
				#if !UNITY_UV_STARTS_AT_TOP
					val = 1-val;
				#endif

				o.invuv = float2(v.uv.x, val);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float4 distortion = tex2D(_NormalsDisplay, i.invuv);
				float2 dist2D =(distortion.xy * 2 - 1) * _Magnitude * distortion.a;
				fixed4 col = tex2D(_MainTex, i.uv + dist2D);
				return col;
			}
			ENDCG
		}
	}
}
