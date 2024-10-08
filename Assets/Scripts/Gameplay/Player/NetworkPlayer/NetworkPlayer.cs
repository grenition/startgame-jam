using Core.Networking.NetworkObjectsFactory;
using Core.Networking.NetworkPlayersService;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Gameplay.Player
{
    public class NetworkPlayer : NetworkBehaviour
    {
        public PlayerObject PlayerObject => _player;
        public PlayerTypes PlayerType => default;

        [SerializeField] private UnityEngine.GameObject _controllerScenePrefab;
        [SerializeField] private PlayerObject _playerPrefab;

        private IObjectsFactory _objectsFactory;
        private ClientIdentification _clientIdentification;
        private UnityEngine.GameObject _controllerScene;
        private INetworkPlayersService _playersService;
        
        private PlayerObject _player;

        [Inject]
        private void Construct(
            IObjectsFactory objectsFactory,
            ClientIdentification clientIdentification,
            INetworkPlayersService playersService)
        {
            _objectsFactory = objectsFactory;
            _clientIdentification = clientIdentification;
            _playersService = playersService;
        }

        public override void OnNetworkSpawn()
        {
            //MoveToNetworkScene();
            
            _playersService.RegisterNetworkPlayer(this);
            var type = _playersService.NetworkPlayers.Count == 1 ? PlayerTypes.Big : PlayerTypes.Small;
            if (IsServer)
            {
                _player = _objectsFactory.SpawnLocalObject(_playerPrefab, TargetScene.GameScene);
                _player.transform.position += Vector3.up * .5f;
                _player.SetPlayerType(type);
                SetPlayerTypeClientRpc((int)type);
            }
        }

        [ClientRpc]
        private void SetPlayerTypeClientRpc(int type)
        {
            if (!IsLocalPlayer) return;

            _clientIdentification.SetPlayerType((PlayerTypes)type);
            _controllerScene = _objectsFactory.SpawnLocalObject(_controllerScenePrefab, TargetScene.NetworkScene, false);
        }

        public override void OnNetworkDespawn()
        {
            _playersService.UnregisterNetworkPlayer(this);
            if (IsServer && _player != null)
            {
                Destroy(_player.gameObject);
            }

            if (!IsLocalPlayer) return;

            if (_controllerScene != null) Destroy(_controllerScene);
        }
        
        private async void MoveToNetworkScene()
        {
            await UniTask.WaitForSeconds(0.2f);
            if (gameObject.scene != SceneManager.GetSceneByName(ObjectsFactory.NetworkScene))
                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName(ObjectsFactory.NetworkScene));
        }
    }
}
