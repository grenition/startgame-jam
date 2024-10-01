using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPool : MonoBehaviour
{
    public const int InitPoolCount = 6;

    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _takeItemSound;

    private readonly Stack<AudioSource> _pool = new(InitPoolCount);

    private void Start()
    {
        for(int i = 0; i < InitPoolCount; i++)
        {
            _pool.Push(Instantiate(_source, _source.transform.parent));
        }
    }

    private AudioSource GetSource()
    {
        if(_pool.Count > 0)
        {
            return _pool.Pop();
        }
        else
        {
            return Instantiate(_source, _source.transform.parent);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        var source = GetSource();
        source.clip = clip;
        StartCoroutine(WaitForEnd(source));
        source.Play();
    }

    public void PlayTakeItemSound(AudioClip clip = null)
    {
        PlaySound(clip ?? _takeItemSound);
    }

    private IEnumerator WaitForEnd(AudioSource source)
    {
        while(source.isPlaying)
        {
            yield return null;
        }
        _pool.Push(source);
    }
}
