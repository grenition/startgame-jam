using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Core.Gameplay.Player
{
    public class NetworkPlayer : NetworkBehaviour
    {
        private IObjectResolver _objectResolver;

        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
            Debug.Log($"Player with owner {OwnerClientId} injected");
        }
    }
}
