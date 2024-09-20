using Core.Networking.NetworkObjectsFactory;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Gameplay.Player
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [SerializeField] private GameObject _controllerScenePrefab;
        [SerializeField] private PlayerObject _playerPrefab;
        
        private IObjectsFactory _objectsFactory;
        private ClientIdentification _clientIdentification;
        private GameObject _controllerScene;
        private PlayerObject _player;
        
        [Inject]
        private void Construct(
            IObjectsFactory objectsFactory,
            ClientIdentification clientIdentification)
        {
            _objectsFactory = objectsFactory;
            _clientIdentification = clientIdentification;
        }

        public override void OnNetworkSpawn()
        {
            if(IsServer)
            {
                _player = _objectsFactory.SpawnLocalObject(_playerPrefab);
                _player.transform.position += Vector3.up * .5f;
                _player.SetPlayerType(PlayerTypes.Big);
            }

            if(!IsLocalPlayer) return;
            
            _controllerScene = _objectsFactory.SpawnLocalObject(_controllerScenePrefab);
            _clientIdentification.SetPlayerType(PlayerTypes.Big);
        }
        public override void OnNetworkDespawn()
        {
            if(IsServer && _player != null)
            {
                Destroy(_player.gameObject);
            }

            if(!IsLocalPlayer) return;

            if (_controllerScene != null) Destroy(_controllerScene);
        }
    }
}
