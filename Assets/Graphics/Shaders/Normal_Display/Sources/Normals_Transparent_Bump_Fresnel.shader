﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Normals Display/Normals_Transparent_Bump_Fresnel"
 {
	Properties
	{
		_alpha ("Alpha", Range(0,1)) = 1
		_NormalMap("Bump", 2D) = "white" {}
		_fresnel("Fresnel Strength",Range(0,4)) = 2
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"RenderType"="NORMALS_TRANSPARENT_BUMP_FRESNEL"
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
				float4 vertex		: POSITION;
				float4 normal		: NORMAL;
				float4 uv 			: TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				float2 uv 			: TEXCOORD0;
				float3 camNormal	:TEXCOORD1;
				float fresnel : TEXCOORD2;
			};

			float _alpha;
			sampler2D _NormalMap;
			float4 _NormalMap_ST;
			float _fresnel;

			VertexOutput vert (VertexInput v)
			{
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv,_NormalMap);
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
				//return fixed4(0,0,1,1);
				return fixed4(((i.camNormal+bump) * i.fresnel)/2 + 0.5f, _alpha);
			}
			ENDCG
		}
	}
}