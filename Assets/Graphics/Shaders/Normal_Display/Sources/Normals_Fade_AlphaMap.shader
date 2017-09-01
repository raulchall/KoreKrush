Shader "Custom/Normals Display/Normals_Fade"
 {
	Properties
	{
		_alpha ("Alpha", Range(0,1)) = 1
		_AlphaMap("AlphaMap",2D) = "white"{}
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="NORMALS_FADE"
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
				float4 uv			: TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 pos			: SV_POSITION;
				float3 camNormal	: TEXCOORD0;
				float2 uv			: TEXCOORD1;
			};

			float _alpha;
			sampler2D _AlphaMap;
			float4 _AlphaMap_ST;

			VertexOutput vert (VertexInput v)
			{
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.camNormal = mul(UNITY_MATRIX_IT_MV, v.normal);
				o.uv = TRANSFORM_TEX(v.uv,_AlphaMap);
				return o;
			}

			fixed4 frag(VertexOutput i) : SV_Target
			{
				float alpha =tex2D(_AlphaMap,i.uv).a * _alpha;
				return fixed4(i.camNormal*0.5f + 0.5f,alpha);
			}
			ENDCG
		}
	}
}