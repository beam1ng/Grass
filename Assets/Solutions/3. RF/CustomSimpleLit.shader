Shader "CustomSimpleLit"
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

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)
            UNITY_INSTANCING_BUFFER_END(Props)

            float _Specular;
            float _Ambient;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = positionInputs.positionCS;
                OUT.uv = IN.uv;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionWS = positionInputs.positionWS;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                float cutoff = UNITY_ACCESS_INSTANCED_PROP(Props, _Cutoff);

                float4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                float4 finalColor = texColor;

                clip(finalColor.a - cutoff);

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