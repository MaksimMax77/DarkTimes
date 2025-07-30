using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Loading
{
    public class LoadingView : MonoBehaviour
    {
        public event Action Filled;
        [SerializeField] private float _destroyTime = 0.1f;
        [SerializeField] private float _fillSpeed = 1f;
        [SerializeField] private Slider _progressFill;
        [SerializeField] private TMP_Text _description;
        private float _targetProgress;
        
        public void StartDestroy()
        {
            Destroy(gameObject, _destroyTime);
        }
        
        public void SetTargetProgress(float progress)
        {
            _targetProgress = progress;
        }

        public void SetDescription(string description)
        {
            _description.text = description;
        }

        public void StartUpdateProgressBar()
        {
            _progressFill.value = 0;
            StartCoroutine(UpdateProgressBar());
        }

        private IEnumerator UpdateProgressBar()
        {
            while (_progressFill.value < _progressFill.maxValue)
            {
                if (_progressFill.value < _targetProgress)
                {
                    _progressFill.value += UnityEngine.Time.deltaTime * _fillSpeed;
                }
                yield return null;
            }

            _progressFill.value = 1;
            Filled?.Invoke();
        }
    }
}