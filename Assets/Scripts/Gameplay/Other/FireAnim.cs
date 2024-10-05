using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class FireAnim : MonoBehaviour
{
    [Serializable]
    public class Element
    {
        public Transform Object;
        public float MinSize, MaxSize;
        public float TimeStep;
    }

    [SerializeField] private float _minTime, _maxTime;
    [SerializeField] private Element[] _elements;

    private void Start()
    {
        foreach(var element in _elements)
        {
            StartCoroutine(AnimateElement(element));
        }
    }

    private IEnumerator AnimateElement(Element element)
    {
        yield return new WaitForSeconds(element.TimeStep);

        while(true)
        {
            float time = UnityEngine.Random.Range(_minTime, _maxTime);
            float scaleMult = UnityEngine.Random.Range(0f, 1f);
            var size = element.MinSize + (element.MaxSize - element.MinSize) * scaleMult;
            element.Object.DOScale(size, time);

            yield return new WaitForSeconds(time);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
