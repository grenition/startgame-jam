using System;
using UniRx;

namespace Core.Networking.RelayService
{
    public class RelayConnectionRequester : IRelayConnectionRequester
    {
        public event Action<string> OnResponce;
        public string JoinCode => _joinCode;
        public ReadOnlyReactiveProperty<bool> RequestOpened => _requestOpened.ToReadOnlyReactiveProperty();

        private string _joinCode;
        private ReactiveProperty<bool> _requestOpened = new(false);
        
        public void OpenConnectionRequest()
        {
            _requestOpened.Value = true;
        }
        public void CloseConnectionRequest(string joinCode)
        {
            if(!_requestOpened.Value) return;
            
            _joinCode = joinCode;
            OnResponce?.Invoke(_joinCode);
            _requestOpened.Value = false;
        }
    }
}
