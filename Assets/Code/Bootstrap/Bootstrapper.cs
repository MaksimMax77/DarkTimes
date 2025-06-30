using System.Collections.Generic;
using Code.AssetsLoad;
using Code.Error;
using Code.Loading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Code.Bootstrap
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private string _labelToLoad = "RemoteResources";
        [SerializeField] private string _mainSceneName = "MainMenu";
        [SerializeField] private LoadingView _loadingView;
        [Inject] private RemoteAssetsDownloader _remoteAssetsDownloader;
        [Inject] private SceneLoader _sceneLoader;
        [Inject] private LoadingsControl _loadingsControl;
        [Inject] private ErrorControl _errorControl;
        
        private async UniTaskVoid Start()
        {
            _loadingsControl.SetLoadingView(_loadingView);
            _errorControl.RestartClicked += OnRestartClicked;
            await DownloadResourcesAndLoadMain();
        }

        private void OnDestroy()
        {
            _errorControl.RestartClicked -= OnRestartClicked;
        }

        private async UniTask DownloadResourcesAndLoadMain()
        {
            var downloadSizeResult = await _remoteAssetsDownloader.GetDownloadSizeAsync(_labelToLoad);

            if (downloadSizeResult > 0)
            {
                _loadingsControl.CreateLoadingsAndStartLoad(new List<ILoadableItem>()
                {
                    _remoteAssetsDownloader,
                    _sceneLoader
                });

                var loadRemoteContentStatus = await _remoteAssetsDownloader.LoadRemoteContentAsync(_labelToLoad);

                if (loadRemoteContentStatus == AsyncOperationStatus.Succeeded)
                {
                    await LoadMainScene();
                }
            }
            else
            {
                _loadingsControl.CreateLoadingsAndStartLoad(new List<ILoadableItem>()
                {
                    _sceneLoader
                });
                
                await LoadMainScene();
            }
        }
        
        private async UniTask LoadMainScene()
        {
            await _sceneLoader.LoadSceneAsync(_mainSceneName);
        }

        private async void OnRestartClicked()
        {
            await DownloadResourcesAndLoadMain();
        }
    }
}