using System;
using Core.StateMachine.Abstract.Controller;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.StateMachine.Abstract.CreatableState
{
    public abstract class State<TStateId> : IState<TStateId>
    {
        public abstract string Name { get; }
        
        private bool _isCompleted = false;
        private bool _isFailed = false;
        private IStateMachineController<TStateId> _stateMachineController;
        private TStateId _thisStateId;
        private bool _onCompleteTransitionActive;
        private TStateId _onCompleteStateId;
        private bool _onFailureTransitionActive;
        private TStateId _onFailureStateId;
        
        protected async virtual UniTask OnEnter() { }
        protected async virtual UniTask OnLogic() { }
        protected async virtual UniTask OnExit() { }
        
        public bool IsCompleted {
            get => _isCompleted;
            protected set {
                if(value == _isCompleted) return;

                _isCompleted = value;
                if(_stateMachineController == null) return;
                
                if(_onCompleteTransitionActive)
                    _stateMachineController.FSM.RequestStateChange(_onCompleteStateId);
                _stateMachineController.FSM.OnLogic();
            }
        }
        public bool IsFailed {
            get => _isFailed;
            protected set {
                if(value == _isFailed) return;

                _isFailed = value;
                if(_stateMachineController == null) return;
                
                if(_onFailureTransitionActive)
                    _stateMachineController.FSM.RequestStateChange(_onFailureStateId);
                _stateMachineController.FSM.OnLogic();
            }
        }
        public UnityHFSM.State<TStateId> CreateHFSMState()
        {
            async void SafeExecution(UniTask action)
            {
                try
                {
                    await action;
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    IsFailed = true;
                }
            }
            
            return new UnityHFSM.State<TStateId>(
                onEnter: _ =>
                {
                    _isCompleted = false;
                    _isFailed = false;
                    
                    Debug.Log($"Entered state: {Name}");
                    SafeExecution(OnEnter());
                },
                onLogic: _ => SafeExecution(OnLogic()),
                onExit: _ => SafeExecution(OnExit())
            );
        }
        public IState<TStateId> ConnectToSoStateMachine(IStateMachineController<TStateId> smc, TStateId stateId)
        {
            if(_stateMachineController != null || smc == null) return this;

            _stateMachineController = smc;
            _stateMachineController.FSM.AddState(stateId, CreateHFSMState());
            _stateMachineController.ResolveState(this);
            
            return this;
        }
        public IState<TStateId> WithTransitionOnCompleteTo(TStateId stateId)
        {
            if (_stateMachineController == null || stateId == null) return this;

            _onCompleteTransitionActive = true;
            _onCompleteStateId = stateId;
            
            return this;
        }
        public IState<TStateId> WithTransitionOnFailureTo(TStateId stateId)
        {
            if (_stateMachineController == null || stateId == null) return this;

            _onFailureTransitionActive = true;
            _onFailureStateId = stateId;
            
            return this;
        }
        
        protected virtual void CompleteState()
        {
            Debug.Log($"{Name} state completed!");
            IsCompleted = true;
        }
    }
}
