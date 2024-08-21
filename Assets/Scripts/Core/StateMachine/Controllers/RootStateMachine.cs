using Core.StateMachine.States;

namespace Core.StateMachine.Controllers
{
    public enum RootStates
    {
        Initialization,
        Starting
    }
    
    public class RootStateMachine : StateMachineController<RootStates>
    {
        protected override RootStates startStateId => RootStates.Initialization;
        protected override void InitializeStates()
        {
            var initializationState = new InitializationState<RootStates>()
                .ConnectToSoStateMachine(this, RootStates.Initialization)
                .WithTransitionOnCompleteTo(RootStates.Starting);
        }
    }
}
