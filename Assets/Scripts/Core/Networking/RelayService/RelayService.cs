using SickDev.CommandSystem;
using UnityEngine;

namespace Core.Networking.RelayService
{
    public class RelayService : IRelayService
    {
        public void CreateRelay()
        {
            Debug.Log("Relay created!");
        }
        
        public void JoinRelay(string joinCode)
        {
            Debug.Log("Joined relay: " + joinCode);
        }
    }
}
