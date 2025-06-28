using System;

namespace Code.Error
{
    public class ErrorControl : IDisposable
    {
        public event Action RestartClicked;
        private ErrorView _errorView;
        
        public ErrorControl(ErrorView errorView)
        {
            _errorView = errorView;
            _errorView.RestartButton.onClick.AddListener(RestartClickedInvoke);
        }

        public void Dispose()
        {
            _errorView.RestartButton.onClick.RemoveListener(RestartClickedInvoke);
        }

        public void ShowError(string message)
        {
            _errorView.SetDescription(message);
            _errorView.gameObject.SetActive(true);
        }

        public void Close()
        {
            _errorView.gameObject.SetActive(false);
        }

        private void RestartClickedInvoke()
        {
            RestartClicked?.Invoke();
        }
    }
}
