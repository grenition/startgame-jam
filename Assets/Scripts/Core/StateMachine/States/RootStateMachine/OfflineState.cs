using Core.SceneManagement;
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
        private ISceneLoader _sceneLoader;
        
        [Inject]
        private void Construct(NetworkManager networkManager, ISceneLoader sceneLoader)
        {
            _networkManager = networkManager;
            _sceneLoader = sceneLoader;
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
            _sceneLoader.TryLoadOfflineScene("OfflneScene");
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
