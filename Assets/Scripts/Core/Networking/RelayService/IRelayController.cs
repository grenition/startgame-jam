using System.Threading.Tasks;
using UniRx;
using Unity.Services.Relay.Models;

namespace Core.Networking.RelayService
{
    public interface IRelayController
    {
        public string ConnectionPayload { get; set; }
        public ReadOnlyReactiveProperty<string> JoinCode { get; }
        
        public Task CreateRelay();
        public Task JoinRelay(string joinCode);
    }
}
