using UnityEngine;

[CreateAssetMenu(fileName = "ActivityInfo", menuName = "Activity")]
public class ActivityInfo : ScriptableObject
{
    [SerializeField] private Sprite _image;

    public Sprite Image => _image;
}
