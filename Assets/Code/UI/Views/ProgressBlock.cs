using System;
using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Views
{
    public class ProgressBlock : MonoBehaviour
    {
        public event Action Filled;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Slider _progressFill;
        [SerializeField] private float _barSpeed;
        private Coroutine _progressCoroutine;
        private bool _filled;

        public void SetDescription(string description)
        {
            _description.text = description;
        }

        public void StartUpdateProgressBar(float targetProgress)
        {
            _progressCoroutine = StartCoroutine(UpdateProgressBar(targetProgress/*, _barSpeed*/));
        }
        
        /*private IEnumerator UpdateProgressBar(float targetProgress, float duration)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                _progressFill.value = Mathf.Lerp(0, targetProgress, elapsed / duration);//todo плохо работает
                elapsed += Time.deltaTime;
                yield return null;
            }

            _progressFill.value = targetProgress;

            if (Mathf.Approximately(_progressFill.value, _progressFill.maxValue))
            {
                Filled?.Invoke();
            }
        }*/
        
        private IEnumerator UpdateProgressBar(float targetProgress)
        {
            while (gameObject.activeInHierarchy)
            {
                if (_progressFill.value < targetProgress)
                {
                    _progressFill.value += Time.deltaTime * _barSpeed;
                }

                if (Mathf.Approximately(_progressFill.value, _progressFill.maxValue))
                {
                    Filled?.Invoke();
                }
                yield return null;
            }
            
            /*if (Mathf.Approximately(_progressFill.value, _progressFill.maxValue))
            {
                Filled?.Invoke();
            }*/
        }

        public void ResetFill()
        {
            if (_progressCoroutine != null)
            {
                StopCoroutine(_progressCoroutine);
            }

            _progressFill.value = 0;
        }
    }
}