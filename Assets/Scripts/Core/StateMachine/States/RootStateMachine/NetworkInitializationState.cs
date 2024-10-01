using System;
using Core.StateMachine.Abstract.CreatableState;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Core.StateMachine.States
{
    public class NetworkInitializationState<TStateId> : State<TStateId>
    {
        public override string Name => "NetworkInitializationState";
        protected async override UniTask OnEnter()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            CompleteState();
        }
    }
}
