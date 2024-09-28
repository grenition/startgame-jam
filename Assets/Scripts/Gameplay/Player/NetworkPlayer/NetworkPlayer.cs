using Core.Networking.NetworkObjectsFactory;
using Core.Networking.NetworkPlayersService;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Gameplay.Player
{
    public class NetworkPlayer : NetworkBehaviour
    {
        public PlayerTypes PlayerType => _syncPlayerType.Value;

        [SerializeField] private GameObject _controllerScenePrefab;
        [SerializeField] private PlayerObject _playerPrefab;

        private IObjectsFactory _objectsFactory;
        private ClientIdentification _clientIdentification;
        private GameObject _controllerScene;
        private INetworkPlayersService _playersService;

        private NetworkVariable<PlayerTypes> _syncPlayerType = new(PlayerTypes.Big,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
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
            _playersService.RegisterNetworkPlayer(this);
            if (IsServer)
            {
                _player = _objectsFactory.SpawnLocalObject(_playerPrefab, TargetScene.GameScene);
                _player.transform.position += Vector3.up * .5f;
                _player.SetPlayerType(PlayerTypes.Big);
            }

            if (!IsLocalPlayer) return;

            _controllerScene = _objectsFactory.SpawnLocalObject(_controllerScenePrefab, TargetScene.NetworkScene, false);
            SelectPlayerType(PlayerTypes.Big);
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

        private void SelectPlayerType(PlayerTypes playerType)
        {
            _clientIdentification.SetPlayerType(playerType);
            _syncPlayerType.Value = playerType;
        }
    }
}
