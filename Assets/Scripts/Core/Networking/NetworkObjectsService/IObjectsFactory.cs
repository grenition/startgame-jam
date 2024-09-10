using System;
using Gameplay.Player;
using Gameplay.Server;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using NetworkPlayer = Gameplay.Player.NetworkPlayer;

namespace Core.Networking.NetworkObjectsFactory
{
    public interface IObjectsFactory
    {
        public Func<IObjectResolver> OvveridedContainer { get; set; }

        public NetworkObject SpawnNetworkObject(NetworkObject prefab);
        public NetworkPlayer SpawnNetworkPlayer(ulong clientId);
        public ServerPlayer SpawnServerPlayer();
        public T SpawnLocalObject<T>(T prefab) where T : MonoBehaviour;
        public GameObject SpawnLocalObject(GameObject prefab);
    }
}
