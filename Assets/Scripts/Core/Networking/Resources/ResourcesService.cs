using UnityEngine;
using VContainer.Unity;

namespace Core.Networking.Settings
{
    public class ResourcesService : IInitializable
    {
        public NetworkSettings NetworkSettings { get; private set; }
        
        public void Initialize()
        {
            NetworkSettings = Resources.Load<NetworkSettings>("Core/NetworkSettings");
        }
    }
}
