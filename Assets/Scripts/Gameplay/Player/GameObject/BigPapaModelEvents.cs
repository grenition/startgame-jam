using Cysharp.Threading.Tasks;
using UnityEngine;

public class BigPapaModelEvents : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _stepEffects;
    [SerializeField] private AudioSource _stepSound;

    private int _effectIndex = 0;

    public void StepEffect()
    {
        _effectIndex++;
        if (_effectIndex >= _stepEffects.Length)
            _effectIndex = 0;

        _stepEffects[_effectIndex].Play();
        _stepSound.Play();
    }
}
