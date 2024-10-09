using UnityEngine;
using VContainer;

public class SceneStarter : MonoBehaviour
{
    public enum LevelType { None, FirstLevel }

    [SerializeField] private LevelType _levelType;
    [SerializeField] private FirstLevel _firstLevel;

    [Inject]
    private void Construct(ControllerNetworkBus bus, ComicsViewer viewer, IObjectResolver resolver)
    {
        if (_levelType is LevelType.FirstLevel)
        {
            bus.MessageReceiver = _firstLevel;
            resolver.Inject(_firstLevel);
        }

        Destroy(gameObject);
    }
}
