using Core.StateMachine.Abstract.CreatableState;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.StateMachine.States
{
    public class InitializationState<TStateId> : State<TStateId>
    {
        public override string Name => "InitializationState";
        protected async override UniTask OnEnter()
        {
            CompleteState();
        }
    }
}
