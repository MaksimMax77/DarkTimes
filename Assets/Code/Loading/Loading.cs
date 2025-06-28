using System;

namespace Code.Loading
{
    public class Loading
    {
        public event Action OnLoadEnd;
        private LoadingView _loadingView;
        private ILoadableItem _loadableItem;
        
        public Loading(LoadingView loadingView, ILoadableItem loadableItem)
        {
            _loadingView = loadingView;
            _loadableItem = loadableItem;
            _loadableItem.OnProgressChanged += OnProgressChanged;
        }

        public void Dispose()
        {
            _loadableItem.OnProgressChanged -= OnProgressChanged;
        }

        private void OnProgressChanged(float progress)
        {
            _loadingView.SetTargetProgress(progress);
        }

        public void StartLoading()
        {
            _loadingView.SetDescription(_loadableItem.Description);
            _loadingView.Filled += OnFilled;
            _loadingView.StartUpdateProgressBar();  
        }

        private void OnFilled()
        {
            _loadingView.Filled -= OnFilled;
            OnLoadEnd?.Invoke();
        }
    }
}
