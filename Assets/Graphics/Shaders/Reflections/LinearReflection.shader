Shader "Unlit/LinearReflection"
{
	Properties
	{
		_Ramp ("Ramp", 2D) = "white" {}
		_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
		[PowerSlider(3.0)]_FresnelStr("Fresnel Strength",Range(0.01,1)) = 1
		//_FresnelStr("Fresnel Strength",Range(0.01,1)) = 2
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
				float3 normal: NORMAL;
				float2 uv: TEXCOORD0;
			};

			struct v2f
			{
				float2 uv: TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float fresnel:TEXCOORD2;
			};

			sampler2D _Ramp;
			float4 _Ramp_ST;
			float _FresnelStr;
			float4 _FresnelColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				/// compute world space position of the vertex
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                // compute world space view direction
                float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                // world space normal
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                // world space reflection vector
                o.uv = (0,reflect(-worldViewDir, worldNormal).y/2+0.5f);
                o.uv = TRANSFORM_TEX(o.uv,_Ramp);

                //fresnel
				float3 V = normalize(_WorldSpaceCameraPos.xyz - worldPos);
				o.fresnel = 1-dot(worldNormal, V)*_FresnelStr;

                return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_Ramp, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				//return col;//*(1-i.fresnel) + i.fresnel*_FresnelColor;
				return col*i.fresnel;
				//return col;
			}
			ENDCG
		}
	}
}
