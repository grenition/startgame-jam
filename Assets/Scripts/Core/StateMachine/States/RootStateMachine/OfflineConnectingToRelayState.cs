using System;
using Core.Constants;
using Core.Networking.RelayService;
using Core.StateMachine.Abstract.CreatableState;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Core.StateMachine.States
{
    public class OfflineConnectingToRelayState<TStateId> : State<TStateId>
    {
        public override string Name => "OfflineConnectingToRelayState";
        
        private IRelayController _relayController;
        private NetworkManager _networkManager;
        private IRelayConnectionRequester _relayConnectionRequester;
        
        [Inject]
        private void Construct(
            IRelayController relayController,
            NetworkManager networkManager,
            IRelayConnectionRequester relayConnectionRequester)
        {
            _relayController = relayController;
            _networkManager = networkManager;
            _relayConnectionRequester = relayConnectionRequester;
        }

        protected override async UniTask OnEnter()
        {
            _networkManager.OnClientStarted += CompleteState;
            _relayConnectionRequester.OnResponce += JoinRelay;

            if (GamePreferences.IsServer)
                _relayController.CreateRelay();
            else
                _relayConnectionRequester.OpenConnectionRequest();
        }
        protected override async UniTask OnExit()
        {
            _networkManager.OnClientStarted -= CompleteState;
            _relayConnectionRequester.OnResponce -= JoinRelay;
        }
        private async void JoinRelay(string code)
        {
            try
            {
                await _relayController.JoinRelay(code);
            }
            catch (Exception e)
            {
                IsFailed = true;
            }
        }
    }
}
