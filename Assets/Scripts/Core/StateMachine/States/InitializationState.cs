using Core.StateMachine.Abstract.CreatableState;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.StateMachine.States
{
    public class InitializationState<TStateId> : State<TStateId>
    {
        protected async override UniTask OnEnter()
        {
            Debug.Log($"{GetType().Name} state completed!");
            IsCompleted = true;
        }
    }
}
