using Core.Networking.NetworkObjectsFactory;
using Core.SaveSystem.System;
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
        public override string Name => "OnlineState";
        public const string NetworkScene = "NetworkScene";
        
        private NetworkManager _networkManager;
        private ISceneLoader _sceneLoader;
        private IObjectsFactory _networkObjectsFactory;
        private ISaveSystem _saveSystem;
        
        [Inject]
        private void Construct(
            NetworkManager networkManager, 
            ISceneLoader sceneLoader,
            IObjectsFactory networkObjectsFactory,
            ISaveSystem saveSystem)
        {
            _networkManager = networkManager;
            _sceneLoader = sceneLoader;
            _networkObjectsFactory = networkObjectsFactory;
            _saveSystem = saveSystem;
        }

        protected async override UniTask OnEnter()
        {
            SceneManager.sceneLoaded += OnSceneLoad;
            _networkManager.OnClientStopped += CompleteState;
            _networkManager.SceneManager.OnLoadComplete += OnNetworkSceneLoad;
            _sceneLoader.TryLoadOnlineScene(NetworkScene);
        }
        protected async override UniTask OnExit()
        {
            _saveSystem.SaveData();
            SceneManager.sceneLoaded -= OnSceneLoad;
            _networkManager.OnClientStopped -= CompleteState;
            _networkManager.SceneManager.OnLoadComplete -= OnNetworkSceneLoad;
        }
        private void CompleteState(bool hostStop) => CompleteState();
        private async void OnNetworkSceneLoad(ulong clientid, string scenename, LoadSceneMode loadscenemode)
        {
            await UniTask.NextFrame();

            if (scenename == NetworkScene)
            {
                if (clientid == NetworkManager.ServerClientId)
                    _saveSystem.LoadData();

                SpawnPlayer(clientid);
            }
        }
        private async void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
        {
            await UniTask.WaitForSeconds(1f);
            
            // if (scene.name != NetworkScene && !_networkManager.IsServer)
            //     SceneManager.UnloadSceneAsync(scene.buildIndex);
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
