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
        
        private IObjectsFactory _objectsFactory;
        private ClientIdentification _clientIdentification;
        private GameObject _controllerScene;
        private INetworkPlayersService _playersService;

        private NetworkVariable<PlayerTypes> _syncPlayerType = new(PlayerTypes.Big, 
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
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
            if(!IsLocalPlayer) return;

            _controllerScene = _objectsFactory.SpawnLocalObject(_controllerScenePrefab);
            SelectPlayerType(PlayerTypes.Big);
        }
        public override void OnNetworkDespawn()
        {
            _playersService.UnregisterNetworkPlayer(this);
            if(!IsLocalPlayer) return;

            if (_controllerScene != null) Destroy(_controllerScene);
        }

        private void SelectPlayerType(PlayerTypes playerType)
        {
            _clientIdentification.SetPlayerType(playerType);
            _syncPlayerType.Value = playerType;
        }
    }
}
