using Core.StateMachine.Abstract.Controller;
using Unity.VisualScripting;

namespace Core.StateMachine.Controllers
{
    public abstract class StateMachineController<TStateId> : IStateMachineController<TStateId>, IInitializable
    {
        public UnityHFSM.StateMachine<TStateId> FSM => _fsm;

        protected UnityHFSM.StateMachine<TStateId> _fsm = new UnityHFSM.StateMachine<TStateId>();
        protected abstract TStateId startStateId { get; }

        public virtual void Initialize()
        {
            InitializeStates();
            
            _fsm.SetStartState(startStateId);
            _fsm.Init();
        }
        protected abstract void InitializeStates();
        public virtual void Update() => _fsm.OnLogic();
    }
}
