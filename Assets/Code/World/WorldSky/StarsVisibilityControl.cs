using UnityEngine;

namespace Code.World.WorldSky
{
    [ExecuteInEditMode]
    public class StarsVisibilityControl : MonoBehaviour
    {
        [SerializeField] private WorldTimeControl _worldTimeControl;
        [SerializeField] private SkyRenderControl _skyRenderControl;
        [SerializeField] private float _maxVisibility = 1.3f;
        [SerializeField] private float _startFadeOut = 6.5f;
        [SerializeField] private float _endFadeOut = 8f;
        private float _startFadeIn = 17f;
        private float _endFadeIn = 18.5f;

        private void Update()
        {
            _skyRenderControl.milkyWayIntensity = CalculateStarsVisibility(_worldTimeControl.CurrentTime);
        }

        private float CalculateStarsVisibility(float time)
        {
            if (time >= _startFadeOut && time <= _endFadeOut)
            {
                return Mathf.Lerp(0f, _maxVisibility,
                    Mathf.InverseLerp(_endFadeOut, _startFadeOut, time));
            }

            if (time > _endFadeOut && time < _startFadeIn)
            {
                return 0f;
            }

            if (time >= _startFadeIn && time <= _endFadeIn)
            {
                return Mathf.Lerp(0f, _maxVisibility,
                    Mathf.InverseLerp(_startFadeIn, _endFadeIn, time));
            }

            return _maxVisibility;
        }
    }
}