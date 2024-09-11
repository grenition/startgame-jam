using System;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using VContainer;

namespace Core.SceneManagement
{
    public class SceneLoader : ISceneLoader
    {
        private NetworkManager _networkManager;

        [Inject]
        private void Construct(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        public async UniTask<bool> TryLoadOnlineScene(string sceneKey, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (!_networkManager.NetworkConfig.EnableSceneManagement)
            {
                return await TryLoadOfflineScene(sceneKey, loadSceneMode);
            }
            
            if (_networkManager.IsClient && !_networkManager.IsServer)
                return false;
            
            if (!_networkManager.IsClient && !_networkManager.IsServer)
                return false;

            if (_networkManager.IsServer)
            {
                _networkManager.SceneManager.LoadScene(sceneKey, loadSceneMode);
                return true;
            }
            
            return false;
        }
        public async UniTask<bool> TryLoadOfflineScene(string sceneKey, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(sceneKey, loadSceneMode);
            return true;
        }
        public async UniTask<bool> TryUnloadOfflineScene(string sceneKey)
        {
            SceneManager.UnloadSceneAsync(sceneKey);
            return true;
        }
    }
}
