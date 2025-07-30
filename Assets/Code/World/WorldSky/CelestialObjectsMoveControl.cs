using UnityEngine;

namespace Code.World.WorldSky
{
    [ExecuteInEditMode]
    public class CelestialObjectsMoveControl : MonoBehaviour
    {
        [SerializeField] private Transform _sunTransform;
        [SerializeField] private Transform _moonTransform;
        [SerializeField] private Transform _directionalLight;
        [SerializeField] private float _minLightAltitude;
        [SerializeField] private WorldTimeControl _worldTimeControl;
        private float _sunElevation;
        private Vector3 _sunLocalDirection;
        private Vector3 _moonLocalDirection;
        private Vector3 _lightLocalDirection;
    
        private void Awake()
        {
            ComputeCelestialCoordinates();
            EvaluateSunMoonElevation();
            SetDirectionalLightRotation();
        }

        private void Update()
        {
            ComputeCelestialCoordinates();
            EvaluateSunMoonElevation();
            SetDirectionalLightRotation();
        }
    
        private void ComputeCelestialCoordinates()
        {
            _sunTransform.localRotation 
                = Quaternion.Euler( _worldTimeControl.CurrentTime * 360.0f / 24.0f - 90.0f, 180.0f, 0.0f); 
            _moonTransform.localRotation = _sunTransform.localRotation * Quaternion.Euler(0, -180, 0);
            Shader.SetGlobalMatrix(ShaderUniforms.StarfieldMatrix, _sunTransform.worldToLocalMatrix);
        }
    
        private void EvaluateSunMoonElevation()
        {
            _sunLocalDirection = _sunTransform.forward;
            _moonLocalDirection = _moonTransform.forward;
            _sunElevation = Vector3.Dot(-_sunLocalDirection, Vector3.up);
        }
    
        private void SetDirectionalLightRotation()
        {
            _directionalLight.localRotation =
                Quaternion.LookRotation(_sunElevation >= 0.0f ? _sunLocalDirection : _moonLocalDirection);
            
            if (_minLightAltitude > 0f && _minLightAltitude < 90f)
            {
                _lightLocalDirection = _directionalLight.localEulerAngles;

                if (_lightLocalDirection.x <= _minLightAltitude)
                {
                    _lightLocalDirection.x = _minLightAltitude;
                }
                
                _directionalLight.localEulerAngles = _lightLocalDirection;
            }
        }
    }
}
