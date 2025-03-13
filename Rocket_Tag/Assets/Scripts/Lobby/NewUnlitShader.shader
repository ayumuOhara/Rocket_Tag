Shader "UI/SemiTransparentWithRoundedCorners"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha("Alpha", Float) = 0.5
        _Radius("Corner Radius", Range(0, 0.5)) = 0.1  // 角の半径を指定
    }
    SubShader
    {
        Tags {
            "RenderType"="UI"
            "Queue"="Overlay"  // UIはOverlayQueueに配置
        }

        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha

            ZWrite Off
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv           : TEXCOORD0;
                float4 vertex       : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_ST;
            float _Alpha;
            float _Radius; // 角の半径

            CBUFFER_START(UnityPerMaterial)
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;// TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {

                // テクスチャの色を取得
                float4 col = i.uv.y;


                return col;
            }
            ENDHLSL
        }
    }
    Fallback "UI/Default"
}
