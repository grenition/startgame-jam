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
        protected async override UniTask OnEnter()
        {
            Debug.Log("Reinitializing services");
            await UniTask.WaitForSeconds(5f);

            Debug.Log($"{GetType().Name} state completed!");
            IsCompleted = true;
        }
    }
}
