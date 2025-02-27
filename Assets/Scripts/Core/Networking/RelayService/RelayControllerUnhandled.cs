using System;
using System.Text;
using System.Threading.Tasks;
using Core.Constants;
using Cysharp.Threading.Tasks;
using SickDev.CommandSystem;
using UniRx;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using DevConsole = SickDev.DevConsole.DevConsole;

namespace Core.Networking.RelayService
{
    public class RelayControllerUnhandled : IRelayController, IInitializable, IDisposable
    {
        public string ConnectionPayload { get; set; } = string.Empty;
        public ReadOnlyReactiveProperty<string> JoinCode => _joinCode.ToReadOnlyReactiveProperty();
        
        private ReactiveProperty<string> _joinCode = new();
        private NetworkManager _networkManager;
        private DevConsole _devConsole;

        [Inject]
        private void Construct(NetworkManager networkManager, DevConsole devConsole)
        {
            _networkManager = networkManager;
            _devConsole = devConsole;
        }
        
        public void Initialize()
        {
            // _devConsole.AddCommand(new ActionCommand(CreateRelay));
            // _devConsole.AddCommand(new ActionCommand<string>(JoinRelay));
            
            _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        }
        public void Dispose()
        {
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }

        public async Task CreateRelay()
        {
            Allocation allocation = await Unity.Services.Relay.RelayService.Instance
                .CreateAllocationAsync(GamePreferences.PlayersCount);
            
            _joinCode.Value = await Unity.Services.Relay.RelayService.Instance
                .GetJoinCodeAsync(allocation.AllocationId);

            var relayServerData = new RelayServerData(allocation, "dtls");
            
            _networkManager.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            _networkManager.StartHost();
            
            Debug.Log("Relay created, join code: " + JoinCode);
        }
        
        public async Task JoinRelay(string joinCode)
        {
            var joinAllocation = await Unity.Services.Relay.RelayService.Instance.JoinAllocationAsync(joinCode);
            
            var relayServerData = new RelayServerData(joinAllocation, "dtls");
            
            _networkManager.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);


            var payload = Encoding.UTF8.GetBytes(ConnectionPayload);
            _networkManager.NetworkConfig.ConnectionData = payload;
            
            _networkManager.StartClient();
            
            Debug.Log("Joined relay: " + joinCode);
        }
        
        private void OnClientDisconnect(ulong clientId)
        {
            if (clientId == _networkManager.LocalClientId)
            {
                string errorMessage = _networkManager.DisconnectReason;
                Debug.Log($"Disconnected from server.");
                _networkManager.Shutdown();
            }
        }
    }
}
