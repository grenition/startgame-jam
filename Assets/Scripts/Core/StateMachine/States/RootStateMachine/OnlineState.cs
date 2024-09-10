using Core.Networking.NetworkObjectsFactory;
using Core.SceneManagement;
using Core.StateMachine.Abstract.CreatableState;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Core.StateMachine.States
{
    public class OnlineState<TStateId> : State<TStateId>
    {
        private NetworkManager _networkManager;
        private ISceneLoader _sceneLoader;
        private INetworkObjectsFactory _networkObjectsFactory;
        
        [Inject]
        private void Construct(
            NetworkManager networkManager, 
            ISceneLoader sceneLoader,
            INetworkObjectsFactory networkObjectsFactory)
        {
            _networkManager = networkManager;
            _sceneLoader = sceneLoader;
            _networkObjectsFactory = networkObjectsFactory;
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
            _networkManager.SceneManager.OnLoadComplete += OnNetworkSceneLoad;
            _sceneLoader.TryLoadOnlineScene("NetworkScene");
        }
        protected async override UniTask OnExit()
        {
            _networkManager.OnClientStopped -= CompleteState;
            _networkManager.SceneManager.OnLoadComplete -= OnNetworkSceneLoad;
        }
        private void CompleteState(bool hostStop)
        {
            Debug.Log($"{GetType().Name} state completed!");
            IsCompleted = true;
        }
        private async void OnNetworkSceneLoad(ulong clientid, string scenename, LoadSceneMode loadscenemode)
        {
            await UniTask.NextFrame();
            
            if(scenename == "NetworkScene")
                SpawnPlayer(clientid);
        }
        private void SpawnPlayer(ulong clientId)
        {
            if (clientId == NetworkManager.ServerClientId)
            {
                _networkObjectsFactory.SpawnServerPlayer();
                return;
            }
            
            _networkObjectsFactory.SpawnNetworkPlayer(clientId); 
        }
    }
}
