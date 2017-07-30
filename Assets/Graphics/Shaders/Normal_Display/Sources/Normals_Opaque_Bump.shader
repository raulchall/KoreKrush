// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Normals Display/Normals_Opaque_Bump"
{
	Properties
	{
		_NormalMap("Bump", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"RenderType"="NORMALS_OPAQUE_BUMP"
		}

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
				float3 camNormal	:NORMAL;
			};

			sampler2D _NormalMap;
			float4 _NormalMap_ST;

			VertexOutput vert (VertexInput v)
			{
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv,_NormalMap);
				o.camNormal = mul(UNITY_MATRIX_IT_MV, v.normal);

				return o;
			}

			fixed4 frag(VertexOutput i) : SV_Target
			{
				//normals display
				half3 bump = UnpackNormal(tex2D(_NormalMap,i.uv));

				return fixed4((i.camNormal+bump)/2 + 0.5f,1);
			}
			ENDCG
		}
	}
}