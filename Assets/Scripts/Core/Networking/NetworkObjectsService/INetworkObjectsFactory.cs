using System;
using Gameplay.Player;
using Gameplay.Server;
using Unity.Netcode;
using VContainer;

namespace Core.Networking.NetworkObjectsFactory
{
    public interface INetworkObjectsFactory
    {
        public Func<IObjectResolver> OvveridedContainer { get; set; }

        public NetworkObject SpawnNetworkObject(NetworkObject prefab);
        public NetworkPlayer SpawnNetworkPlayer(ulong clientId);
        public ServerPlayer SpawnServerPlayer();
    }
}
