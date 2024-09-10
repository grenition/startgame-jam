using UnityEngine;

namespace Core.SceneManagement
{
    [CreateAssetMenu(menuName = "Data/SceneTransitionsConfig", fileName = "SceneTransitionsConfig")]
    public class SceneTransitionsConfig : ScriptableObject
    {
        [field: SerializeField] public SceneConfig HUBScene { get; private set; }
    }
}
