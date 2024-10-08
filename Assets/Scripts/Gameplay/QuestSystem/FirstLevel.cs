using UnityEngine;

public class FirstLevel : NetworkBusLevelMessageReceiver
{
    [SerializeField] private GameObject[] _openedFence, _closedFence;

    public const string BearDoorsID = "BearDoorsID";

    public override void OnReceiveMessage(string id, params int[] data)
    {
        if(id == BearDoorsID)
        {
            BearOpenDoors();
        }
    }

    private void BearOpenDoors()
    {
        foreach (var opened in _openedFence)
        {
            opened.SetActive(true);
        }
        foreach (var closed in _closedFence)
        {
            closed.SetActive(false);
        }
    }
}
