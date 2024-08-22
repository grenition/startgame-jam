using Core.StateMachine.States;

namespace Core.StateMachine.Controllers
{
    public enum RootStates
    {
        Initialization,
        NetworkInitialization,
        NetworkInitializationFailure,
        Offline,
        Online
    }
    
    public class RootStateMachine : StateMachineController<RootStates>
    {
        protected override RootStates startStateId => RootStates.Initialization;
        protected override void InitializeStates()
        {
            var initializationState = new InitializationState<RootStates>()
                .ConnectToSoStateMachine(this, RootStates.Initialization)
                .WithTransitionOnCompleteTo(RootStates.NetworkInitialization);

            var networkInitializationState = new NetworkInitializationState<RootStates>()
                .ConnectToSoStateMachine(this, RootStates.NetworkInitialization)
                .WithTransitionOnCompleteTo(RootStates.Initialization)
                .WithTransitionOnFailureTo(RootStates.NetworkInitializationFailure);

            var networkInitializationFailureState = new NetworkInitializationFailureState<RootStates>()
                .ConnectToSoStateMachine(this, RootStates.NetworkInitializationFailure)
                .WithTransitionOnCompleteTo(RootStates.NetworkInitialization);
        }
    }
}
