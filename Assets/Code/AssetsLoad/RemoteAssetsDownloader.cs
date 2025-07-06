using System;
using Code.AssetsLoad.Info;
using Code.Error;
using Code.Loading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Code.AssetsLoad
{
    public class RemoteAssetsDownloader : ILoadableItem, IObjectWithError
    {
        public event Action<string> OnError;
        public event Action<float> OnProgressChanged;
        private string _errorMessage;
        
        public string Description { get; set; }

        public RemoteAssetsDownloader(RemoteAssetsDownloaderInfo info)
        {
            _errorMessage = info.ErrorMessage;
            Description = info.Description;
        }

        public async UniTask<long> GetDownloadSizeAsync(string label)
        {
            var handle = Addressables.GetDownloadSizeAsync(label);

            try
            {
                await handle.ToUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    return handle.Result;
                }
            }
            catch (Exception)
            {
                OnError?.Invoke(_errorMessage);
            }
            finally
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            
            return handle.Result;
        }

        public async UniTask<AsyncOperationStatus> LoadRemoteContentAsync(string label)
        {
            var status = AsyncOperationStatus.None;
            var handle = Addressables.DownloadDependenciesAsync(label);

            try
            {
                await UniTask.WhenAll(
                    MonitorProgress(handle),
                    handle.ToUniTask()
                );
                
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    status = AsyncOperationStatus.Succeeded;
                    OnProgressChanged?.Invoke(1f);
                }
            }
            catch (Exception)
            {
                status = AsyncOperationStatus.Failed;
                OnError?.Invoke(_errorMessage);
            }
            finally
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }

            return status;
        }
        
        private async UniTask MonitorProgress(AsyncOperationHandle handle)
        {
            while (!handle.IsDone)
            {
                OnProgressChanged?.Invoke(handle.GetDownloadStatus().Percent);
                await UniTask.Yield();
            }
        }
    }
}