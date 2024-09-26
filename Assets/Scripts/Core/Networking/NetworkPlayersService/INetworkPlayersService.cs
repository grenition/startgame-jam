using System.Collections.Generic;
using Gameplay.Player;
using Gameplay.Server;

namespace Core.Networking.NetworkPlayersService
{
    public interface INetworkPlayersService
    {
        public ServerPlayer ServerPlayer { get; }
        public NetworkPlayer BigPlayer { get; }
        public NetworkPlayer SmallPlayer { get; }
        public NetworkPlayer LocalPlayer { get; }
        public List<NetworkPlayer> NetworkPlayers { get; }

        public void RegisterServerPlayer(ServerPlayer serverPlayer);
        public void UnregisterServerPlayer(ServerPlayer serverPlayer);
        public void RegisterNetworkPlayer(NetworkPlayer networkPlayer);
        public void UnregisterNetworkPlayer(NetworkPlayer networkPlayer);
    }
}
