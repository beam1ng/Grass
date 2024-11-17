Shader "PrettyGrass/TerrainGrid"
{
    Properties
    {
        _VerticalOffset ("Vertical Offset", Float) = 0.25
        _GridWidth ("Grid Width", Float) = 0.1
        _GridFadeDistance ("Grid Fade Distance", Float) = 100
        _GridFadeLength ("Grid Fade Length", Float) = 100
        _GridColor_Edge ("Grid Edge Color", Color) = (1,1,1)
        _GridColor_Fill ("Grid Edge Fill", Color) = (0.2,0.2,0.2)
    }
    
    SubShader
    {
        Tags { "RenderQueue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            //includes
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 relativePositionFlatWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };
            
            uniform float _VerticalOffset;
            uniform float _GridWidth;
            uniform float _GridFadeDistance;
            uniform float _GridFadeLength;
            uniform float3 _GridColor_Edge;
            uniform float3 _GridColor_Fill;

            v2f vert (appdata v)
            {
                v2f o;
                const float4 offsetPositionCS = float4(v.vertex.xyz + float3(0,_VerticalOffset,0),1);
                o.vertex = mul(UNITY_MATRIX_MVP,offsetPositionCS);
                o.relativePositionFlatWS = TransformObjectToWorld(v.vertex).xz - TransformObjectToWorld(float3(0,0,0)).xz;
                o.positionWS = TransformObjectToWorld(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float isEdge = any(step(1.0 - _GridWidth, (i.relativePositionFlatWS.xy - 0.5 * _GridWidth.xx) % 1.0));
                float cameraDistance = distance(GetCameraPositionWS(),i.positionWS);
                float fadeFactor = smoothstep(_GridFadeDistance,_GridFadeDistance - _GridFadeLength, cameraDistance);
                float3 gridColor = lerp(_GridColor_Fill,_GridColor_Edge,isEdge * fadeFactor);
                float alpha = 0.3f;
                return float4(gridColor ,alpha);
            }
            ENDHLSL
        }
    }
}
