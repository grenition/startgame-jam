using Core.Networking.RelayService;
using TMPro;
using UniRx;
using UnityEngine;
using VContainer;

namespace Core.UI
{
    public class UIJoinCodeField : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _readOnlyInputField;
        
        private IRelayController _relayController;

        [Inject]
        private void Construct(IRelayController relayController)
        {
            _relayController = relayController;
        }

        private void Awake()
        {
            _relayController.JoinCode
                .Do(_ => UpdateJoinCode())
                .Subscribe()
                .AddTo(this);
            UpdateJoinCode();
        }

        public void UpdateJoinCode()
        {
            _readOnlyInputField.text = _relayController.JoinCode.Value;
        }
    }
}
