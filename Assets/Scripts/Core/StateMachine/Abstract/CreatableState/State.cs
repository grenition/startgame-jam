using Core.StateMachine.Abstract.Controller;

namespace Core.StateMachine.Abstract.CreatableState
{
    public abstract class State<TStateId> : IState<TStateId>
    {
        protected virtual void OnEnter() { }
        protected virtual void OnLogic() { }
        protected virtual void OnExit() { }

        public UnityHFSM.State<TStateId> CreateHFSMState()
        {
            return new UnityHFSM.State<TStateId>(
                onEnter: _ => OnEnter(),
                onLogic: _ => OnLogic(),
                onExit: _ => OnExit()
            );
        }
        public bool IsCompleted {
            get => _isCompleted;
            protected set {
                if(value == _isCompleted) return;

                _isCompleted = value;
                _stateMachineController?.Update();
            }
        }
        public IState<TStateId> ConnectToSoStateMachine(IStateMachineController<TStateId> smc, TStateId stateId)
        {
            if(_stateMachineController != null || smc == null) return this;

            _stateMachineController = smc;
            _stateMachineController.FSM.AddState(stateId, CreateHFSMState());
            
            return this;
        }
        public IState<TStateId> WithTransitionOnCompleteTo(TStateId stateId)
        {
            if (_stateMachineController == null) return this;

            _stateMachineController.FSM.AddTransition(
                new UnityHFSM.Transition<TStateId>(_thisStateId, stateId, _ => IsCompleted));
            
            return this;
        }

        private bool _isCompleted = false;
        private IStateMachineController<TStateId> _stateMachineController;
        private TStateId _thisStateId;
    }
}
