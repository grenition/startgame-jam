namespace Core.SceneManagement
{
    public interface ISceneLoader
    {
        public bool TryLoadOnlineScene(string sceneKey);
        public bool TryLoadOfflineScene(string sceneKey);
    }
}
