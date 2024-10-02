using Core.Extensions;
using Core.Networking.RelayService;
using Core.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI.Panels
{
    public class UIConnectionRequestPanel : UIPanel
    {
        [SerializeField] private TMP_InputField _codeInputField;
        [SerializeField] private Button _connectButton;
        
        private IRelayConnectionRequester _connectionRequester;
        
        [Inject]
        private void Construct(IRelayConnectionRequester connectionRequester)
        {
            _connectionRequester = connectionRequester;
        }
        
        protected override void Awake()
        {
            base.Awake();
            
            _connectionRequester.RequestOpened
                .Do(value => UpdatePanelState())
                .Subscribe()
                .AddTo(this);

            _connectButton
                .Bind(() =>
                {
                    _connectionRequester.CloseConnectionRequest(_codeInputField.text);
                }).AddTo(this);
            
            UpdatePanelState();
        }

        public void UpdatePanelState()
        {
            if (!_connectionRequester.RequestOpened.Value)
                Hide();
            else
                Show();
        }
    }
}
