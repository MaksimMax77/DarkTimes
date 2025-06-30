using System;
using System.Collections.Generic;

namespace Code.Error
{
    public class ErrorControl: IDisposable
    {
        public event Action RestartClicked;
        private ErrorView _errorView;
        private List<IObjectWithError> _errorObjects;
        
        public ErrorControl(ErrorView errorView, List<IObjectWithError> errorObjects)
        {
            _errorView = errorView;
            _errorObjects = errorObjects;

            for (var i = _errorObjects.Count - 1; i >= 0; i--)
            {
                _errorObjects[i].OnError += ShowError;
            }
            _errorView.RestartButton.onClick.AddListener(RestartClickedInvoke);
        }
        
        public void Dispose()
        {
            for (var i = _errorObjects.Count - 1; i >= 0; i--)
            {
                _errorObjects[i].OnError -= ShowError;
            }
            _errorView.RestartButton.onClick.RemoveListener(RestartClickedInvoke);
        }

        private void ShowError(string message)
        {
            _errorView.SetDescription(message);
            _errorView.gameObject.SetActive(true);
        }

        private void RestartClickedInvoke()
        {
            _errorView.gameObject.SetActive(false);
            RestartClicked?.Invoke();
        }
    }
}
