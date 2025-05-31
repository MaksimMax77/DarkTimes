using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class RemoteAssetsLoadManager
{
    public event Action<float> LoadingProgressChanged;
    private string _labelToLoad = "RemoteResources";
    
    public RemoteAssetsLoadManager()
    {
        LoadRemoteContentAsync(_labelToLoad).Forget();
    }
    
    private async UniTask LoadRemoteContentAsync(string label)
    {
        var sizeHandle = Addressables.GetDownloadSizeAsync(label);
        await sizeHandle.ToUniTask();

        if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to get download size.");
            return;
        }

        if (sizeHandle.Result == 0)
        {
            Debug.LogError("Download size = 0");
            return;
        }

        var downloadHandle = Addressables.DownloadDependenciesAsync(label);
        while (!downloadHandle.IsDone)
        {
            LoadingProgressChanged?.Invoke(downloadHandle.PercentComplete * 100);
            await UniTask.Yield();
        }

        await downloadHandle.ToUniTask();

        switch (downloadHandle.Status)
        {
            case AsyncOperationStatus.Failed:
                Debug.LogError("Download failed!");
                break;
            case AsyncOperationStatus.Succeeded:
                LoadingProgressChanged?.Invoke(downloadHandle.PercentComplete * 100);
                break;
        }
    }
}