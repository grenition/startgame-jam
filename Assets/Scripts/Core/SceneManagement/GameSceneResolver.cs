using Core.LifetimeScopes;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.SceneManagement
{
    public class GameSceneResolver : IInitializable
    {
        private NetworkManager _networkManager;
        private GameObject _gameObject;
        
        [Inject]
        private void Construct(
            NetworkManager networkManager,
            GameLifetimeScope lifetimeScope)
        {
            _networkManager = networkManager;
            _gameObject = lifetimeScope.gameObject;
        }
        
        
        public async void Initialize()
        {
            if(_networkManager.IsServer) return;
            
            var scene = _gameObject.scene;
            var rootObjects = scene.GetRootGameObjects();

            foreach (var rootObject in rootObjects)
            {
                if(rootObject.TryGetComponent<NetworkObject>(out _))
                    continue;
                GameObject.Destroy(rootObject);
            }
        }
    }
}
