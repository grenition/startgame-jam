using UnityEngine;
using VContainer;

public class SceneStarter : MonoBehaviour
{
    public enum LevelType { None, FirstLevel }

    [SerializeField] private LevelType _levelType;

    [Inject]
    private void Construct(ControllerNetworkBus bus)
    {
        if (_levelType is LevelType.FirstLevel)
        {
            bus.MessageReceiver = new FirstLevel();
        }

        Destroy(gameObject);
    }
}
