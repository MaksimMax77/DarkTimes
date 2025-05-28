using UnityEngine;
using Zenject;

namespace Code.Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private LoadingScreenController _loadingScreenPrefab;

        public override void InstallBindings()
        {
            Container.Bind<LoadingScreenController>()
                .FromComponentInNewPrefab(_loadingScreenPrefab)
                .AsSingle()
                .NonLazy();

            Container.Bind<Bootstrapper>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.Bind<ResourceLoader>().AsSingle();
        }
    }
}
