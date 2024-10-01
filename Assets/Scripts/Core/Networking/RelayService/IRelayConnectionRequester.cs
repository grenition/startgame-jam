using System;
using UniRx;

namespace Core.Networking.RelayService
{
    public interface IRelayConnectionRequester
    {
        public event Action<string> OnResponce;
        public string JoinCode { get; }
        public ReadOnlyReactiveProperty<bool> RequestOpened { get; }

        public void OpenConnectionRequest();
        public void CloseConnectionRequest(string joinCode);
    }
}
