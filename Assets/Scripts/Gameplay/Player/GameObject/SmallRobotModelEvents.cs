using DG.Tweening;
using System.Collections;
using UnityEngine;

public class SmallRobotModelEvents : MonoBehaviour
{
    [SerializeField] private AudioSource _walkingSoundSource;

    private Coroutine _walkingSoundCor;
    private float _maxVolume = 1f;
    private bool _playing = false;

    public const float FadeDuration = .5f;

    private void Start()
    {
        _maxVolume = _walkingSoundSource.volume;
    }

    public void IdleAnim()
    {
        if(_playing)
        {
            _playing = false;

            if (_walkingSoundCor != null)
                StopCoroutine(_walkingSoundCor);
            _walkingSoundCor = StartCoroutine(WalkingSoundIE(0f, true));
        }
    }

    public void WalkingAnim()
    {
        if(!_playing)
        {
            _playing = true;

            if (_walkingSoundCor != null)
                StopCoroutine(_walkingSoundCor);
            _walkingSoundCor = StartCoroutine(WalkingSoundIE(_maxVolume, false));
        }
    }

    private IEnumerator WalkingSoundIE(float volume, bool stop)
    {
        if(!stop && !_walkingSoundSource.isPlaying)
        {
            _walkingSoundSource.Play();
        }

        _walkingSoundSource.DOFade(volume, FadeDuration);

        yield return new WaitForSeconds(FadeDuration);

        if (stop && _walkingSoundSource.isPlaying)
        {
            _walkingSoundSource.Stop();
        }
    }
}
