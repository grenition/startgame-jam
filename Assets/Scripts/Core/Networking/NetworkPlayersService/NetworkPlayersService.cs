using System.Collections.Generic;
using Core.Gameplay.Player;

namespace Core.Networking.NetworkPlayersService
{
    public class NetworkPlayersService : INetworkPlayersService
    {

        public IReadOnlyList<NetworkPlayer> AllPlayers { get; }
        public NetworkPlayer LocalPlayer { get; }
    }
}
