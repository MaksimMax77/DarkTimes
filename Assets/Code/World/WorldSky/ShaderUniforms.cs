using UnityEngine;

namespace Code.World.WorldSky
{
    internal static class ShaderUniforms 
    {
        internal static readonly int SunTexture = Shader.PropertyToID("_SunTexture");
        internal static readonly int MoonTexture = Shader.PropertyToID("_MoonTexture");
        internal static readonly int StarFieldTexture = Shader.PropertyToID("_StarFieldTexture");
        internal static readonly int DynamicCloudTexture = Shader.PropertyToID("_DynamicCloudTexture");
        internal static readonly int StaticCloudTexture = Shader.PropertyToID("_StaticCloudTexture");

        internal static readonly int SunDirection = Shader.PropertyToID("_SunDirection");
        internal static readonly int MoonDirection = Shader.PropertyToID("_MoonDirection");
        internal static readonly int SunMatrix = Shader.PropertyToID("_SunMatrix");
        internal static readonly int MoonMatrix = Shader.PropertyToID("_MoonMatrix");
        internal static readonly int UpDirectionMatrix = Shader.PropertyToID("_UpDirectionMatrix");
        internal static readonly int StarfieldMatrix = Shader.PropertyToID("_StarFieldMatrix");

        internal static readonly int ScatteringMode = Shader.PropertyToID("_ScatteringMode");
        internal static readonly int Kr = Shader.PropertyToID("_Kr");
        internal static readonly int Km = Shader.PropertyToID("_Km");
        internal static readonly int Rayleigh = Shader.PropertyToID("_Rayleigh");
        internal static readonly int Mie = Shader.PropertyToID("_Mie");
        internal static readonly int MieDistance = Shader.PropertyToID("_MieDepth");
        internal static readonly int Scattering = Shader.PropertyToID("_Scattering");
        internal static readonly int Luminance = Shader.PropertyToID("_Luminance");
        internal static readonly int Exposure = Shader.PropertyToID("_Exposure");
        internal static readonly int RayleighColor = Shader.PropertyToID("_RayleighColor");
        internal static readonly int MieColor = Shader.PropertyToID("_MieColor");
        internal static readonly int ScatteringColor = Shader.PropertyToID("_ScatteringColor");

        internal static readonly int SunTextureSize = Shader.PropertyToID("_SunTextureSize");
        internal static readonly int SunTextureIntensity = Shader.PropertyToID("_SunTextureIntensity");
        internal static readonly int SunTextureColor = Shader.PropertyToID("_SunTextureColor");
        internal static readonly int MoonTextureSize = Shader.PropertyToID("_MoonTextureSize");
        internal static readonly int MoonTextureIntensity = Shader.PropertyToID("_MoonTextureIntensity");
        internal static readonly int MoonTextureColor = Shader.PropertyToID("_MoonTextureColor");
        internal static readonly int StarsIntensity = Shader.PropertyToID("_StarsIntensity");
        internal static readonly int MilkyWayIntensity = Shader.PropertyToID("_MilkyWayIntensity");
        internal static readonly int StarFieldColor = Shader.PropertyToID("_StarFieldColor");
        internal static readonly int StarFieldRotation = Shader.PropertyToID("_StarFieldRotationMatrix");
        
        internal static readonly int DynamicCloudAltitude = Shader.PropertyToID("_DynamicCloudAltitude");
        internal static readonly int DynamicCloudDirection = Shader.PropertyToID("_DynamicCloudDirection");
        internal static readonly int DynamicCloudDensity = Shader.PropertyToID("_DynamicCloudDensity");
        internal static readonly int DynamicCloudColor1 = Shader.PropertyToID("_DynamicCloudColor1");
        internal static readonly int DynamicCloudColor2 = Shader.PropertyToID("_DynamicCloudColor2");
        internal static readonly int ThunderLightningEffect = Shader.PropertyToID("_ThunderLightningEffect");
        internal static readonly int StaticCloudInterpolator = Shader.PropertyToID("_StaticCloudInterpolator");
        internal static readonly int StaticCloudLayer1Speed = Shader.PropertyToID("_StaticCloudLayer1Speed");
        internal static readonly int StaticCloudLayer2Speed = Shader.PropertyToID("_StaticCloudLayer2Speed");
        internal static readonly int StaticCloudColor = Shader.PropertyToID("_StaticCloudColor");
        internal static readonly int StaticCloudScattering = Shader.PropertyToID("_StaticCloudScattering");
        internal static readonly int StaticCloudExtinction = Shader.PropertyToID("_StaticCloudExtinction");
        internal static readonly int StaticCloudSaturation = Shader.PropertyToID("_StaticCloudSaturation");
        internal static readonly int StaticCloudOpacity = Shader.PropertyToID("_StaticCloudOpacity");
    }
}
