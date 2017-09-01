Shader "Unlit/SkyReflection"
{
	Properties{
	_Color("Color",Color) = (1,1,1,1)
	_Reflection("Reflections", 2D) = "white"{}
	_Cube("Cube", Cube) = ""
	}
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f {
                half3 worldRefl : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _Reflection;
            fixed4 _Color;
            samplerCUBE _Cube;

			v2f vert (float4 vertex : POSITION, float3 normal : NORMAL)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                // compute world space position of the vertex
                float3 worldPos = mul(unity_ObjectToWorld, vertex).xyz;
                // compute world space view direction
                float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                // world space normal
                float3 worldNormal = UnityObjectToWorldNormal(normal);
                // world space reflection vector
                o.worldRefl = reflect(-worldViewDir, worldNormal);
                float4 uv = (o.worldRefl.xy,o.worldRefl.z,1);
                //o.col = tex2Dlod(_Reflection,uv);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the default reflection cubemap, using the reflection vector
                //half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, i.worldRefl);
                // decode cubemap data into actual color
                //half3 skyColor = DecodeHDR (skyData, unity_SpecCube0_HDR);
                // output it!
                //return fixed4(skyColor*_Color.rgb,1);
                //return i.fresnel;
                return texCUBE(_Cube, i.worldRefl)*_Color;
                //return i.col;
                //return tex2D(_Reflection, i.worldRefl.xy)*_Color;
            }
            ENDCG
        }
    }
}