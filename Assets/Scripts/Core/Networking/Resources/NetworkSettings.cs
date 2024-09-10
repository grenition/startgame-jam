using Gameplay.Server;
using UnityEngine;
using NetworkPlayer = Gameplay.Player.NetworkPlayer;

namespace Core.Networking.Settings
{
    [CreateAssetMenu(menuName = "Core/NetworkSettings", fileName = "NetworkSettings")]
    public class NetworkSettings : ScriptableObject
    {
        [field: SerializeField] public NetworkPlayer Player { get; private set; }
        [field: SerializeField] public ServerPlayer ServerPlayer { get; private set; }
    }
}
