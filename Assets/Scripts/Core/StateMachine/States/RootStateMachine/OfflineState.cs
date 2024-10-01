using Core.SaveSystem.System;
using Core.SceneManagement;
using Core.StateMachine.Abstract.CreatableState;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Core.StateMachine.States
{
    public class OfflineState<TStateId> : State<TStateId>
    {
        public override string Name => "OfflineState";
        
        private NetworkManager _networkManager;
        private ISceneLoader _sceneLoader;
        private ISaveSystem _saveSystem;
        
        [Inject]
        private void Construct(
            NetworkManager networkManager, 
            ISceneLoader sceneLoader,
            ISaveSystem saveSystem)
        {
            _networkManager = networkManager;
            _sceneLoader = sceneLoader;
            _saveSystem = saveSystem;
        }

        protected async override UniTask OnEnter()
        {
            if (_networkManager.IsConnectedClient)
            {
                CompleteState();
                return;
            }

            _saveSystem.ResetData();
            _sceneLoader.TryLoadOfflineScene("OfflneScene", LoadSceneMode.Single);

            CompleteState();
        }
    }
}
