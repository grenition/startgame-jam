using System;
using Core.Networking.NetworkPlayersService;
using Core.Networking.Settings;
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
    public class ObjectsFactory : IObjectsFactory, IInitializable
    {
        public const string NetworkScene = "NetworkScene";

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
            SceneManager.MoveGameObjectToScene(networkPlayer.gameObject, SceneManager.GetSceneByName(NetworkScene));
            networkPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

            return networkPlayer;
        }

        public ServerPlayer SpawnServerPlayer()
        {
            var prefab = _resources.NetworkSettings.ServerPlayer;
            
            if (!_networkManager.IsServer || prefab == null) return null;
            
            var player = GetContainer().Instantiate(prefab);
            player.name = $"ServerPlayer";
            SceneManager.MoveGameObjectToScene(player.gameObject, SceneManager.GetSceneByName(NetworkScene));
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.ServerClientId);

            return player;
        }

        public T SpawnLocalObject<T>(T prefab, TargetScene targetScene, bool inject = true) where T : Component
        {
            if (!prefab) return null;

            var obj = inject ? GetContainer().Instantiate(prefab) : Object.Instantiate(prefab);
            
            switch (targetScene)
            {
                case TargetScene.NetworkScene:
                    SceneManager.MoveGameObjectToScene(obj.gameObject, SceneManager.GetSceneByName(NetworkScene));
                    break;
                case TargetScene.GameScene:
                    var serverSceneMover = new ServerSceneMover(obj.gameObject);
                    GetContainer().Inject(serverSceneMover);
                    serverSceneMover.TryMove();
                    break;
            }
            return obj;
        }
        public GameObject SpawnLocalObject(GameObject prefab, TargetScene targetScene, bool inject = true)
        {
            if (!prefab) return null;

            var obj = inject ? GetContainer().Instantiate(prefab) : Object.Instantiate(prefab);
            
            switch (targetScene)
            {
                case TargetScene.NetworkScene:
                    SceneManager.MoveGameObjectToScene(obj.gameObject, SceneManager.GetSceneByName(NetworkScene));
                    break;
                case TargetScene.GameScene:
                    var serverSceneMover = new ServerSceneMover(obj.gameObject);
                    GetContainer().Inject(serverSceneMover);
                    serverSceneMover.TryMove();
                    break;
            }
            return obj;
        }

        private IObjectResolver GetContainer()
        {
            var resolver = OvveridedContainer?.Invoke();

            return resolver ?? _objectResolver;
        }

    }
    public class ServerSceneMover
    {
        private INetworkPlayersService _networkPlayersService;
        private GameObject obj;

        [Inject]
        public void Construct(INetworkPlayersService networkPlayersService)
        {
            _networkPlayersService = networkPlayersService;
        }

        public ServerSceneMover(GameObject obj) => this.obj = obj;

        public bool TryMove()
        {
            if (_networkPlayersService == null || _networkPlayersService.ServerPlayer == null)
                return false;
            var serverPlayer = _networkPlayersService.ServerPlayer;

            var sceneLoaded = false;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == _networkPlayersService.ServerPlayer.CurrentData.activeScene)
                {
                    sceneLoaded = true;
                    break;
                }
            }

            if (!sceneLoaded) return false;
            
            SceneManager.MoveGameObjectToScene(obj.gameObject, SceneManager.GetSceneByName(serverPlayer.CurrentData.activeScene));
            return true;
        }
    }
}
