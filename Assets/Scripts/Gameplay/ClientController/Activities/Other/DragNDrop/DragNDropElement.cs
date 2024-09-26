using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragNDropElement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private bool _stayElementAfterDrop;

    private Vector3 _initPos, _initScale, _draggingScale;

    public bool Dragging { get; private set; } = false;
    public bool CanDrag { get; set; } = true;
    public bool StayElementAfterDrop { get; set;} = false;

    public event Action<DragNDropElement> StartedDraging;
    public event Action<DragNDropElement> Droped;

    private void Start()
    {
        _initPos = transform.position;
        _initScale = transform.localScale;
        _draggingScale = _initScale * 1.2f;
        StayElementAfterDrop = _stayElementAfterDrop;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!CanDrag) return;

        Dragging = true;
        transform.localScale = _draggingScale;
        StartedDraging?.Invoke(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopDrag();
    }

    public void StopDrag()
    {
        if (!Dragging) return;

        Dragging = false;
        Droped?.Invoke(this);
        transform.localScale = _initScale;
        if (!StayElementAfterDrop)
        {
            transform.position = _initPos;
        }
    }

    private void Update()
    {
        if(Dragging)
        {
            transform.position = Input.mousePosition;
        }
    }
}
