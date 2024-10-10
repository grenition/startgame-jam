using UnityEngine;

[CreateAssetMenu(fileName = "ActivityInfo", menuName = "Activity/Default")]
public class ActivityInfo : ScriptableObject
{
    [SerializeField] private Sprite _image;
    [SerializeField] private ActivityStarter _miniGamePrefab;
    [SerializeField] private bool _singlePlayer;
    [SerializeField] private bool _onlyOnePlayer;
    [SerializeField] private string _cantInteractWithTwoReason;

    public Sprite Image => _image;
    public ActivityStarter MiniGamePrefab => GetActivityStarter();
    public bool SinglePlayer => _singlePlayer;
    public bool OnlyOnePlayer => _onlyOnePlayer;

    public virtual ActivityStarter GetActivityStarter()
    {
        return _miniGamePrefab;
    }

    public virtual bool CanInteract(out string reason)
    {
        reason = "";
        return true;
    }

    public bool CanInteractWithTwoPlayers(
        ControllerNetworkBus bus,
        ClientIdentification identification,
        out string reason)
    {
        reason = _cantInteractWithTwoReason;
        if(_onlyOnePlayer)
        {
            var info = bus.SmallPlayerActivity;
            if (identification.PlayerType is PlayerTypes.Small)
                info = bus.BigPlayerActivity;

            return info == null;
        }
        return true;
    }
}
