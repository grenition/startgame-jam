using Core.Networking.NetworkPlayersService;
using System.Numerics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Gameplay.Player.GameObject
{
    public class GamePlayersResolver : IInitializable
    {
        private INetworkPlayersService _playersService;
        private NetworkManager _networkManager;
        [Inject]
        public GamePlayersResolver(
            INetworkPlayersService playersService,
            NetworkManager networkManager)
        {
            _playersService = playersService;
            _networkManager = networkManager;
        }
        
        public void Initialize()
        {
            if(!_networkManager.IsServer) return;
            var serverScene = _playersService.ServerPlayer.CurrentData.activeScene;
            foreach (var networkPlayer in _playersService.NetworkPlayers)
            {
                if(networkPlayer.PlayerObject == null) continue;
                SceneManager.MoveGameObjectToScene(networkPlayer.PlayerObject.gameObject, SceneManager.GetSceneByName(serverScene));
                networkPlayer.PlayerObject.transform.position = UnityEngine.Vector3.zero;
            }
        }
    }
}
