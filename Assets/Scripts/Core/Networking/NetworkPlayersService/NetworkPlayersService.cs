using System.Collections.Generic;
using Gameplay.Player;

namespace Core.Networking.NetworkPlayersService
{
    public class NetworkPlayersService : INetworkPlayersService
    {

        public IReadOnlyList<NetworkPlayer> AllPlayers { get; }
        public NetworkPlayer LocalPlayer { get; }
    }
}
