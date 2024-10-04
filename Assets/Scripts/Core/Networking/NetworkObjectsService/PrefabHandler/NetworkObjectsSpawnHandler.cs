using System;
using Core.StateMachine.States;
using Gameplay.Server;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using NetworkPlayer = Gameplay.Player.NetworkPlayer;
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

            var wasActive = _prefab.gameObject.activeSelf;
            _prefab.gameObject.SetActive(false);
            
            var instance = objectResolver.Instantiate(_prefab, position, rotation);
            
            if(instance.TryGetComponent(out NetworkPlayer networkPlayer))
                networkPlayer.name = $"NetworkPlayer: owner {ownerClientId}";
            
            if(instance.TryGetComponent(out ServerPlayer serverPlayer))
                serverPlayer.name = $"ServerPlayer";

            SceneManager.MoveGameObjectToScene(instance.gameObject, SceneManager.GetSceneByName(ObjectsFactory.NetworkScene));

            _prefab.gameObject.SetActive(wasActive);
            instance.gameObject.SetActive(wasActive);
            
            return instance.GetComponent<NetworkObject>();
        }
        
        public void Destroy(NetworkObject networkObject)
        {
            Object.Destroy(networkObject.gameObject);
        }
    }
}
