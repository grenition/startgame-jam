using Core.Networking.NetworkObjectsService;
using Core.Networking.RelayService;
using Core.SaveSystem.System;
using Core.SceneManagement;
using Core.StateMachine.Controllers;
using Gameplay.Player.Service;
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
            InstallCoreSystems(builder);
            InstallPlayerServices(builder);
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
        private void InstallCoreSystems(IContainerBuilder builder)
        {
            builder.RegisterInstance(DevConsole.singleton);
            builder.RegisterEntryPoint<SceneLoader>().As<ISceneLoader>();
            builder.Register<SaveSystem.System.SaveSystem>(Lifetime.Singleton).As<ISaveSystem>();
            builder.Register<NetworkObjectsService>(Lifetime.Singleton).As<INetworkObjectsService>();
        }
        private void InstallPlayerServices(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<PlayerService>().As<IPlayerService>();
        }
    }
}
