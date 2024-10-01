using System;
using Core.Constants;
using Core.Networking.NetworkPlayersService;
using Core.Networking.Settings;
using Core.SaveSystem.Savable;
using Core.SaveSystem.System;
using Core.SceneManagement;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using VContainer;

namespace Gameplay.Server
{
    public class ServerPlayer : NetworkBehaviour, ISavable<ServerPlayer.Data>
    {
        [Serializable]
        public class Data
        {
            public string activeScene;

            public Data() { }
            public Data(string activeScene) => this.activeScene = activeScene;
        }

        public Data CurrentData => _data;
        
        private Data _data = new();
        private ResourcesService _resourcesService;
        private ISceneLoader _sceneLoader;
        private NetworkManager _networkManager;
        private ClientIdentification _clientIdentification;
        private INetworkPlayersService _playersService;
        private ISaveSystem _saveSystem;
        
        [Inject]
        private void Construct(
            ResourcesService resourcesService,
            ISceneLoader sceneLoader,
            NetworkManager networkManager,
            ClientIdentification clientIdentification,
            INetworkPlayersService playersService,
            ISaveSystem saveSystem)
        {
            _resourcesService = resourcesService;
            _sceneLoader = sceneLoader;
            _networkManager = networkManager;
            _clientIdentification = clientIdentification;
            _playersService = playersService;
            _saveSystem = saveSystem;
        }

        #region OnNetworkSpawn/Despawn callbacks
        public override void OnNetworkSpawn()
        {
            _playersService.RegisterServerPlayer(this);
            if(!IsLocalPlayer) return;
            
            _networkManager.OnClientConnectedCallback += OnClientConnected;
            _networkManager.OnClientDisconnectCallback += OnClientDisconnected;

            _clientIdentification.SetPlayerType(PlayerTypes.Host);
            
            _saveSystem.RegisterSavable(this);
            _saveSystem.LoadDataTo(this);
            
            LoadServerScene(_data.activeScene);
        }
        public override void OnNetworkDespawn()
        {
            _playersService.UnregisterServerPlayer(this);
            if(!IsLocalPlayer) return;

            _networkManager.OnClientConnectedCallback -= OnClientConnected;
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
            
            _saveSystem.SaveDataFrom(this);
            _saveSystem.UnregisterSavable(this);
            
            UnloadCurrentServerScene();
        }
        #endregion
        
        #region OnClientConnected/Disconnected callback
        private void OnClientConnected(ulong clientId)
        {
            if (_networkManager.ConnectedClients.Count == GamePreferences.PlayersCount)
            {
                OnAllClientsConnected();
            }   
        }
        private void OnClientDisconnected(ulong clientId)
        {
            OnAnyClientDisconnected();
        }
        #endregion
        
        #region OnAllClientsConnected/AnyClientDisconnected events
        private void OnAllClientsConnected()
        {
            
        }
        private void OnAnyClientDisconnected()
        {
            
        }
        #endregion

        #region Interactions

        public void LoadServerScene(string sceneName)
        {
            if (sceneName != _data.activeScene)
                UnloadCurrentServerScene();

            _data.activeScene = sceneName;
            _sceneLoader.TryLoadOfflineScene(_data.activeScene, LoadSceneMode.Additive);
        }

        public void UnloadCurrentServerScene()
        {
            _sceneLoader.TryUnloadOfflineScene(_data.activeScene);
        }

        #endregion
        
        #region Save system
        public string Key => nameof(ServerPlayer);
        public void ApplyData(Data data)
        {
            _data = data;
        }
        public Data GetData()
        {
            return _data;
        }
        public Data GetDefaultData()
        {
            return new Data(_resourcesService.SceneTransitions.HUBScene.Scene);
        }
        #endregion
    }
}
