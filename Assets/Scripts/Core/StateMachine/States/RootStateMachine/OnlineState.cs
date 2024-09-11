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
        public const string NetworkScene = "NetworkScene";
        
        private NetworkManager _networkManager;
        private ISceneLoader _sceneLoader;
        private IObjectsFactory _networkObjectsFactory;
        
        [Inject]
        private void Construct(
            NetworkManager networkManager, 
            ISceneLoader sceneLoader,
            IObjectsFactory networkObjectsFactory)
        {
            _networkManager = networkManager;
            _sceneLoader = sceneLoader;
            _networkObjectsFactory = networkObjectsFactory;

        }
        
        protected async override UniTask OnEnter()
        {
            Debug.Log($"Entered state: {GetType().Name}");

            SceneManager.sceneLoaded += OnSceneLoad;
            _networkManager.OnClientStopped += CompleteState;
            _networkManager.SceneManager.OnLoadComplete += OnNetworkSceneLoad;
            _sceneLoader.TryLoadOnlineScene(NetworkScene);
        }
        protected async override UniTask OnExit()
        {
            SceneManager.sceneLoaded -= OnSceneLoad;
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
            
            if(scenename == NetworkScene)
                SpawnPlayer(clientid);
        }
        private async void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
        {
            await UniTask.NextFrame();
                
            if (scene.name != NetworkScene && !_networkManager.IsServer)
                SceneManager.UnloadSceneAsync(scene.buildIndex);
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
