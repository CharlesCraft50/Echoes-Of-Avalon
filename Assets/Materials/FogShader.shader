Shader "Custom/FogShader"
{
    Properties
    {
        _MainTexture ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _FogColor ("Fog Color", Color) = (0,0,0,1)
        _FogStart ("Fog Start", Float) = 10
        _FogEnd ("Fog End", Float) = 20
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            float4 _Color;
            float4 _FogColor;
            float _FogStart;
            float _FogEnd;
            
            sampler2D _MainTexture;
            float4 _MainTexture_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.pos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTexture);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float fogFactor = saturate((i.screenPos.z - _FogStart) / (_FogEnd - _FogStart));
                half4 col = tex2D(_MainTexture, i.uv) * _Color;
                return lerp(col, _FogColor, fogFactor);
            }
            ENDCG
        }
    }
}
