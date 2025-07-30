Shader "Sky/EmptySky"
{
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" "IgnoreProjector"="True" }
	    Cull Back     // Render side
		Fog{Mode Off} // Don't use fog
     	ZWrite Off    // Don't draw to depth buffer

        Pass
        {
            HLSLPROGRAM
            
            #pragma vertex vertex_program
            #pragma fragment fragment_program
            #pragma target 3.0
            #include "UnityCG.cginc"

            // Constants
            #define PI 3.1415926535
            #define Pi316 0.0596831
            #define Pi14 0.07957747
            #define MieG float3(0.4375f, 1.5625f, 1.5f)
            
            // Textures
            uniform sampler2D   _SunTexture;
            uniform sampler2D   _MoonTexture;
            uniform samplerCUBE _StarFieldTexture;

            // Directions
            uniform float3   _SunDirection;
            uniform float3   _MoonDirection;
            uniform float4x4 _SunMatrix;
            uniform float4x4 _MoonMatrix;
            uniform float4x4 _UpDirectionMatrix;
            uniform float4x4 _StarFieldMatrix;
            uniform float4x4 _StarFieldRotationMatrix;

            // Scattering
            uniform float  _FogScatteringScale;
            uniform int    _ScatteringMode;
            uniform float  _Kr;
            uniform float  _Km;
            uniform float3 _Rayleigh;
            uniform float3 _Mie;
            uniform float  _Scattering;
            uniform float  _Luminance;
            uniform float  _Exposure;
            uniform float4 _RayleighColor;
            uniform float4 _MieColor;
            uniform float4 _ScatteringColor;

            // Outer Space
            uniform float  _SunTextureSize;
            uniform float  _SunTextureIntensity;
            uniform float4 _SunTextureColor;
            uniform float  _MoonTextureSize;
            uniform float  _MoonTextureIntensity;
            uniform float4 _MoonTextureColor;
            uniform float  _StarsIntensity;
            uniform float  _MilkyWayIntensity;
            uniform float4 _StarFieldColor;

            // Raytracing moon sphere
            bool iSphere(in float3 origin, in float3 direction, in float3 position, in float radius, out float3 normalDirection)
            {
                float3 rc = origin - position;
                float c = dot(rc, rc) - (radius * radius);
                float b = dot(direction, rc);
                float d = b * b - c;
                float t = -b - sqrt(abs(d));
                float st = step(0.0, min(t, d));
                normalDirection = normalize(-position + (origin + direction * t));
                
                if (st > 0.0) { return true; }
                return false;
            }

            // Mesh data
            struct Attributes
            {
                float4 vertex : POSITION;
            };

            // Vertex to fragment
            struct Varyings
            {
                float4 Position : SV_POSITION;
                float3 WorldPos : TEXCOORD0;
                float3 SunPos   : TEXCOORD1;
                float3 MoonPos  : TEXCOORD2;
                float3 StarPos  : TEXCOORD3;
            };

            // Vertex shader
            Varyings vertex_program (Attributes v)
            {
                Varyings Output = (Varyings)0;
                
                Output.Position = UnityObjectToClipPos(v.vertex);
                Output.WorldPos = normalize(mul((float3x3)unity_WorldToObject, v.vertex.xyz));
                Output.WorldPos = normalize(mul((float3x3)_UpDirectionMatrix, Output.WorldPos));

                Output.SunPos = mul((float3x3)_SunMatrix, v.vertex.xyz) * _SunTextureSize;
                Output.StarPos = mul((float3x3)_StarFieldRotationMatrix, Output.WorldPos);
                Output.StarPos = mul((float3x3)_StarFieldMatrix, Output.StarPos);
                Output.MoonPos = mul((float3x3)_MoonMatrix, v.vertex.xyz) * 0.75 * _MoonTextureSize;
                Output.MoonPos.x *= -1.0;

                return Output;
            }

            // Fragment shader
            float4 fragment_program (Varyings Input) : SV_Target
            {
                // Directions
                float3 viewDir = normalize(Input.WorldPos);
                float sunCosTheta = dot(viewDir, _SunDirection);
                float moonCosTheta = dot(viewDir, _MoonDirection);
                float r = length(float3(0.0, 50.0, 0.0));
                float sunRise = saturate(dot(float3(0.0, 500.0, 0.0), _SunDirection) / r);
                float moonRise = saturate(dot(float3(0.0, 500.0, 0.0), _MoonDirection) / r);

                // Optical depth
                float zenith = acos(saturate(dot(float3(0.0, 1.0, 0.0), viewDir))) * _FogScatteringScale;
                float z = (cos(zenith) + 0.15 * pow(93.885 - ((zenith * 180.0f) / PI), -1.253));
                float SR = _Kr / z;
                float SM = _Km / z;

                // Extinction
                float3 fex = exp(-(_Rayleigh * SR  + _Mie * SM));
                float horizonExtinction = saturate((viewDir.y) * 1000.0) * fex.b;
                float moonExtinction = saturate((viewDir.y) * 2.5);
                float sunset = clamp(dot(float3(0.0, 1.0, 0.0), _SunDirection), 0.0, 0.5);
                float3 Esun = _ScatteringMode == 0 ? lerp(fex, (1.0 - fex), sunset) : _ScatteringColor;

                // Sun inScattering
                float  rayPhase = 2.0 + 0.5 * pow(sunCosTheta, 2.0);
                float  miePhase = MieG.x / pow(MieG.y - MieG.z * sunCosTheta, 1.5);
                float3 BrTheta  = Pi316 * _Rayleigh * rayPhase * _RayleighColor;
                float3 BmTheta  = Pi14  * _Mie * miePhase * _MieColor * sunRise;
                float3 BrmTheta = (BrTheta + BmTheta) / (_Rayleigh + _Mie);
                float3 inScatter = BrmTheta * Esun * _Scattering * (1.0 - fex);
                inScatter *= sunRise;

                // Moon inScattering
                rayPhase = 2.0 + 0.5 * pow(moonCosTheta, 2.0);
                miePhase = MieG.x / pow(MieG.y - MieG.z * moonCosTheta, 1.5);
                BrTheta  = Pi316 * _Rayleigh * rayPhase * _RayleighColor;
                BmTheta  = Pi14  * _Mie * miePhase * _MieColor * moonRise;
                BrmTheta = (BrTheta + BmTheta) / (_Rayleigh + _Mie);
                Esun = _ScatteringMode == 0 ? (1.0 - fex) : _ScatteringColor;
                float3 moonInScatter = BrmTheta * Esun * _Scattering * 0.1 * (1.0 - fex);
                moonInScatter *= 1.0 - sunRise;

                // Default night sky - When there is no moon in the sky
                BrmTheta = BrTheta / (_Rayleigh + _Mie);
                float3 skyLuminance = BrmTheta * _ScatteringColor * _Luminance * (1.0 - fex);

                // Sun texture
                float3 sun_texture = tex2D( _SunTexture, Input.SunPos + 0.5).rgb * _SunTextureColor * _SunTextureIntensity;
                sun_texture = pow(sun_texture, 2.0);
                sun_texture *= fex.b * saturate(sunCosTheta);

                // Moon sphere
                float3 rayOrigin = float3(0.0, 0.0, 0.0);//_WorldSpaceCameraPos;
                float3 rayDirection = viewDir;
                float3 moonPosition = _MoonDirection * 38400.0 * _MoonTextureSize;
                float3 normalDirection = float3(0.0, 0.0, 0.0);
                float3 moonColor = float3(0.0, 0.0, 0.0);
                float4 moonTexture = saturate(tex2D( _MoonTexture, Input.MoonPos.xy + 0.5) * moonCosTheta);
                float moonMask = 1.0 - moonTexture.a * _MoonTextureIntensity;
                if(iSphere(rayOrigin, rayDirection, moonPosition, 17370.0, normalDirection))
                {
                    float moonSphere = max(dot(normalDirection, _SunDirection), 0.0) * moonTexture.a * 2.0;
                    moonColor = moonTexture.rgb * moonSphere * _MoonTextureColor * _MoonTextureIntensity * moonExtinction;
                }

                // Starfield
                float4 starTex = texCUBE(_StarFieldTexture, Input.StarPos);
                float3 stars = starTex.rgb * pow(starTex.a, 2.0) * _StarsIntensity;
                float3 milkyWay = pow(starTex.rgb, 1.5) * _MilkyWayIntensity;
                float3 starfield = (stars + milkyWay) * _StarFieldColor * horizonExtinction * moonMask;

                // Output
                float3 OutputColor = inScatter + sun_texture + skyLuminance + moonInScatter + moonColor + starfield;

                // Tonemapping
                OutputColor = saturate(1.0 - exp(-_Exposure * OutputColor));

                // Color correction
                OutputColor = pow(OutputColor, 2.2);
                #ifdef UNITY_COLORSPACE_GAMMA
                OutputColor = pow(OutputColor, 0.4545);
                #endif

                return float4(OutputColor, 1.0);
            }
            
            ENDHLSL
        }
    }
}