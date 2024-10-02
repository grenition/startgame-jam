using System.Collections.Generic;
using System.Linq;
using Core.Constants;
using Gameplay.Player;
using Gameplay.Server;
using UniRx;

namespace Core.Networking.NetworkPlayersService
{
    public class NetworkPlayersService : INetworkPlayersService
    {
        public ReactiveProperty<bool> AllPlayersConnected { get; private set; } = new(false);
        public ServerPlayer ServerPlayer { get; private set; }
        public NetworkPlayer BigPlayer => NetworkPlayers.FirstOrDefault(x => x.PlayerType == PlayerTypes.Big);
        public NetworkPlayer SmallPlayer => NetworkPlayers.FirstOrDefault(x => x.PlayerType == PlayerTypes.Small);
        public NetworkPlayer LocalPlayer => NetworkPlayers.FirstOrDefault(x => x.IsLocalPlayer);
        public List<NetworkPlayer> NetworkPlayers { get; private set; } = new();

        public void RegisterServerPlayer(ServerPlayer serverPlayer) => ServerPlayer ??= serverPlayer;
        public void UnregisterServerPlayer(ServerPlayer serverPlayer)
        {
            if(ServerPlayer != serverPlayer) return;
            ServerPlayer = null;
        }
        public void RegisterNetworkPlayer(NetworkPlayer networkPlayer)
        {
            if(networkPlayer == null || NetworkPlayers.Contains(networkPlayer)) return;
            NetworkPlayers.Add(networkPlayer);
            AllPlayersConnected.Value = NetworkPlayers.Count == GamePreferences.PlayersCount;
        }
        public void UnregisterNetworkPlayer(NetworkPlayer networkPlayer)
        {
            if (NetworkPlayers.Contains(networkPlayer))
                NetworkPlayers.Remove(networkPlayer);
            AllPlayersConnected.Value = NetworkPlayers.Count == GamePreferences.PlayersCount;
        }
    }
}
