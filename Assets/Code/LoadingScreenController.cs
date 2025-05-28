using UnityEngine;
using UnityEngine.UI;

namespace Code
{
    public class LoadingScreenController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvas;
        [SerializeField] private Slider _progressBar;

        public void Show()
        {
            _canvas.alpha = 1f;
        }

        public void Hide()
        {
            _canvas.alpha = 0f;
        }

        public void SetProgress(float progress)
        {
            _progressBar.value = progress;
        }
    }
}
