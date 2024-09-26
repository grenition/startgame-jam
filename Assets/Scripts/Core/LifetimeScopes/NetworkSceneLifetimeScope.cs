using Core.Interactions;
using Core.Networking.NetworkObjectsFactory;
using Core.Networking.NetworkPlayersService;
using Core.StateMachine.Controllers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.LifetimeScopes
{
    public class NetworkSceneLifetimeScope : LifetimeScope
    {
        public static IObjectResolver Resolver => instance != null ? instance.Container : null;
        private static NetworkSceneLifetimeScope instance = null;
        
        [SerializeField] private NetworkGameStateMachine _stateMachine;
        [SerializeField] private ControllerNetworkBus _controllerBus;
        [SerializeField] private NetworkInteractionsService _interactionsService;

        private ClientIdentification _clientIdentification;
        
        protected override void Configure(IContainerBuilder builder)
        {
            instance ??= this;
            
            builder.RegisterInstance(_stateMachine);
            autoInjectGameObjects.Add(_stateMachine.gameObject);
            
            builder.Register<NetworkPlayersService>(Lifetime.Singleton).As<INetworkPlayersService>();
            
            builder.RegisterInstance(_controllerBus);
            autoInjectGameObjects.Add(_controllerBus.gameObject);
            
            _clientIdentification = new();
            builder.RegisterInstance(_clientIdentification);
            
            builder.RegisterEntryPoint<FactoryContainerOverrider>();
            
            builder.RegisterInstance(_interactionsService).As<IInteractionService>();
            autoInjectGameObjects.Add(_interactionsService.gameObject);
        }
    }
}
