using UnityEngine;

[CreateAssetMenu(fileName = "ActivityInfo", menuName = "Activity")]
public class ActivityInfo : ScriptableObject
{
    [SerializeField] private Sprite _image;
    [SerializeField] private ActivityStarter _miniGamePrefab;
    [SerializeField] private bool _singlePlayer;

    public Sprite Image => _image;
    public ActivityStarter MiniGamePrefab => GetActivityStarter();
    public bool SinglePlayer => _singlePlayer;

    public virtual ActivityStarter GetActivityStarter()
    {
        return _miniGamePrefab;
    }

    public virtual bool CanInteract(out string reason)
    {
        reason = "";
        return true;
    }
}
