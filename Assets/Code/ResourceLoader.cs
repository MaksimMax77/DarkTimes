using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourceLoader : MonoBehaviour
{
    public async Task LoadAllEssentialAssets(Action<float> onProgress)
    {
        var handle = Addressables.DownloadDependenciesAsync("Essential");
        while (!handle.IsDone)
        {
            onProgress?.Invoke(handle.PercentComplete);
            await Task.Yield();
        }
    }
}
