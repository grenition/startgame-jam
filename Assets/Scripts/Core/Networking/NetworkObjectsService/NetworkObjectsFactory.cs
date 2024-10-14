using Core.Networking.Settings;
using Unity.Netcode;
using VContainer;
using VContainer.Unity;

namespace Core.Networking.NetworkObjectsFactory
{
    public class NetworkObjectsFactory : ObjectsFactory, IInitializable
    {

        public NetworkObjectsFactory(NetworkManager networkManager, ResourcesService resources, IObjectResolver objectResolver) : base(networkManager, resources, objectResolver)
        {
        }
        
        public void Initialize()
        {
            foreach (var prefabsList in _networkManager.NetworkConfig.Prefabs.NetworkPrefabsLists)
            {
                foreach (var prefab in prefabsList.PrefabList)
                {
                    var networkObject = prefab.Prefab.GetComponent<NetworkObject>();
                    _networkManager.PrefabHandler.AddHandler(networkObject, 
                        new NetworkObjectsSpawnHandler(networkObject, GetContainer));
                }
            }
        }
    }
}
