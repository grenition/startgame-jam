using Core.Networking.Authentificator;
using Core.StateMachine.Controllers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.LifetimeScopes
{
    public class NetworkSceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private NetworkGameStateMachine _stateMachine;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_stateMachine);
        }
    }
}
