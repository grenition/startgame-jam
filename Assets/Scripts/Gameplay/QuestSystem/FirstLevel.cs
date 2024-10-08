using Unity.Netcode;
using UnityEngine;

public class FirstLevel : NetworkBehaviour
{
    [SerializeField] private GameObject[] _openedFence, _closedFence;

    public void BearOpenDoorsClient()
    {
        BearOpenDoorsServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void BearOpenDoorsServerRpc()
    {
        foreach(var opened in _openedFence)
        {
            opened.SetActive(true);
        }
        foreach(var closed in _closedFence)
        {
            closed.SetActive(false);
        }
    }
}
