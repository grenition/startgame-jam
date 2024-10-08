using Unity.Netcode;
using UnityEngine;

public abstract class NetworkBusLevelMessageReceiver
{
    public abstract void OnReceiveMessage(string id, params int[] data);
}
