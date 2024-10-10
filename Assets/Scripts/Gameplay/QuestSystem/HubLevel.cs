using Gameplay.QuestSystem;
using System;
using UnityEngine;

[Serializable]
public class HubLevel : NetworkBusLevelMessageReceiver
{
    [SerializeField] private InteractionPoint _goToNextScenePoint;

    public const string ChangeSceneID = "ChangeScene";

    public override void GoToHub()
    {
        return;
    }

    public override void OnReceiveMessage(string id, params int[] data)
    {
        if(id == ChangeSceneID)
        {
            _goToNextScenePoint.Interact();
        }
    }
}
