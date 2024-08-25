using Core.SceneManagement;
using Core.StateMachine.Abstract.CreatableState;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Core.StateMachine.States
{
    public class OnlineState<TStateId> : State<TStateId>
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

            if (!_networkManager.IsConnectedClient)
            {
                CompleteState(false);
                return;
            }

            _networkManager.OnClientStopped += CompleteState;
            _sceneLoader.TryLoadOnlineScene("OnlineScene_HUB");
        }
        protected async override UniTask OnExit()
        {
            _networkManager.OnClientStopped -= CompleteState;
        }
        private void CompleteState(bool hostStop)
        {
            Debug.Log($"{GetType().Name} state completed!");
            IsCompleted = true;
        }
    }
}
