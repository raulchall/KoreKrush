Shader "Custom/CustomComposition"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _NormalsDisplay;
			sampler2D _Half;
			sampler2D _Vignette;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				half4 vignette = tex2D(_Vignette, i.uv);
				half2 dist_base = (vignette.rg*2-1)*0.075f;
				half mask = vignette.a;
				half inverseMask = vignette.b;

				//find distortion mesh shape, also deformed for the screen cubic distortion
				float2 disp = tex2D(_NormalsDisplay, i.uv + dist_base);
				//center the distortion value in that pixel and add the screen cubic distortion;
				disp = (disp*2-1)*0.075f + dist_base + i.uv;

				fixed4 col = tex2D(_MainTex, disp);
				fixed4 blur = tex2D(_Half,disp);
				half col0H = col;
				half col1H = blur;
				return ((col + blur*col1H)*inverseMask + (blur+col)*col1H*mask)-mask*0.1f;

				//do not erase usarlo para debugear el view normals display
				//return  fixed4(tex2D(_NormalsDisplay, i.uv + dist_base).rg,0,1);
			}
			ENDCG
		}
	}
}
