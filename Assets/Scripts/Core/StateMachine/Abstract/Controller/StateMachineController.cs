using Core.StateMachine.Abstract.Controller;
using Core.StateMachine.Abstract.CreatableState;
using UnityHFSM;
using VContainer;
using VContainer.Unity;

namespace Core.StateMachine.Controllers
{
    public abstract class StateMachineController<TStateId> : IStateMachineController<TStateId>, IInitializable
    {
        public StateMachine<TStateId> FSM => _fsm;

        protected StateMachine<TStateId> _fsm = new StateMachine<TStateId>();
        protected abstract TStateId startStateId { get; }
        protected IObjectResolver _objectResolver;

        [Inject]
        protected void Construct(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }
        
        public virtual void Initialize()
        {
            InitializeStates();
            
            _fsm.SetStartState(startStateId);
            _fsm.Init();
        }
        protected abstract void InitializeStates();
        public void ResolveState<T>(IState<T> state)
        {
            if(state == null) return;
            
            _objectResolver.Inject(state);
        }
    }
}
