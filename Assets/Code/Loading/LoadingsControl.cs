using System.Collections.Generic;
using UnityEngine;

namespace Code.Loading
{
    public class LoadingsControl
    {
        private LoadingView _loadingViewPrefab;
        private LoadingView _loadingView;
        private Queue<Loading> _loadings = new();

        public LoadingsControl(LoadingView loadingViewPrefab)
        {
            _loadingViewPrefab = loadingViewPrefab;
        }

        public void CreateLoadingsAndStartLoad(List<ILoadableItem> loadableItems)
        {
            RemoveAllLoadings();
            TryCreateViewInstance();

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

        private void TryCreateViewInstance()
        {
            if (_loadingView != null)
            {
                return;
            }

            _loadingView = Object.Instantiate(_loadingViewPrefab);
            Object.DontDestroyOnLoad(_loadingView);
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
    }
}
