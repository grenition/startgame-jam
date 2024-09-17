using UnityEngine;

public class ClientIdentification
{
    public PlayerTypes PlayerType { get; private set; } = PlayerTypes.All;

    public void SetPlayerType(PlayerTypes type)
    {
        if(PlayerType != PlayerTypes.All)
        {
            Debug.LogError("Can't set PlayerType. PlayerType is already set");
            return;
        }

        ForceSetPlayerType(type);
    }

    public void ForceSetPlayerType(PlayerTypes type)
    {
        if (type is PlayerTypes.All or PlayerTypes.LocalPlayers)
        {
            Debug.LogError($"Can't set PlayerType. Incorrect value: {type}");
            return;
        }

        PlayerType = type;
    }

    public bool IsMyType(PlayerTypes type)
    {
        if (type is PlayerTypes.Host)
            return PlayerType is PlayerTypes.Host;
        else if (type is PlayerTypes.Small)
            return PlayerType is PlayerTypes.Small;
        else if (type is PlayerTypes.Big)
            return PlayerType is PlayerTypes.Big;
        else if (type is PlayerTypes.LocalPlayers)
            return PlayerType is PlayerTypes.Small or PlayerTypes.Big;
        else
            return true;
    }
}

public enum PlayerTypes
{
    Host, Small, Big, LocalPlayers, All
}
