using System.Collections.Generic;

namespace Core.Networking.Authentificator
{
    public interface INetworkAuthentificator
    {
        public List<string> AllowedTokens { get; set; }
    }
}
