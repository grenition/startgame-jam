using UnityEngine.UI;
using UnityEngine;

public class UILine : Graphic
{
    [field: SerializeField] public float Width { get; set; }
    [field: SerializeField] public Vector2 StartPoint { get; set; }
    [field: SerializeField] public Vector2 EndPoint { get; set; }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Vector2 vec = EndPoint - StartPoint;
        float d = vec.magnitude;
        if (d == 0)
            return;

        float sin = vec.y / d;
        float cos = vec.x / d;
        float dx = Width * sin;
        float dy = Width * cos;
        vec = new Vector2(dx, -dy);

        var vertex = UIVertex.simpleVert;

        vertex.position = StartPoint - vec;
        vh.AddVert(vertex);
        vertex.position = StartPoint + vec;
        vh.AddVert(vertex);
        vertex.position = EndPoint - vec;
        vh.AddVert(vertex);
        vertex.position = EndPoint + vec;
        vh.AddVert(vertex);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(1, 2, 3);
    }

}
