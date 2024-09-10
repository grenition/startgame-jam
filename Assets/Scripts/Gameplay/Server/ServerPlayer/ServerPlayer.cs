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
        
        [Inject]
        private void Construct(
            ResourcesService resourcesService,
            ISceneLoader sceneLoader)
        {
            _resourcesService = resourcesService;
            _sceneLoader = sceneLoader;
        }
        
        public override void OnNetworkSpawn()
        {
            if(!IsLocalPlayer) return;
            
            Debug.Log("Loading scene");
            _sceneLoader.TryLoadOfflineScene(
                _resourcesService.SceneTransitions.HUBScene.Scene, LoadSceneMode.Additive);
        }
        public override void OnNetworkDespawn()
        {
            if(!IsLocalPlayer) return;

            _sceneLoader.TryUnloadOfflineScene(
                _resourcesService.SceneTransitions.HUBScene.Scene);
        }
    }
}
