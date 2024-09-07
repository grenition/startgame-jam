using System;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using NetworkPlayer = Core.Gameplay.Player.NetworkPlayer;
using Object = UnityEngine.Object;

namespace Core.Networking.NetworkObjectsFactory
{
    public class NetworkObjectsSpawnHandler : INetworkPrefabInstanceHandler
    {
        private NetworkObject _prefab;
        private Func<IObjectResolver> _objectResolverGetter;

        public NetworkObjectsSpawnHandler(NetworkObject prefab, Func<IObjectResolver> objectResolverGetter)
        {
            _prefab = prefab;
            _objectResolverGetter = objectResolverGetter;
        }
        
        public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            var objectResolver = _objectResolverGetter?.Invoke();
            if (objectResolver == null) return null;
            
            var instance = objectResolver.Instantiate(_prefab, position, rotation);
            
            if(instance.TryGetComponent(out NetworkPlayer networkPlayer))
                networkPlayer.name = $"NetworkPlayer: owner {ownerClientId}";
            
            return instance.GetComponent<NetworkObject>();
        }
        
        public void Destroy(NetworkObject networkObject)
        {
            Object.Destroy(networkObject.gameObject);
        }
    }
}
