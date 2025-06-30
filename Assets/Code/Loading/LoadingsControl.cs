using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Code.Loading
{
    public class LoadingsControl : IDisposable
    { 
        private LoadingView _loadingView;
        private Queue<Loading> _loadings = new();

        public void SetLoadingView(LoadingView loadingView)
        {
            _loadingView = loadingView;
            Object.DontDestroyOnLoad(_loadingView);
        }

        public void CreateLoadingsAndStartLoad(List<ILoadableItem> loadableItems)
        {
            RemoveAllLoadings();

            for (int i = 0; i < loadableItems.Count; i++)
            {
                _loadings.Enqueue(new Loading(_loadingView, loadableItems[i]));
            }

            var activeLoading = _loadings.Peek();
            activeLoading.OnLoadEnd += OnLoadEnd;
            activeLoading.StartLoading();
        }

        private void RemoveAllLoadings()
        {
            foreach (var loading in _loadings)
            {
                loading.OnLoadEnd -= OnLoadEnd;
                loading.Dispose();
            }

            _loadings.Clear();
        }
        
        private void OnLoadEnd()
        {
            var loading = _loadings.Dequeue();
            loading.OnLoadEnd -= OnLoadEnd;
            loading.Dispose();

            if (_loadings.Count == 0)
            {
                _loadingView.StartDestroy();
                return;
            }

            var nextLoading = _loadings.Peek();
            nextLoading.OnLoadEnd += OnLoadEnd;
            nextLoading.StartLoading();
        }

        public void Dispose()
        {
            foreach (var loading in _loadings)
            {
                loading.OnLoadEnd -= OnLoadEnd;
            }
        }
    }
}
