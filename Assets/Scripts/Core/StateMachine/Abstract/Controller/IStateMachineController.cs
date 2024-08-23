using Core.StateMachine.Abstract.CreatableState;
using UnityHFSM;

namespace Core.StateMachine.Abstract.Controller
{
    public interface IStateMachineController<TStateId>
    {
        public UnityHFSM.StateMachine<TStateId> FSM { get; }
        public void ResolveState<T>(IState<T> state);
    }
}
