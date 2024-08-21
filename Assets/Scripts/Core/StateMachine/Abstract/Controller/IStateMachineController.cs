using UnityHFSM;

namespace Core.StateMachine.Abstract.Controller
{
    public interface IStateMachineController<TStateId>
    {
        public UnityHFSM.StateMachine<TStateId> FSM { get; }
        public void Update();
    }
}
