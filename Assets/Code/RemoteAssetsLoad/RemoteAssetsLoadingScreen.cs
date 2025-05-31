using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace Code.RemoteAssetsLoad
{
    public class RemoteAssetsLoadingScreen : MonoBehaviour, IDisposable
    {
        [SerializeField] private TMP_Text _tmpText;
        private RemoteAssetsLoadManager _remoteAssetsLoadManager;

        [Inject]
        public void Init(RemoteAssetsLoadManager remoteAssetsLoadManager)
        {
            _remoteAssetsLoadManager = remoteAssetsLoadManager;
            remoteAssetsLoadManager.LoadingProgressChanged += UpdateProgress;
        }

        public void Dispose()
        {
            _remoteAssetsLoadManager.LoadingProgressChanged -= UpdateProgress;
        }

        private void UpdateProgress(float value)
        {
            _tmpText.text = $"Загрузка: {value}%";
        }
    }
}
