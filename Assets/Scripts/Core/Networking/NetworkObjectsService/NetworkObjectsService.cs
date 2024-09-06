using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Core.Networking.NetworkObjectsService
{
    public class NetworkObjectsService : INetworkObjectsService
    {
        private NetworkManager _networkManager;
        
        [Inject]
        public NetworkObjectsService(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        public NetworkObject SpawnNetworkObject(NetworkObject prefab)
        {
            if (prefab == null) return null;

            var networkObject = GameObject.Instantiate(prefab);
            networkObject.Spawn();

            return networkObject;
        }
    }
}
