// Invisible floor that only shows the shadows cast on it, so cannons and blocks keep their shadows over the illustrated backdrop.
Shader "JellyPop/ShadowCatcher"
{
    Properties { _ShadowColor ("Shadow Color", Color) = (0.02, 0.0, 0.14, 0.5)
        _Gain ("Strength Gain", Float) = 10 }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        CGPROGRAM
        #pragma surface surf ShadowOnly alpha:fade noambient
        fixed4 _ShadowColor;
        float _Gain;
        struct Input { float dummy; };
        fixed4 LightingShadowOnly(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            fixed4 c;
            c.rgb = _ShadowColor.rgb;
            // Scene lights use a very low shadow strength (0.1), so amplify the attenuation back to a full-strength mask.
            c.a = saturate((1 - atten) * _Gain) * _ShadowColor.a;
            return c;
        }
        void surf(Input IN, inout SurfaceOutput o) { o.Albedo = 0; o.Alpha = 1; }
        ENDCG
    }
    Fallback Off
}
