using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Core.SceneManagement
{
    public class SceneLoader : ISceneLoader
    {
        private NetworkManager _networkManager;
        private Dictionary<string, AsyncOperation> _loadingScenes = new();

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
        public async UniTask<bool> TryUnloadOnlineScene(string sceneKey, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
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
                _networkManager.SceneManager.UnloadScene(SceneManager.GetSceneByName(sceneKey));
                return true;
            }
            
            return false;
        }
        // ReSharper disable Unity.PerformanceAnalysis
        public async UniTask<bool> TryLoadOfflineScene(string sceneKey, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (_loadingScenes.ContainsKey(sceneKey)) return false;
            
            var asyncOperation = SceneManager.LoadSceneAsync(sceneKey, loadSceneMode);
            _loadingScenes.Add(sceneKey, asyncOperation);

            await asyncOperation.ToUniTask();

            _loadingScenes.Remove(sceneKey);
            return true;
        }
        public async UniTask<bool> TryUnloadOfflineScene(string sceneKey)
        {
            SceneManager.UnloadSceneAsync(sceneKey);
            return true;
        }
    }
}
