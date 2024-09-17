using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class ActivityStarter : MonoBehaviour
{
    public ControllerNetworkBus Bus { get; private set; }

    public void Initialize(Image screen, ControllerNetworkBus bus)
    {
        Bus = bus;
        OnInitialize(screen);
    }

    protected abstract void OnInitialize(Image screen);

    public abstract RectTransform GetScreenChild();
    protected void Finish()
    {
        Bus.FinishActivity(PlayerTypes.Small);
    }

    public abstract UniTask OnFinish();
}
