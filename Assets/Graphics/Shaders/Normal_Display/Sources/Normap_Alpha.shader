// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Normals Display/Normals_Fade_Bump"
 {
	Properties
	{
		_alpha ("Alpha", Range(0,1)) = 1
		_NormalMap("Bump", 2D) = "white" {}
		_AlphaMap("Fade Map",2D) = "white"{}
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"RenderType"="NORMALS_FADE_BUMP"
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
			};

			float _alpha;
			sampler2D _NormalMap;
			sampler2D _AlphaMap;
			float4 _NormalMap_ST;
			float4 _AlphaMap_ST;

			VertexOutput vert (VertexInput v)
			{
				VertexOutput o;
				o.uv = TRANSFORM_TEX(v.uv,_NormalMap);
				o.uv2 = TRANSFORM_TEX(v.uv,_AlphaMap);
				o.camNormal = mul(UNITY_MATRIX_IT_MV, v.normal);
				o.pos = UnityObjectToClipPos(v.vertex);

				return o;
			}

			fixed4 frag(VertexOutput i) : SV_Target
			{
				//normals display
				half3 bump = UnpackNormal(tex2D(_NormalMap,i.uv));
				float alpha =tex2D(_AlphaMap,i.uv2).b * _alpha;

				return fixed4((i.camNormal+bump)*0.5,alpha);
			}
			ENDCG
		}
	}
}