using System;
using Code.AssetsLoad.Info;
using Code.Loading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Code.AssetsLoad
{
    public class SceneLoader : ILoadableItem
    {
        public event Action<float> OnProgressChanged;
        public event Action<string> OnError;

        public string Description { get; set; }

        public SceneLoader(SceneLoaderInfo sceneLoaderInfo)
        {
            Description = sceneLoaderInfo.Description;
        }
        
        public async UniTask LoadSceneAsync(string key)
        {
            var handle = Addressables.LoadSceneAsync(key);

            try
            {
                await UniTask.WhenAll(
                    MonitorProgress(handle),
                    handle.Task.AsUniTask().SuppressCancellationThrow()
                );
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex.Message);
                throw new Exception("Загрузка сцены завершилась с ошибкой.");
            }
        }
        
        private async UniTask MonitorProgress(AsyncOperationHandle<SceneInstance> handle)
        {
            while (!handle.IsDone)
            {
                OnProgressChanged?.Invoke(handle.PercentComplete);
                await UniTask.Yield();
            }
        }
    }
}