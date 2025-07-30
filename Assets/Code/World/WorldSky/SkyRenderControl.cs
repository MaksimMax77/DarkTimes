using UnityEngine;

namespace Code.World.WorldSky
{
    [ExecuteInEditMode]
    public class SkyRenderControl : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private bool _referencesHeaderGroup;
        [SerializeField] private bool _scatteringHeaderGroup;
        [SerializeField] private bool _outerSpaceHeaderGroup;
        [SerializeField] private bool _fogScatteringHeaderGroup;
        [SerializeField] private bool _cloudsHeaderGroup;
        [SerializeField] private bool _optionsHeaderGroup;
#endif

        // References
        [SerializeField] private Transform _sunTransform = null;
        [SerializeField] private Transform _moonTransform = null;
        [SerializeField] private Material _skyMaterial = null;
        [SerializeField] private Shader _emptySkyShader = null;
        [SerializeField] private Shader _staticCloudsShader = null;
        [SerializeField] private Shader _dynamicCloudsShader = null;
        [SerializeField] private Texture2D _sunTexture = null;
        [SerializeField] private Texture2D _moonTexture = null;
        [SerializeField] private Cubemap _starfieldTexture = null;
        [SerializeField] private Texture2D _dynamicCloudsTexture = null;
        public Texture2D staticCloudTexture = null;

        // Scattering
        public float molecularDensity = 2.545f;
        public float wavelengthR = 680.0f;
        public float wavelengthG = 550.0f;
        public float wavelengthB = 450.0f;
        public float kr = 8.4f;
        public float km = 1.2f;
        public float rayleigh = 1.5f;
        public float mie = 1.0f;
        public float mieDistance = 1.0f;
        public float scattering = 0.25f;
        public float luminance = 1.5f;
        public float exposure = 2.0f;
        public Color rayleighColor = Color.white;
        public Color mieColor = Color.white;
        public Color scatteringColor = Color.white;

        // Outer space
        public float sunTextureSize = 1.5f;
        public float sunTextureIntensity = 1.0f;
        public Color sunTextureColor = Color.white;
        public float moonTextureSize = 1.5f;
        public float moonTextureIntensity = 1.0f;
        public Color moonTextureColor = Color.white;
        public float starsIntensity = 0.5f;
        public float milkyWayIntensity = 0.0f;
        public Color starfieldColor = Color.white;
        public float starfieldRotationX = 0.0f;
        public float starfieldRotationY = 0.0f;
        public float starfieldRotationZ = 0.0f;

        // Clouds
        public float dynamicCloudsAltitude = 7.5f;
        public float dynamicCloudsDirection;
        public float dynamicCloudsSpeed = 0.1f;
        public float dynamicCloudsDensity = 0.75f;
        public Color dynamicCloudsColor1 = Color.white;
        public Color dynamicCloudsColor2 = Color.white;
        public float staticCloudLayer1Speed = 0.0025f;
        public float staticCloudLayer2Speed = 0.0075f;
        public float staticCloudScattering = 1.0f;
        public float staticCloudExtinction = 1.5f;
        public float staticCloudSaturation = 2.5f;
        public float staticCloudOpacity = 1.25f;
        public Color staticCloudColor = Color.white;
        private Vector2 _dynamicCloudsDirection = Vector2.zero;
        private float _staticCloudLayer1Speed;
        private float _staticCloudLayer2Speed;
        
        [SerializeField] private ScatteringMode _scatteringMode = ScatteringMode.Automatic;
        [SerializeField] private CloudMode _cloudMode = CloudMode.Dynamic;
        private Quaternion _starfieldRotation;
        private Matrix4x4 _starfieldRotationMatrix;

        private void Awake()
        {
            _dynamicCloudsDirection = Vector2.zero;
            InitializeShaderUniforms();
        }

        private void OnEnable()
        {
            if (_skyMaterial)
                RenderSettings.skybox = _skyMaterial;
        }


        private void LateUpdate()
        {
            // In editor only
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                InitializeShaderUniforms();

                if (_skyMaterial)
                    RenderSettings.skybox = _skyMaterial;
            }
            #endif

            // Clouds movement
            _dynamicCloudsDirection = ComputeCloudPosition();
            _staticCloudLayer1Speed += staticCloudLayer1Speed * Time.deltaTime;
            _staticCloudLayer2Speed += staticCloudLayer2Speed * Time.deltaTime;
            if (_staticCloudLayer1Speed >= 1.0f) { _staticCloudLayer1Speed -= 1.0f; }
            if (_staticCloudLayer2Speed >= 1.0f) { _staticCloudLayer2Speed -= 1.0f; }
            UpdateShaderUniforms();
        }

        private void InitializeShaderUniforms()
        {
            _skyMaterial.SetTexture(ShaderUniforms.SunTexture, _sunTexture);
            _skyMaterial.SetTexture(ShaderUniforms.MoonTexture, _moonTexture);
            _skyMaterial.SetTexture(ShaderUniforms.StarFieldTexture, _starfieldTexture);
            _skyMaterial.SetTexture(ShaderUniforms.DynamicCloudTexture, _dynamicCloudsTexture);
            _skyMaterial.SetTexture(ShaderUniforms.StaticCloudTexture, staticCloudTexture);
        }

        private void UpdateShaderUniforms()
        {
            _starfieldRotation = Quaternion.Euler(starfieldRotationX, starfieldRotationY, starfieldRotationZ);
            _starfieldRotationMatrix = Matrix4x4.TRS(Vector3.zero, _starfieldRotation, Vector3.one);
            
            UpdateLocalShaderUniforms(_skyMaterial);
        }

        private void UpdateLocalShaderUniforms(Material mat)
        {
            mat.SetVector(ShaderUniforms.SunDirection, transform.InverseTransformDirection(-_sunTransform.forward));
            mat.SetVector(ShaderUniforms.MoonDirection, transform.InverseTransformDirection(-_moonTransform.forward));
            mat.SetMatrix(ShaderUniforms.SunMatrix, _sunTransform.worldToLocalMatrix);
            mat.SetMatrix(ShaderUniforms.MoonMatrix, _moonTransform.worldToLocalMatrix);
            mat.SetMatrix(ShaderUniforms.UpDirectionMatrix, transform.worldToLocalMatrix);
            mat.SetInt(ShaderUniforms.ScatteringMode, (int)_scatteringMode);
            mat.SetFloat(ShaderUniforms.Kr, kr * 1000f);
            mat.SetFloat(ShaderUniforms.Km, km * 1000f);
            mat.SetVector(ShaderUniforms.Rayleigh, ComputeRayleigh() * rayleigh);
            mat.SetVector(ShaderUniforms.Mie, ComputeMie() * mie);
            mat.SetFloat(ShaderUniforms.MieDistance, mieDistance);
            mat.SetFloat(ShaderUniforms.Scattering, scattering * 60f);
            mat.SetFloat(ShaderUniforms.Luminance, luminance);
            mat.SetFloat(ShaderUniforms.Exposure, exposure);
            mat.SetColor(ShaderUniforms.RayleighColor, rayleighColor);
            mat.SetColor(ShaderUniforms.MieColor, mieColor);
            mat.SetColor(ShaderUniforms.ScatteringColor, scatteringColor);
            mat.SetFloat(ShaderUniforms.SunTextureSize, sunTextureSize);
            mat.SetFloat(ShaderUniforms.SunTextureIntensity, sunTextureIntensity);
            mat.SetColor(ShaderUniforms.SunTextureColor, sunTextureColor);
            mat.SetFloat(ShaderUniforms.MoonTextureSize, moonTextureSize);
            mat.SetFloat(ShaderUniforms.MoonTextureIntensity, moonTextureIntensity);
            mat.SetColor(ShaderUniforms.MoonTextureColor, moonTextureColor);
            mat.SetFloat(ShaderUniforms.StarsIntensity, starsIntensity);
            mat.SetFloat(ShaderUniforms.MilkyWayIntensity, milkyWayIntensity);
            mat.SetColor(ShaderUniforms.StarFieldColor, starfieldColor);
            mat.SetMatrix(ShaderUniforms.StarFieldRotation, _starfieldRotationMatrix);
            mat.SetFloat(ShaderUniforms.DynamicCloudAltitude, dynamicCloudsAltitude);
            mat.SetVector(ShaderUniforms.DynamicCloudDirection, _dynamicCloudsDirection);
            mat.SetFloat(ShaderUniforms.DynamicCloudDensity, Mathf.Lerp(25.0f, 0.0f, dynamicCloudsDensity));
            mat.SetVector(ShaderUniforms.DynamicCloudColor1, dynamicCloudsColor1);
            mat.SetVector(ShaderUniforms.DynamicCloudColor2, dynamicCloudsColor2);
            mat.SetFloat(ShaderUniforms.StaticCloudLayer1Speed, _staticCloudLayer1Speed);
            mat.SetFloat(ShaderUniforms.StaticCloudLayer2Speed, _staticCloudLayer2Speed);
            mat.SetFloat(ShaderUniforms.StaticCloudScattering, staticCloudScattering);
            mat.SetFloat(ShaderUniforms.StaticCloudExtinction, staticCloudExtinction);
            mat.SetFloat(ShaderUniforms.StaticCloudSaturation, staticCloudSaturation);
            mat.SetFloat(ShaderUniforms.StaticCloudOpacity, staticCloudOpacity);
            mat.SetVector(ShaderUniforms.StaticCloudColor, staticCloudColor);
        }
        
        /// <summary>
        /// Total rayleigh computation.
        /// </summary>
        private Vector3 ComputeRayleigh()
        {
            Vector3 rayleigh = Vector3.one;
            Vector3 lambda = new Vector3(wavelengthR, wavelengthG, wavelengthB) * 1e-9f;
            float n = 1.0003f; // Refractive index of air
            float pn = 0.035f; // Depolarization factor for standard air.
            float n2 = n * n;
            //float N = 2.545E25f;
            float N = molecularDensity * 1E25f;
            float temp = (8.0f * Mathf.PI * Mathf.PI * Mathf.PI * ((n2 - 1.0f) * (n2 - 1.0f))) / (3.0f * N) * ((6.0f + 3.0f * pn) / (6.0f - 7.0f * pn));

            rayleigh.x = temp / Mathf.Pow(lambda.x, 4.0f);
            rayleigh.y = temp / Mathf.Pow(lambda.y, 4.0f);
            rayleigh.z = temp / Mathf.Pow(lambda.z, 4.0f);

            return rayleigh;
        }

        /// <summary>
        /// Total mie computation.
        /// </summary>
        private Vector3 ComputeMie()
        {
            Vector3 mie;

            //float c = (0.6544f * Turbidity - 0.6510f) * 1e-16f;
            float c = (0.6544f * 5.0f - 0.6510f) * 10f * 1e-9f;
            Vector3 k = new Vector3(686.0f, 678.0f, 682.0f);

            mie.x = (434.0f * c * Mathf.PI * Mathf.Pow((4.0f * Mathf.PI) / wavelengthR, 2.0f) * k.x);
            mie.y = (434.0f * c * Mathf.PI * Mathf.Pow((4.0f * Mathf.PI) / wavelengthG, 2.0f) * k.y);
            mie.z = (434.0f * c * Mathf.PI * Mathf.Pow((4.0f * Mathf.PI) / wavelengthB, 2.0f) * k.z);

            //float c = (6544f * 5.0f - 6510f) * 10.0f * 1.0e-9f;
            //mie.x = (0.434f * c * Mathf.PI * Mathf.Pow((2.0f * Mathf.PI) / wavelengthR, 2.0f) * k.x) / 3.0f;
            //mie.y = (0.434f * c * Mathf.PI * Mathf.Pow((2.0f * Mathf.PI) / wavelengthG, 2.0f) * k.y) / 3.0f;
            //mie.z = (0.434f * c * Mathf.PI * Mathf.Pow((2.0f * Mathf.PI) / wavelengthB, 2.0f) * k.z) / 3.0f;

            return mie;
        }

        /// <summary>
        /// Returns the cloud uv position based on the direction and speed.
        /// </summary>
        private Vector2 ComputeCloudPosition()
        {
            float x = _dynamicCloudsDirection.x;
            float z = _dynamicCloudsDirection.y;
            float dir = Mathf.Lerp(0f, 360f, dynamicCloudsDirection);
            float windSpeed = dynamicCloudsSpeed * 0.05f * Time.deltaTime;

            x += windSpeed * Mathf.Sin(0.01745329f * dir);
            z += windSpeed * Mathf.Cos(0.01745329f * dir);

            if (x >= 1.0f) x -= 1.0f;
            if (z >= 1.0f) z -= 1.0f;

            return new Vector2(x, z);
        }

        /// <summary>
        /// Updates the material settings if there is a change from Inspector.
        /// </summary>
        public void UpdateSkySettings()
        {
            switch (_cloudMode)
            {
                case CloudMode.Off:
                    _skyMaterial.shader = _emptySkyShader;
                    break;
                case CloudMode.Static:
                    _skyMaterial.shader = _staticCloudsShader;
                    break;
                case CloudMode.Dynamic:
                    _skyMaterial.shader = _dynamicCloudsShader;
                    break;
            }
        }
    }
    
    public enum ScatteringMode
    {
        Automatic,
        Custom
    }
    
    public enum CloudMode
    {
        Off,
        Static,
        Dynamic
    }
}
