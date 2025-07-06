using Code.Bootstrap;
using Code.Error;
using UnityEngine;
using Zenject;

namespace Code.Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private Bootstrapper _bootstrapper;
        [SerializeField] private ErrorView _errorView;

        public override void InstallBindings()
        {
            Container.Bind<ErrorView>().FromInstance(_errorView).AsSingle();
            Container.Bind<ErrorControl>().AsSingle().NonLazy();
            Container.Bind<Bootstrapper>().FromInstance(_bootstrapper).AsSingle().NonLazy();
        }
    }
}