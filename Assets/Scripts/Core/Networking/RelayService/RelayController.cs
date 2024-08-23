using System;
using SickDev.CommandSystem;
using UniRx;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay.Models;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using DevConsole = SickDev.DevConsole.DevConsole;

namespace Core.Networking.RelayService
{
    public class RelayController : IRelayController, IInitializable
    {
        public const int MaxConnections = 2;
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
            _devConsole.AddCommand(new ActionCommand(CreateRelay));
            _devConsole.AddCommand(new ActionCommand<string>(JoinRelay));
        }
        
        public async void CreateRelay()
        {
            try
            {
                Allocation allocation = await Unity.Services.Relay.RelayService.Instance
                    .CreateAllocationAsync(MaxConnections);

                _joinCode.Value = await Unity.Services.Relay.RelayService.Instance
                    .GetJoinCodeAsync(allocation.AllocationId);
                
                _networkManager.GetComponent<UnityTransport>()
                    .SetHostRelayData(
                        allocation.RelayServer.IpV4,
                        (ushort)allocation.RelayServer.Port,
                        allocation.AllocationIdBytes,
                        allocation.Key,
                        allocation.ConnectionData
                    );

                _networkManager.StartHost();
                
                Debug.Log("Relay created, join code: " + JoinCode);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to create relay! \n" + ex);
            }
        }
        public async void JoinRelay(string joinCode)
        {
            try
            {
                var joinAllocation = await Unity.Services.Relay.RelayService.Instance
                    .JoinAllocationAsync(joinCode);
                
                _networkManager.GetComponent<UnityTransport>()
                    .SetClientRelayData(
                        joinAllocation.RelayServer.IpV4,
                        (ushort)joinAllocation.RelayServer.Port,
                        joinAllocation.AllocationIdBytes,
                        joinAllocation.Key,
                        joinAllocation.ConnectionData,
                        joinAllocation.HostConnectionData
                    );

                _networkManager.StartClient();
                
                Debug.Log("Joined relay: " + joinCode);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to join relay! \n" + ex);
            }
        }
    }
}
