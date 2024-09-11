using System;
using Core.Networking.Settings;
using Gameplay.Server;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using NetworkPlayer = Gameplay.Player.NetworkPlayer;

namespace Core.Networking.NetworkObjectsFactory
{
    public class ObjectsFactory : IObjectsFactory, IInitializable
    {
        public Func<IObjectResolver> OvveridedContainer { get; set; }

        private NetworkManager _networkManager;
        private ResourcesService _resources;
        private IObjectResolver _objectResolver;
        
        [Inject]
        public ObjectsFactory(
            NetworkManager networkManager,
            ResourcesService resources,
            IObjectResolver objectResolver)
        {
            _networkManager = networkManager;
            _resources = resources;
            _objectResolver = objectResolver;
        }
        
        public void Initialize()
        {
            foreach (var prefabsList in _networkManager.NetworkConfig.Prefabs.NetworkPrefabsLists)
            {
                foreach (var prefab in prefabsList.PrefabList)
                {
                    var networkObject = prefab.Prefab.GetComponent<NetworkObject>();
                    _networkManager.PrefabHandler.AddHandler(networkObject, 
                        new NetworkObjectsSpawnHandler(networkObject, GetContainer));
                }
            }
        }

        public NetworkObject SpawnNetworkObject(NetworkObject prefab)
        {
            if (!_networkManager.IsServer || prefab == null) return null;

            var networkObject = GetContainer().Instantiate(prefab);
            networkObject.Spawn();

            return networkObject;
        }

        public NetworkPlayer SpawnNetworkPlayer(ulong clientId)
        {
            var prefab = _resources.NetworkSettings.Player;
            
            if (!_networkManager.IsServer || prefab == null) return null;
            
            var networkPlayer = GetContainer().Instantiate(prefab);
            networkPlayer.name = $"NetworkPlayer: owner {clientId}";
            networkPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            
            return networkPlayer;
        }

        public ServerPlayer SpawnServerPlayer()
        {
            var prefab = _resources.NetworkSettings.ServerPlayer;
            
            if (!_networkManager.IsServer || prefab == null) return null;
            
            var player = GetContainer().Instantiate(prefab);
            player.name = $"ServerPlayer";
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.ServerClientId);
            
            return player;
        }

        public T SpawnLocalObject<T>(T prefab) where T : MonoBehaviour
        {
            if (prefab == null) return null;

            var obj = UnityEngine.Object.Instantiate(prefab);
            GetContainer().Inject(obj);
            return obj;
        }

        public GameObject SpawnLocalObject(GameObject prefab)
        {
            if (prefab == null) return null;

            var obj = UnityEngine.Object.Instantiate(prefab);
            GetContainer().Inject(obj);
            return obj;
        }

        private IObjectResolver GetContainer()
        {
            var resolver = OvveridedContainer?.Invoke();

            return resolver ?? _objectResolver;
        }
    }
}
