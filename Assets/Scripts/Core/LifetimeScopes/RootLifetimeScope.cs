using Core.Networking.RelayService;
using Core.SceneManagement;
using Core.StateMachine.Controllers;
using SickDev.DevConsole;
using Unity.Netcode;
using VContainer;
using VContainer.Unity;

namespace Core.LifetimeScopes
{
    public sealed class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            InstallStateMachines(builder);
            InstallNetworking(builder);
            InstallTools(builder);
        }
        
        private void InstallStateMachines(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<RootStateMachine>();
        }
        private void InstallNetworking(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<RelayController>().As<IRelayController>();
            
            GetComponent<NetworkManager>().SetSingleton();
            builder.RegisterInstance(NetworkManager.Singleton);
        }
        private void InstallTools(IContainerBuilder builder)
        {
            builder.RegisterInstance(DevConsole.singleton);
            builder.RegisterEntryPoint<SceneLoader>().As<ISceneLoader>();
        }
    }
}
