using Core.StateMachine.Abstract.CreatableState;
using VContainer;

namespace Core.StateMachine.States
{
    public class InitializationState<TStateId> : State<TStateId>
    {
        private UnityHFSM.StateMachine _stateMachine;
        
        [Inject]
        private void Construct(UnityHFSM.StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        protected override void OnEnter()
        {
            IsCompleted = true;
        }
    }
}
