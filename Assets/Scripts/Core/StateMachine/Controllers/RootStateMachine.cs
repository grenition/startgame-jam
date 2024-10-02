using Core.StateMachine.States;

namespace Core.StateMachine.Controllers
{
    public enum RootStates
    {
        Initialization,
        NetworkInitialization,
        NetworkInitializationFailure,
        Offline,
        OfflineConnectingToRelay,
        OfflineConnectingFailure,
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
                .WithTransitionOnFailureTo(RootStates.NetworkInitializationFailure)
                .WithTransitionOnCompleteTo(RootStates.Offline);
            
            var networkInitializationFailureState = new NetworkInitializationFailureState<RootStates>()
                .ConnectToSoStateMachine(this, RootStates.NetworkInitializationFailure)
                .WithTransitionOnCompleteTo(RootStates.NetworkInitialization);

            var offlineState = new OfflineState<RootStates>()
                .ConnectToSoStateMachine(this, RootStates.Offline)
                .WithTransitionOnCompleteTo(RootStates.OfflineConnectingToRelay);

            var offlineConnectingToRelayState = new OfflineConnectingToRelayState<RootStates>()
                .ConnectToSoStateMachine(this, RootStates.OfflineConnectingToRelay)
                .WithTransitionOnCompleteTo(RootStates.Online)
                .WithTransitionOnFailureTo(RootStates.OfflineConnectingFailure);

            var offlineConnectingFailureState = new OfflineConnectingFailure<RootStates>()
                .ConnectToSoStateMachine(this, RootStates.OfflineConnectingFailure)
                .WithTransitionOnCompleteTo(RootStates.OfflineConnectingToRelay);

            var onlineState = new OnlineState<RootStates>()
                .ConnectToSoStateMachine(this, RootStates.Online)
                .WithTransitionOnCompleteTo(RootStates.Offline);
        }
    }
}
