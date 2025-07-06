using Code.AssetsLoad;
using Code.AssetsLoad.Info;
using Code.Loading;
using UnityEngine;
using Zenject;

namespace Code.Installers
{
    public class ProjectContextInstaller : MonoInstaller
    {
        [SerializeField] private RemoteAssetsDownloaderInfo _remoteAssetsDownloaderInfo;
        [SerializeField] private SceneLoaderInfo _sceneLoaderInfo;

        public override void InstallBindings()
        {
            Container.BindInstance(_remoteAssetsDownloaderInfo).AsSingle();
            Container.BindInstance(_sceneLoaderInfo).AsSingle();
            Container.BindInterfacesAndSelfTo<RemoteAssetsDownloader>().AsSingle();
            Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<LoadingsControl>().AsSingle();
        }
    }
}