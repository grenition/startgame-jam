using Cysharp.Threading.Tasks;

namespace Core.SceneManagement
{
    public interface ISceneLoader
    {
        public UniTask<bool> TryLoadOnlineScene(string sceneKey);
        public UniTask<bool> TryLoadOfflineScene(string sceneKey);
    }
}
