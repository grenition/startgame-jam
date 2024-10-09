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
        [SerializeField] private NetworkGameStateMachine _stateMachine;
        [SerializeField] private ControllerNetworkBus _controllerBus;
        [SerializeField] private NetworkInteractionsService _interactionsService;
        [SerializeField] private AudioPool _audioPool;
        [SerializeField] private ComicsViewer _comicsViewer;

        private ClientIdentification _clientIdentification;
        private CompletedTasks _tasks;

        protected override void Awake()
        {
            Debug.Log($"Initialiazed {nameof(NetworkSceneLifetimeScope)}");
            base.Awake();
        }
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"Start building {nameof(NetworkSceneLifetimeScope)}");
            
            builder.RegisterInstance(_stateMachine);
            autoInjectGameObjects.Add(_stateMachine.gameObject);
            
            builder.Register<NetworkPlayersService>(Lifetime.Singleton).As<INetworkPlayersService>();
            builder.RegisterInstance(_audioPool);
            
            builder.RegisterInstance(_controllerBus);
            autoInjectGameObjects.Add(_controllerBus.gameObject);
            
            _clientIdentification = new();
            builder.RegisterInstance(_clientIdentification);
            _tasks = new();
            builder.RegisterInstance(_tasks);
            
            builder.RegisterEntryPoint<FactoryContainerOverrider>().AsSelf();
            
            builder.RegisterInstance(_interactionsService).As<IInteractionService>();
            autoInjectGameObjects.Add(_interactionsService.gameObject);

            builder.RegisterInstance(_comicsViewer);
            autoInjectGameObjects.Add(_comicsViewer.gameObject);

            Debug.Log($"{nameof(NetworkSceneLifetimeScope)} builded!");
        }
    }
}
