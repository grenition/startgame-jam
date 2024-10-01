using System;
using Core.StateMachine.Abstract.CreatableState;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Core.StateMachine.States
{
    public class NetworkInitializationFailureState<TStateId> : State<TStateId>
    {
        public override string Name => "NetworkInitializationFailureState";
        public const float ReinitializationDelay = 10f;
        protected async override UniTask OnEnter()
        {
            Debug.Log($"Reinitializing network services in {ReinitializationDelay} seconds");
            await UniTask.WaitForSeconds(ReinitializationDelay);

            CompleteState();
        }
    }
}
