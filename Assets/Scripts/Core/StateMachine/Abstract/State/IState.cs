using System;
using Core.StateMachine.Abstract.Controller;
using UnityHFSM;

namespace Core.StateMachine.Abstract.CreatableState
{
    public interface IState<TStateId>
    {
        public UnityHFSM.State<TStateId> CreateHFSMState();
        public bool IsCompleted { get; }
        public bool IsFailed { get; }
        public IState<TStateId> ConnectToSoStateMachine(IStateMachineController<TStateId> smc, TStateId stateId);
        public IState<TStateId> WithTransitionOnCompleteTo(TStateId stateId);
        public IState<TStateId> WithTransitionOnFailureTo(TStateId stateId);
    }
}
