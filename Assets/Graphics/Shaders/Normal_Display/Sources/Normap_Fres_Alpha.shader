// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Normals Display/Normals_Fade_Bump_Fresnel"
 {
	Properties
	{
		_alpha ("Alpha", Range(0,1)) = 1
		_NormalMap("Bump", 2D) = "white" {}
		_AlphaMap("Fade Map",2D) = "white"{}
		_fresnel("Fresnel Strength",Range(0,4)) = 2
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"RenderType"="NORMALS_FADE_BUMP_FRESNEL"
			"IgnoreProjector"="True"
		}

		Blend SrcAlpha OneMinusSrcAlpha

	 	Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct VertexInput
			{
				float4 vertex: POSITION;
				float4 normal:NORMAL;
				float4 uv : TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				float2 uv 			: TEXCOORD0;
				float2 uv2 			: TEXCOORD1;
				float3 camNormal	:TEXCOORD2;
				float fresnel		:TEXCOORD3;
			};

			float _alpha;
			sampler2D _NormalMap;
			sampler2D _AlphaMap;
			float4 _NormalMap_ST;
			float4 _AlphaMap_ST;
			float _fresnel;

			VertexOutput vert (VertexInput v)
			{
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv,_NormalMap);
				o.uv2 = TRANSFORM_TEX(v.uv,_AlphaMap);
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
				//normals display
				half3 bump = UnpackNormal(tex2D(_NormalMap,i.uv));
				float alpha =tex2D(_AlphaMap,i.uv2).b * _alpha;

				return fixed4(((i.camNormal+bump) * i.fresnel)/2 + 0.5, alpha);
			}
			ENDCG
		}
	}
}