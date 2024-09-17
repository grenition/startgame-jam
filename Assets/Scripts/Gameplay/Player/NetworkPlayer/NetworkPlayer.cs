using Core.Networking.NetworkObjectsFactory;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Gameplay.Player
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [SerializeField] private GameObject _controllerScenePrefab;
        
        private IObjectsFactory _objectsFactory;
        private ClientIdentification _clientIdentification;
        private GameObject _controllerScene;
        
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
            if(!IsLocalPlayer) return;
            
            _controllerScene = _objectsFactory.SpawnLocalObject(_controllerScenePrefab);
            _clientIdentification.SetPlayerType(PlayerTypes.Big);
        }
        public override void OnNetworkDespawn()
        {
            if(!IsLocalPlayer) return;

            if (_controllerScene != null) Destroy(_controllerScene);
        }
    }
}
