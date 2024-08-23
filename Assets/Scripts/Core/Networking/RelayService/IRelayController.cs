using System.Threading.Tasks;
using UniRx;
using Unity.Services.Relay.Models;

namespace Core.Networking.RelayService
{
    public interface IRelayController
    {
        public ReadOnlyReactiveProperty<string> JoinCode { get; }
        
        public void CreateRelay();
        public void JoinRelay(string joinCode);
    }
}
