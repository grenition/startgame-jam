using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StringStarter : ActivityStarter
{
    [SerializeField] private RectTransform _parent;
    [SerializeField] private Transform[] _smallObjects, _bigObjects;
    [SerializeField] private GameObject _smallParent, _bigParent;
    [SerializeField] private LineRenderer _line;
    [SerializeField] private Transform _linesParent;

    private List<KeyValuePair<int, int>> _connections = new();
    private List<LineRenderer> _lines = new();
    private List<List<int>> _itemToItem = new();
    private List<List<int>> _itemToLine = new();
    private Transform[] _objects;

    public override RectTransform GetScreenChild()
    {
        return _parent;
    }

    public override async UniTask OnFinish()
    {
        await UniTask.Yield();
        return;
    }

    protected override void OnInitialize(Image screen)
    {
        if(Identification.PlayerType is PlayerTypes.Small)
        {
            _smallParent.SetActive(true);
            _objects = _smallObjects;
            _connections.AddRange(new KeyValuePair<int, int>[]
            {
                new(0, 1), new(0, 2), new(0, 3), new(1, 4), new(1, 3), new(1, 2), new(2, 4), new(2, 3)
            });
        }
        else
        {
            _objects = _bigObjects;
            _bigParent.SetActive(true);
        }

        CreateConnections();
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

        for(int i = 0; i < _objects.Length; i++)
        {
            ReDrawConnection(i);
        }
    }

    private void ReDrawConnection(int itemIndex)
    {
        foreach(var lineIndex in _itemToLine[itemIndex])
        {
            _lines[lineIndex].SetPosition(0, _objects[_connections[lineIndex].Key].transform.position - new Vector3(0, 0, 1));
            _lines[lineIndex].SetPosition(1, _objects[_connections[lineIndex].Value].transform.position - new Vector3(0, 0, 1));
        }
    }
}
