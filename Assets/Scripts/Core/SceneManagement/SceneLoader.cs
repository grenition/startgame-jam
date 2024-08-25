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
        
        public bool TryLoadOnlineScene(string sceneKey)
        {
            if (_networkManager.IsClient)
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
        public bool TryLoadOfflineScene(string sceneKey)
        {
            SceneManager.LoadScene(sceneKey);
            return true;
        }
    }
}
