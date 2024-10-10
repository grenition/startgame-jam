using Cysharp.Threading.Tasks;
using UnityEngine;

public class BigPapaModelEvents : MonoBehaviour
{
    [SerializeField] private ParticleSystem _stepEffects;
    [SerializeField] private AudioSource _stepSound;

    public void StepEffect()
    {
        StepEffectAsync();
    }

    private async void StepEffectAsync()
    {
        _stepEffects.Play();
        _stepSound.Play();
        await UniTask.Yield();
        _stepEffects.Stop();
    }
}
