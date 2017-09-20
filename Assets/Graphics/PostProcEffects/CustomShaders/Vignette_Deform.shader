Shader "Hidden/Return_Pixel_Stuff"
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

			fixed4 frag (v2f i) : SV_Target
			{
				//from corner to center UV
				float2 v = i.uv*2 - 1;
				//the vignete mask
				float mask = dot(v,v);
				float inverseMask = 1-mask;
				//cubic screen distortion
				float2 dist_base = v*inverseMask;

				float4 col;
				col.rg = (dist_base*0.5f+1)/2.0f;
				col.b = inverseMask;
				col.a = mask;
				return col;
			}
			ENDCG
		}
	}
}
