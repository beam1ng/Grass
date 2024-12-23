Shader "Custom/InstancedIndirectLitPacked"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        _Specular ("Specular", Range(0.0, 1.0)) = 0
        _Ambient ("Ambient", Range(0.0, 1.0)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "TransparentCutout"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Blend One Zero
        Cull Off
        ZTest LEqual
        ZWrite On
        ZClip True

        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                uint instanceID : SV_InstanceID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            struct GrassBufferInstanceData
            {
                float3 positionWs;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float _Cutoff;
            float _Specular;
            float _Ambient;

            StructuredBuffer<GrassBufferInstanceData> _PositionBuffer;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 positionOffsetWS = _PositionBuffer[IN.instanceID].positionWs;
                const float rotationAngleOY = dot(normalize(frac(positionOffsetWS)),normalize(float3(0.147,0.609,0.699))) * 16.1803f;
                const float3 posOS = mul(RotationFromAxisAngle(float3(0,1,0),sin(rotationAngleOY),cos(rotationAngleOY)),IN.positionOS.xyz);
                OUT.positionWS = posOS + positionOffsetWS;
                OUT.positionCS = mul(UNITY_MATRIX_VP, float4(OUT.positionWS.xyz, 1));
                OUT.normalWS = float3(0, 0, -1);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN)

                float4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                float4 finalColor = texColor;

                clip(finalColor.a - _Cutoff);

                float3 normalWS = normalize(IN.normalWS);
                Light mainLight = GetMainLight();
                float3 lightDir = mainLight.direction;
                float3 lightColor = mainLight.color;
                float NdotL = max(dot(normalWS, lightDir), 0.0);
                float3 diffuse = lightColor * NdotL;

                float3 specularColor = float3(1, 1, 1);
                float3 cameraDir = normalize(_WorldSpaceCameraPos - IN.positionWS);
                float3 halfVector = normalize(lightDir + cameraDir);
                float specularIntensity = _Specular * max(dot(halfVector, normalWS), 0.0);
                float3 specular = specularColor * specularIntensity;

                float3 ambientLightColor = float3(1, 1, 1);
                float3 ambient = ambientLightColor * _Ambient;

                finalColor.rgb *= (diffuse + specular + ambient);

                return finalColor;
            }
            ENDHLSL
        }
    }
}