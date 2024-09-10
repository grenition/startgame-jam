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
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_stateMachine);
            builder.RegisterInstance(_controllerBus);
        }
    }
}
