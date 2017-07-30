Shader "Custom/Basic Postprocesing Effects/Blur/Box"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
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
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _BlurStr;

			float4 boxBlur(sampler2D tex, float2 uv,half strength)
			{
				return ( tex2D(tex, uv)
					+ tex2D(tex, uv + float2(-0.005,0)*strength)
					+ tex2D(tex, uv + float2(0.005,0)*strength)
				 	+ tex2D(tex, uv + float2(0,-0.005)*strength)
				 	+ tex2D(tex, uv + float2(0,0.005)*strength)
				 	+ tex2D(tex,uv + float2(0.005,0.005)*strength)
				 	+ tex2D(tex,uv + float2(-0.005,-0.005)*strength)
				 	+ tex2D(tex,uv + float2(0.005,-0.005)*strength)
				 	+ tex2D(tex,uv + float2(-0.005,0.005)*strength)
				  	)/9;
			}


			fixed4 frag (v2f i) : SV_Target
			{
				half2 v = i.uv*2 - 1;
				//return (1,1,1,1);
				float2 uv = i.uv;
				return boxBlur(_MainTex, uv, 1);
			}
			ENDCG
		}
	}
}
