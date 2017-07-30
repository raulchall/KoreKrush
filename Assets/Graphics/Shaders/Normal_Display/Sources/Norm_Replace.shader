// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Normals Display/Normals Display Replace"
{
	Properties
	{
		_alpha ("Alpha", Range(0,1)) = 1
		_NormalMap("Normals", 2D) = "white" {}
		_AlphaMap("Alpha",2D) = "white"{}
		_fresnel("FresnelStrength",Range(0,4)) = 2
	}
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
	SubShader
	{
		Tags
		{
			"RenderType"="NORMALS_OPAQUE_BUMP_FRESNEL"
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
				float3 camNormal	:TEXCOORD1;
				float fresnel : TEXCOORD2;
			};

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
				half3 bump = UnpackNormal(tex2D(_NormalMap,i.uv));
				return fixed4(((i.camNormal+bump) * i.fresnel*0.5f) + 0.5f, 1);
			}
			ENDCG
		}
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
				half3 normalWorld = UnityObjectToWorldNormal(v.normal);

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
	SubShader
	{
		Tags
		{	
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="NORMALS_TRANSPARENT"
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
				float3 normal		:NORMAL;
			};

			struct VertexOutput
			{
				float4 pos			: SV_POSITION;
				float3 camNormal	: TEXCOORD0;
			};

			float _alpha;

			VertexOutput vert (VertexInput v)
			{
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.camNormal = mul(UNITY_MATRIX_IT_MV, v.normal);
				return o;
			}

			fixed4 frag(VertexOutput i) : SV_Target
			{
				return fixed4((i.camNormal*0.5f + 0.5f),_alpha);
			}
			ENDCG
		}
	}
	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="NORMALS_TRANSPARENT_BUMP"
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
				float3 normal		: NORMAL;
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
				float alpha =tex2D(_AlphaMap,i.uv).b * _alpha;
				return fixed4(i.camNormal*0.5f + 0.5f,alpha);
			}
			ENDCG
		}
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
	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="NORMALS_TRANSPARENT_FRESNEL"
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
				float3 normal		:NORMAL;
			};

			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				float3 camNormal	:TEXCOORD0;
				float fresnel : TEXCOORD1;
			};

			float _alpha;
			float _fresnel;

			VertexOutput vert (VertexInput v)
			{
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.camNormal = mul(UNITY_MATRIX_IT_MV, v.normal);

				//fresnel
				half3 posWorld = mul(unity_ObjectToWorld, v.vertex).xyz;
				half3 normalWorld = mul(unity_ObjectToWorld, v.normal);


				float3 V = normalize(_WorldSpaceCameraPos.xyz - posWorld);
				float3 N = normalize(normalWorld);
				o.fresnel = pow(1-dot(N, V), _fresnel);

				return o;
			}

			fixed4 frag(VertexOutput i) : SV_Target
			{
				float fresnel = i.fresnel;
				return fixed4(i.camNormal*i.fresnel/2 + 0.5f,_alpha);
			}
			ENDCG
		}
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