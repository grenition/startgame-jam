using System;
using Core.Networking.Settings;
using Core.SceneManagement;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Gameplay.Server
{
    public class ServerPlayer : NetworkBehaviour
    {
        private ResourcesService _resourcesService;
        private ISceneLoader _sceneLoader;
        private NetworkManager _networkManager;
        
        [Inject]
        private void Construct(
            ResourcesService resourcesService,
            ISceneLoader sceneLoader,
            NetworkManager networkManager)
        {
            _resourcesService = resourcesService;
            _sceneLoader = sceneLoader;
            _networkManager = networkManager;
        }
        
        #region OnNetworkSpawn/Despawn callbacks
        public override void OnNetworkSpawn()
        {
            if(!IsLocalPlayer) return;
            
            _networkManager.OnClientConnectedCallback += OnClientConnected;
            _networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        }
        public override void OnNetworkDespawn()
        {
            if(!IsLocalPlayer) return;

            _networkManager.OnClientConnectedCallback -= OnClientConnected;
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        }
        #endregion
        
        #region OnClientConnected/Disconnected callback
        private void OnClientConnected(ulong clientId)
        {
            if (_networkManager.ConnectedClients.Count == 2)
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
            _sceneLoader.TryLoadOfflineScene(
                _resourcesService.SceneTransitions.HUBScene.Scene, LoadSceneMode.Additive);
        }
        private void OnAnyClientDisconnected()
        {
            _sceneLoader.TryUnloadOfflineScene(
                _resourcesService.SceneTransitions.HUBScene.Scene);
        }
        #endregion
    }
}
