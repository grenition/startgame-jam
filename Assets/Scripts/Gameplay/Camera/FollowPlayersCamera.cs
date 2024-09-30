using System.Linq;
using UnityEngine;

public class FollowPlayersCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform[] _followObjects;
    [SerializeField] private float _offset;

    private void Update()
    {
        transform.position = CalculatePos();
    }

    private Vector3 GetCenterPos()
    {
        if (_followObjects.Length == 0)
            return Vector3.zero;

        Vector3 pos = Vector3.zero;
        foreach(var obj in _followObjects)
        {
            pos += obj.position;
        }
        return pos / _followObjects.Length;
    }

    private Vector3 CalculatePos()
    {
        var center = GetCenterPos();
        var maxDistance = _followObjects.Max(x => Vector3.Distance(center, x.position));
        Debug.Log(maxDistance);
        var diameter = maxDistance + _offset;

        var distance = diameter / (2f * Mathf.Tan(_camera.fieldOfView * Mathf.Deg2Rad / 2));
        var vector = -_camera.transform.forward;

        return center + vector * distance;
    }
}
