using UnityEngine;
using NetworkPlayer = Core.Gameplay.Player.NetworkPlayer;

namespace Core.Networking.Settings
{
    [CreateAssetMenu(menuName = "Core/NetworkSettings", fileName = "NetworkSettings")]
    public class NetworkSettings : ScriptableObject
    {
        [field: SerializeField] public NetworkPlayer Player { get; private set; }
    }
}
