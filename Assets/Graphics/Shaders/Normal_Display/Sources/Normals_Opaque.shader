Shader "Custom/Normals Display/Normals_Opaque"
 {

	SubShader
	{
		Tags
		{
			"RenderType"="NORMALS_OPAQUE"
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
				float3 normal:NORMAL;
			};

			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				float3 camNormal	:TEXCOORD4;
			};

			VertexOutput vert (VertexInput v)
			{
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.camNormal = mul(UNITY_MATRIX_IT_MV, v.normal);
				return o;
			}

			fixed4 frag(VertexOutput i) : SV_Target
			{
				return fixed4(i.camNormal*0.5f + 0.5f,1);
			}
			ENDCG
		}
	}
}