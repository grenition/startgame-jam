using System.Collections.Generic;
using Gameplay.Player;

namespace Core.Networking.NetworkPlayersService
{
    public interface INetworkPlayersService
    {
        public IReadOnlyList<NetworkPlayer> AllPlayers { get; }
        public NetworkPlayer LocalPlayer { get; }
    }
}
