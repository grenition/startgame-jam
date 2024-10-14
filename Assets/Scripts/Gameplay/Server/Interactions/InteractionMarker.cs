using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class InteractionMarker
{
    public List<GameObject> Markers { get; } = new(2);
    public Dictionary<ActivityInfo, GameObject> UsedMarkers { get; } = new(2);

    private List<ActivityInfo> _tasks = new();
    private List<ActivityInfo> _demarkTasks = new();

    public InteractionMarker(IEnumerable<GameObject> markers)
    {
        Markers.AddRange(markers);
    }

    public async void Mark(Vector3 pos, ActivityInfo info)
    {
        if (_tasks.Contains(info) || UsedMarkers.ContainsKey(info))
            return;
        _tasks.Add(info);

        while (Markers.Count == 0)
        {
            await UniTask.Yield();
        }

        var marker = Markers[^1];
        Markers.RemoveAt(Markers.Count - 1);

        marker.transform.position = pos;
        marker.transform.localScale = new(1, 0, 1);
        marker.SetActive(true);
        marker.transform.DOScaleY(1.5f, .3f);

        await UniTask.WaitForSeconds(.2f);

        marker.transform.DOScaleY(1f, .2f);

        await UniTask.WaitForSeconds(.2f);

        _tasks.Remove(info);

        if(UsedMarkers.ContainsKey(info))
        {
            Markers.Add(marker);
            marker.SetActive(false);
        }
        else
        {
            UsedMarkers.Add(info, marker);
        }
    }

    public async UniTask DeMark(ActivityInfo info)
    {
        if (!_tasks.Contains(info) && !UsedMarkers.ContainsKey(info))
            return;

        if (_demarkTasks.Contains(info))
            return;
        _demarkTasks.Add(info);
        while (!UsedMarkers.ContainsKey(info))
        {
            await UniTask.Yield();
        }

        var marker = UsedMarkers[info];
        UsedMarkers.Remove(info);
        _demarkTasks.Remove(info);

        marker.transform.DOScale(Vector3.zero, .5f);

        await UniTask.WaitForSeconds(.5f);

        marker.SetActive(false);
        Markers.Add(marker);
    }

    public async void DeMarkAll()
    {
        _tasks.Clear();

        while (UsedMarkers.Count > 0)
        {
            foreach (var info in UsedMarkers.Keys)
            {
                await DeMark(info);
                break;
            }
        }
    }
}
