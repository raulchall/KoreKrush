﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ScreenSpaceDistortion"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DisplaceTex ("Texture", 2D) = "white" {}
		_Magnitude("Magnitude", Range(0,0.1)) = 1
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
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _DisplaceTex;
			float4 _DisplaceTex_ST;
			float _Magnitude;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv2 = TRANSFORM_TEX(v.uv, _DisplaceTex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 disp = tex2D(_DisplaceTex, i.uv2).xy;
				disp = ((disp.xy * 2) - 1) * _Magnitude;
				fixed4 col = tex2D(_MainTex, i.uv + disp);

				return col;
			}
			ENDCG
		}
	}
}
