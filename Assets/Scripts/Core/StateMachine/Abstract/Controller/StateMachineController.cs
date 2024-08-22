using Core.StateMachine.Abstract.Controller;
using UnityHFSM;
using VContainer.Unity;

namespace Core.StateMachine.Controllers
{
    public abstract class StateMachineController<TStateId> : IStateMachineController<TStateId>, IInitializable
    {
        public StateMachine<TStateId> FSM => _fsm;

        protected StateMachine<TStateId> _fsm = new StateMachine<TStateId>();
        protected abstract TStateId startStateId { get; }

        public virtual void Initialize()
        {
            InitializeStates();
            
            _fsm.SetStartState(startStateId);
            _fsm.Init();
        }
        protected abstract void InitializeStates();
    }
}
