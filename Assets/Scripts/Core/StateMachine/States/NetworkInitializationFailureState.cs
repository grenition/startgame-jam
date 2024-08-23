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
        public const float ReinitializationDelay = 10f;
        protected async override UniTask OnEnter()
        {
            Debug.Log($"Reinitializing network services in {ReinitializationDelay}");
            await UniTask.WaitForSeconds(ReinitializationDelay);

            Debug.Log($"{GetType().Name} state completed!");
            IsCompleted = true;
        }
    }
}
