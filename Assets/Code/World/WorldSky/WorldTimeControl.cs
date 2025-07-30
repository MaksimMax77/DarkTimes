using UnityEngine;

namespace Code.World.WorldSky
{
    public class WorldTimeControl : MonoBehaviour
    {
        [Range(0, 24)] [SerializeField] private float _currentTime = 6.0f;
        [SerializeField] private float _timelineTransitionTargetSpeed = 1f;
        [SerializeField] private int _hour = 6;
        [SerializeField] private int _minute;
        
        public float CurrentTime => _currentTime;

        private void Awake()
        {
            EvaluateTimeOfDay();
        }

        private void Update()
        {
            _currentTime += _timelineTransitionTargetSpeed * Time.deltaTime;
            
            if (_currentTime > 24)
            {
                _currentTime = 0;
            }

            EvaluateTimeOfDay();
        }

        private void OnValidate()
        {
            EvaluateTimeOfDay();
        }
        
        private void EvaluateTimeOfDay()
        {
            _hour = (int)Mathf.Floor(_currentTime);
            _minute = (int)Mathf.Floor(_currentTime * 60 % 60);
        }
    }
}