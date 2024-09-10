using UnityEngine;

namespace Core.SceneManagement
{
    [CreateAssetMenu(menuName = "Data/SceneConfig", fileName = "SceneConfig")]
    public class SceneConfig : ScriptableObject
    {
        [field: SerializeField] public string SceneName { get; private set; } = string.Empty;
        [field: SerializeField, TextArea] public string SceneDescription { get; private set; } = string.Empty;
        [field: SerializeField, Scene] public string Scene { get; private set; } = string.Empty;
    }
}
