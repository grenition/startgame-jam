using System;
using Gameplay.Player;
using Gameplay.Server;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using NetworkPlayer = Gameplay.Player.NetworkPlayer;
using Object = UnityEngine.Object;

namespace Core.Networking.NetworkObjectsFactory
{
    public enum TargetScene
    {
        NetworkScene,
        GameScene,
        None
    }
    public interface IObjectsFactory
    {
        public Func<IObjectResolver> OvveridedContainer { get; set; }

        public NetworkObject SpawnNetworkObject(NetworkObject prefab);
        public NetworkPlayer SpawnNetworkPlayer(ulong clientId);
        public ServerPlayer SpawnServerPlayer();
        public T SpawnLocalObject<T>(T prefab, TargetScene targetScene, bool inject = true) where T : Component;
        public GameObject SpawnLocalObject(GameObject prefab, TargetScene targetScene, bool inject = true);
    }
}
