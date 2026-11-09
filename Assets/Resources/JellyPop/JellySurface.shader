Shader "JellyPop/Surface"
{
    Properties { _Color ("Puzzle color", Color) = (1,1,1,1) _Spent ("Spent cannon", Range(0,1)) = 0 }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            fixed4 _Color;
            half _Spent;
            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; };
            struct v2f { float4 pos : SV_POSITION; float3 normal : TEXCOORD0; float3 world : TEXCOORD1; };
            v2f vert(appdata v)
            {
                v2f o; o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                float3 n = normalize(i.normal);
                float3 view = normalize(_WorldSpaceCameraPos - i.world);
                float3 light = normalize(float3(-.45, .85, -.4));
                float diffuse = saturate(dot(n, light) * .5 + .5);
                float spec = pow(saturate(dot(n, normalize(light + view))), 56);
                float rim = pow(1 - saturate(dot(n, view)), 3);
                float3 candy = _Color.rgb * lerp(.65, 1.12, diffuse);
                candy += spec * .65 + rim * float3(.16, .12, .22);
                candy = lerp(candy, dot(candy, float3(.3,.59,.11)) * float3(.78,.72,.86), _Spent * .5);
                return fixed4(saturate(candy), 1);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
