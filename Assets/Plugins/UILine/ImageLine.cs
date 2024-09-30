using UnityEngine;
using UnityEngine.UI;

public class ImageLine : MonoBehaviour
{
    [field: SerializeField] public Image Image { get; set; }
    [field: SerializeField] public float Width { get; set; }

    [field: SerializeField] public Vector3 StartPoint { get; set; }
    [field: SerializeField] public Vector3 EndPoint { get; set; }

    private void Update()
    {
        //Calculate();
    }

    public void Calculate()
    {
        var dist = Vector2.Distance(StartPoint, EndPoint);
        Image.rectTransform.offsetMin = new(-dist / 2, -Width / 2);
        Image.rectTransform.offsetMax = new(dist / 2, Width / 2);

        var vec = EndPoint - StartPoint;
        var angle = Vector2.Angle(Vector2.right, vec);
        if (vec.y < 0)
        {
            angle = 180 - angle;
        }

        Image.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        Image.rectTransform.localPosition = StartPoint + vec / 2;
    }
}
