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
        
        public async UniTask<bool> TryLoadOnlineScene(string sceneKey)
        {
            if (_networkManager.IsClient && !_networkManager.IsServer)
                return false;
            
            if (!_networkManager.IsClient && !_networkManager.IsServer)
                return false;

            if (_networkManager.IsServer)
            {
                _networkManager.SceneManager.LoadScene(sceneKey, LoadSceneMode.Single);
                return true;
            }

            return false;
        }
        public async UniTask<bool> TryLoadOfflineScene(string sceneKey)
        {
            SceneManager.LoadScene(sceneKey);
            return true;
        }
    }
}
