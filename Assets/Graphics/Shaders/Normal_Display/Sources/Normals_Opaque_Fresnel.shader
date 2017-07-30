Shader "Custom/Normals Display/Normals_Opaque_Fresnel"
 {
	Properties
	{
		_fresnel("FresnelStrength",Range(0,4)) = 2
	}

	SubShader
	{
		Tags
		{
			"RenderType"="NORMALS_OPAQUE_FRESNEL"
		}
	 	Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct VertexInput
			{
				float4 vertex		: POSITION;
				float3 normal		:NORMAL;
			};

			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				float3 camNormal	:TEXCOORD0;
				float fresnel : TEXCOORD1;
			};

			float _fresnel;

			VertexOutput vert (VertexInput v)
			{
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.camNormal = mul(UNITY_MATRIX_IT_MV, v.normal);

				//fresnel
				half3 posWorld = mul(unity_ObjectToWorld, v.vertex).xyz;
				half3 normalWorld = mul(unity_ObjectToWorld, v.normal).xyz;

				float3 V = normalize(_WorldSpaceCameraPos.xyz - posWorld);
				float3 N = normalize(normalWorld);
				o.fresnel = pow(1-dot(N, V), _fresnel);

				return o;
			}

			fixed4 frag(VertexOutput i) : SV_Target
			{
				return fixed4((((i.camNormal) * i.fresnel)/2 + 0.5f),1);
			}
			ENDCG
		}
	}
}