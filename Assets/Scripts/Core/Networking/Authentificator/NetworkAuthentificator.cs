using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using VContainer;
using VContainer.Unity;

namespace Core.Networking.Authentificator
{
    public class NetworkAuthentificator : IInitializable, IDisposable
    {
        public List<string> AllowedTokens { get; set; } = new();
        
        private NetworkManager _networkManager;

        [Inject]
        private void Construct(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        public void Initialize()
        {
            _networkManager.ConnectionApprovalCallback += ApproveConnection;
        }
        public void Dispose()
        {
            _networkManager.ConnectionApprovalCallback -= ApproveConnection;
        }
        
        private void ApproveConnection(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            var clientAuthData = request.Payload;
            string receivedToken = Encoding.UTF8.GetString(clientAuthData);

            if (AllowedTokens.Contains(receivedToken))
            {
                response.Reason = "Bad auth request";
                response.Approved = false;
                response.Pending = true; 
            }
            else
            {
                response.Approved = true;
                response.CreatePlayerObject = true;
            }
        }
    }
}
