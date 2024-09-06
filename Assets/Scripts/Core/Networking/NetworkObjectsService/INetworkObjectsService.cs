using Cysharp.Threading.Tasks;
using Unity.Netcode;

namespace Core.Networking.NetworkObjectsService
{
    public interface INetworkObjectsService
    {
        public NetworkObject SpawnNetworkObject(NetworkObject prefab);
    }
}
