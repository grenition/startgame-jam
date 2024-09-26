using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class StringStarter : ActivityStarter
{
    [SerializeField] private RectTransform _parent;
    [SerializeField] private DragNDropElement[] _smallObjects, _bigObjects;
    [SerializeField] private GameObject _smallParent, _bigParent;
    [SerializeField] private UILine _line;
    [SerializeField] private Transform _linesParent;
    [SerializeField] private Image _grassToInactive;

    [SerializeField] private Material _default;
    [SerializeField] private Material _wrong;

    public static readonly Vector2 ScreenSize = new Vector2(1280, 720);
    public const float FloatCalculationsInaccuracy = .000001f;
    public const string MessageID = "StringStarterWin";

    private readonly List<KeyValuePair<int, int>> _connections = new();
    private readonly List<UILine> _lines = new();
    private readonly List<List<int>> _itemToItem = new();
    private readonly List<List<int>> _itemToLine = new();
    private RectTransform[] _objects;
    private bool _draging = false;

    private Inventory _inventory;

    private bool _smallWin = false, _bigWin = false;

    [Inject]
    private void Construct(Inventory inventory)
    {
        _inventory = inventory;
    }

    public override RectTransform GetScreenChild()
    {
        return _parent;
    }

    public override async UniTask OnFinish()
    {
        await UniTask.Yield();
        return;
    }

    private void OnReceiveMessage(string id, int[] data)
    {
        if(id == MessageID)
        {
            PlayerTypes type = (PlayerTypes)data[0];
            if(type is PlayerTypes.Small)
                _smallWin = true;
            else if(type is PlayerTypes.Big)
                _bigWin = true;

            if(type is PlayerTypes.Big && _smallWin && _bigWin)
            {
                Bus.SpecialDataTransmitted -= OnReceiveMessage;
                Finish();
            }
        }
    }

    protected override void OnInitialize(Image screen)
    {
        Bus.SpecialDataTransmitted += OnReceiveMessage;
        if(_inventory.HasItemByName(_inventory.Names.Pruner))
        {
            _inventory.RemoveItemByName(_inventory.Names.Pruner);
        }

        if(Identification.PlayerType is PlayerTypes.Small)
        {
            StartCoroutine(StartIE(_smallParent));
            _objects = _smallObjects.Select(el => el.GetComponent<RectTransform>()).ToArray();
            _connections.AddRange(new KeyValuePair<int, int>[]
            {
                new(0, 1), new(0, 3), new(0, 4), new(0, 5),
                new(2, 1), new(2, 3), new(2, 4), new(2, 5),
                new(1, 3), new(3, 4), new(4, 5)
            });
        }
        else
        {
            _objects = _bigObjects.Select(el => el.GetComponent<RectTransform>()).ToArray();
            StartIE(_bigParent);
            _connections.AddRange(new KeyValuePair<int, int>[]
            {
                new(0, 1), new(0, 2), new(0, 3), new(1, 4),
                new(1, 3), new(1, 2), new(2, 4), new(2, 3)
            });
        }

        CreateConnections();
        CreateActions((Identification.PlayerType is PlayerTypes.Small) ? _smallObjects : _bigObjects);
    }

    private IEnumerator StartIE(GameObject active)
    {
        _grassToInactive.DOColor(new Color(1, 1, 1, 0), .3f);
        yield return new WaitForSeconds(.3f);
        active.SetActive(true);
    }

    private void CreateActions(DragNDropElement[] elements)
    {
        foreach(var obj in elements)
        {
            obj.StartedDraging += OnDragStarted;
            obj.Droped += OnDragEnded;
        }
    }

    private void OnDragStarted(DragNDropElement drag)
    {
        _draging = true;
        ColorAll(_default);
    }

    private void OnDragEnded(DragNDropElement drag)
    {
        _draging = false;
        var pos = drag.transform.localPosition;
        if(pos.x < -ScreenSize.x / 2f)
        {
            pos.x = -ScreenSize.x / 2f + 10f;
        }
        else if(pos.x > ScreenSize.x / 2f)
        {
            pos.x = ScreenSize.x / 2f - 10f;
        }

        if(pos.y < -ScreenSize.y / 2f)
        {
            pos.y = -ScreenSize.y / 2f + 10f;
        }
        else if(pos.y > ScreenSize.y / 2f)
        {
            pos.y = ScreenSize.y / 2f - 10f;
        }

        drag.transform.localPosition = pos;
        ReDrawConnections();
        CheckCrosses();
    }

    private void Update()
    {
        if(_draging)
        {
            ReDrawConnections();
        }
    }

    private void CreateConnections()
    {
        _lines.Add(_line);
        for(int i = 1; i < _connections.Count; i++)
        {
            _lines.Add(Instantiate(_line, _linesParent));
        }

        for(int i = 0; i < _objects.Length; i++)
        {
            var itemList = new List<int>();
            var lineList = new List<int>();
            for(int j = 0; j < _connections.Count; j++)
            {
                if (_connections[j].Key == i)
                {
                    lineList.Add(j);
                    itemList.Add(_connections[j].Value);
                }
                else if (_connections[j].Value == i)
                {
                    lineList.Add(j);
                    itemList.Add(_connections[j].Key);
                }
            }
            _itemToItem.Add(itemList);
            _itemToLine.Add(lineList);
        }

        ReDrawConnections();
    }

    private void ReDrawConnections()
    {
        for (int i = 0; i < _objects.Length; i++)
        {
            ReDrawConnection(i);
        }
        foreach (var line in _lines)
        {
            line.SetAllDirty();
        }
    }

    private void ReDrawConnection(int itemIndex)
    {
        foreach(var lineIndex in _itemToLine[itemIndex])
        {
            _lines[lineIndex].StartPoint = _objects[_connections[lineIndex].Key].localPosition;
            _lines[lineIndex].EndPoint = _objects[_connections[lineIndex].Value].localPosition;
        }
    }

    private void ColorAll(Material mat)
    {
        foreach(var line in _lines)
        {
            line.material = mat;
        }
    }

    private void CheckCrosses()
    {
        bool hasWrongStrings = false;
        foreach(var line in _lines)
        {
            foreach(var otherLine in _lines)
            {
                if (line == otherLine)
                    continue;

                bool isFirstUp = line.EndPoint.x - line.StartPoint.x == 0;
                bool isSecondUp = otherLine.EndPoint.x - otherLine.StartPoint.x == 0;

                if (isFirstUp && isSecondUp)
                    continue;

                if(!isFirstUp && !isSecondUp)
                {
                    float k1 = (line.EndPoint.y - line.StartPoint.y) / (line.EndPoint.x - line.EndPoint.y);
                    float k2 = (otherLine.EndPoint.y - otherLine.StartPoint.y) / (otherLine.EndPoint.x - otherLine.EndPoint.y);
                    if (Mathf.Abs(k2 - k1) <= FloatCalculationsInaccuracy)
                        continue;
                }

                var cross = FindCross(line.StartPoint, line.EndPoint, otherLine.StartPoint, otherLine.EndPoint);

                float left1 = (cross.x - line.StartPoint.x) / (line.EndPoint.x - line.StartPoint.x);

                float left2 = (cross.x - otherLine.StartPoint.x) / (otherLine.EndPoint.x - otherLine.StartPoint.x);

                if(left1 > 0 && left1 < 1 && left2 > 0 && left2 < 1)
                {
                    line.material = _wrong;
                    otherLine.material = _wrong;
                    hasWrongStrings = true;
                }
            }
        }

        if(!hasWrongStrings)
        {
            WaitForWin();
        }
    }

    private void WaitForWin()
    {
        var list = _bigObjects.ToList();
        list.AddRange(_smallObjects);
        foreach(var obj in list)
        {
            obj.CanDrag = false;
        }
        var data = new int[] { (int)Identification.PlayerType };
        Bus.SendBusMessage(MessageID, data, PlayerTypes.LocalPlayers);
    }

    private Vector2 FindCross(Vector2 startPoint, Vector2 endPoint, Vector2 otherStartPoint, Vector2 otherEndPoint)
    {
        var x1 = startPoint.x; var y1 = startPoint.y;
        var x2 = endPoint.x; var y2 = endPoint.y;
        var x3 = otherStartPoint.x; var y3 = otherStartPoint.y;
        var x4 = otherEndPoint.x; var y4 = otherEndPoint.y;

        var cross = new Vector2(
            ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) /
            ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)),
            ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) /
            ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4))
            );

        return cross;
    }
}
