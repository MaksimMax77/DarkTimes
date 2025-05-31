using Code.RemoteAssetsLoad;
using UnityEngine;
using Zenject;

namespace Code.Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private RemoteAssetsLoadingScreen _remoteAssetsLoadingScreenPrefab;

        public override void InstallBindings()
        {
            Container.Bind<RemoteAssetsLoadingScreen>().FromInstance(_remoteAssetsLoadingScreenPrefab).AsSingle();
            Container.Bind<RemoteAssetsLoadManager>().AsSingle().NonLazy();
        }
    }
}
