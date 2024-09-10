using Core.StateMachine.Abstract.Controller;
using Core.StateMachine.Abstract.CreatableState;
using Unity.Netcode;
using UnityHFSM;
using VContainer;

namespace Core.StateMachine.Controllers
{
    public enum NetworkGameState
    {
        AwaitingPlayers,
        Game
    }
    
    public class NetworkGameStateMachine : NetworkBehaviour, IStateMachineController<NetworkGameState>
    {
        public StateMachine<NetworkGameState> FSM { get; private set; } = new();

        private IObjectResolver _objectResolver;
        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }
        
        private void Awake()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            //FSM.SetStartState(NetworkGameState.AwaitingPlayers);
            //FSM.Init();
        }
        
        public void ResolveState<T>(IState<T> state)
        {
            if(state == null) return;
            
            _objectResolver.Inject(state);
        }
    }
}
