using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowPlayersCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform[] _followObjects;
    [SerializeField] private float _offset;

    private void Update()
    {
        if(_followObjects.Length > 0)
        {
            transform.position = CalculatePos();
        }
    }

    public void AddFollowObject(Transform transform)
    {
        var list = new List<Transform>(_followObjects);
        list.Add(transform);
        _followObjects = list.ToArray();
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
