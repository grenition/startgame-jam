using Core.StateMachine.Abstract.CreatableState;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Core.StateMachine.States
{
    public class OfflineState<TStateId> : State<TStateId>
    {
        private NetworkManager _networkManager;
        
        [Inject]
        private void Construct(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }
        
        protected async override UniTask OnEnter()
        {
            Debug.Log($"Entered state: {GetType().Name}");

            if (_networkManager.IsConnectedClient)
            {
                CompleteState();
                return;
            }

            _networkManager.OnClientStarted += CompleteState;
        }
        protected async override UniTask OnExit()
        {
            _networkManager.OnClientStarted -= CompleteState;
        }
        private void CompleteState()
        {
            Debug.Log($"{GetType().Name} state completed!");
            IsCompleted = true;
        }
    }
}
