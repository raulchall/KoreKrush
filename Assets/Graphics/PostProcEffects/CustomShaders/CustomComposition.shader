Shader "Custom/CustomComposition"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Magnitude("Magnitude", Range(0,0.1)) = 1
		_ScreenDirt("Dirt", 2D) = "white" {}
		_OverlayFlares("Flares",Range(0,1)) = 0.25
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
			sampler2D _ScreenDirt;
			sampler2D _NormalsDisplay;
			sampler2D _Half;
			float4 _NormalsDisplay_ST;
			float4 _MainTex_TexelSize;
			float _Magnitude;
			sampler2D _Quart;
			float _OverlayFlares;
			float4 _ScreenDirt_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv2 = TRANSFORM_TEX(v.uv.yx, _ScreenDirt);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				//from corner to center UV
				half2 v = i.uv*2 - 1;
				//the vignete mask
				half mask = dot(v,v);
				half inverseMask = 1-mask;
				//cubic screen distortion
				half2 dist_base = (v*inverseMask*0.05);
				//find distortion mesh shape, also deformed for the screen cubic distortion
				half2 disp = tex2D(_NormalsDisplay, i.uv + dist_base);
				//center the distortion;
				disp = (disp*2-1)*0.1 + dist_base + i.uv;

				fixed4 col = tex2D(_MainTex, disp);
				fixed4 blur = tex2D(_Half, disp);
				fixed4 col3 = tex2D(_ScreenDirt, i.uv2);
				half col0H = col;
				half col1H = blur;
				//return mask;
				return ((col + blur*col1H)*inverseMask + (blur+col)*col1H*mask + col3*_OverlayFlares) - mask* 0.04f;
			}
			ENDCG
		}
	}
}
