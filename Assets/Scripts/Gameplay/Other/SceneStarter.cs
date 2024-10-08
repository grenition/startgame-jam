using UnityEngine;
using VContainer;

public class SceneStarter : MonoBehaviour
{
    public enum LevelType { None, FirstLevel }

    [SerializeField] private LevelType _levelType;
    [SerializeField] private Sprite[] _startComics;

    [Inject]
    private void Construct(ControllerNetworkBus bus, ComicsViewer viewer)
    {
        if (_levelType is LevelType.FirstLevel)
        {
            bus.MessageReceiver = new FirstLevel();
        }

        viewer.OpenComicsWithControllers(_startComics);

        Destroy(gameObject);
    }
}
