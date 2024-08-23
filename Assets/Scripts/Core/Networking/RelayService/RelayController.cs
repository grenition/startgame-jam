using System;
using SickDev.CommandSystem;
using UniRx;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay.Models;
using UnityEngine;
using VContainer.Unity;

namespace Core.Networking.RelayService
{
    public class RelayController : IRelayController, IInitializable
    {
        public const int MaxConnections = 2;
        public ReadOnlyReactiveProperty<string> JoinCode => _joinCode.ToReadOnlyReactiveProperty();
        
        private ReactiveProperty<string> _joinCode = new();
        
        public void Initialize()
        {
            DevConsole.singleton.AddCommand(new ActionCommand(CreateRelay));
            DevConsole.singleton.AddCommand(new ActionCommand<string>(JoinRelay));
        }
        
        public async void CreateRelay()
        {
            try
            {
                Allocation allocation = await Unity.Services.Relay.RelayService.Instance
                    .CreateAllocationAsync(MaxConnections);

                _joinCode.Value = await Unity.Services.Relay.RelayService.Instance
                    .GetJoinCodeAsync(allocation.AllocationId);
                
                NetworkManager.Singleton.GetComponent<UnityTransport>()
                    .SetHostRelayData(
                        allocation.RelayServer.IpV4,
                        (ushort)allocation.RelayServer.Port,
                        allocation.AllocationIdBytes,
                        allocation.Key,
                        allocation.ConnectionData
                    );

                NetworkManager.Singleton.StartHost();
                
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
                
                NetworkManager.Singleton.GetComponent<UnityTransport>()
                    .SetClientRelayData(
                        joinAllocation.RelayServer.IpV4,
                        (ushort)joinAllocation.RelayServer.Port,
                        joinAllocation.AllocationIdBytes,
                        joinAllocation.Key,
                        joinAllocation.ConnectionData,
                        joinAllocation.HostConnectionData
                    );

                NetworkManager.Singleton.StartClient();
                
                Debug.Log("Joined relay: " + joinCode);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to join relay! \n" + ex);
            }
        }
    }
}
