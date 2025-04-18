using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

namespace Core.SceneManagement
{
    public interface ISceneLoader
    {
        public UniTask<bool> TryLoadOnlineScene(string sceneKey, LoadSceneMode loadSceneMode = LoadSceneMode.Single);
        public UniTask<bool> TryUnloadOnlineScene(string sceneKey, LoadSceneMode loadSceneMode = LoadSceneMode.Single);
        public UniTask<bool> TryLoadOfflineScene(string sceneKey, LoadSceneMode loadSceneMode = LoadSceneMode.Single);
        public UniTask<bool> TryUnloadOfflineScene(string sceneKey);
    }
}
