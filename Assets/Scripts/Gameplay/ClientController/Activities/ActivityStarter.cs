using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public abstract class ActivityStarter : MonoBehaviour
{
    public ControllerNetworkBus Bus { get; private set; }
    public ClientIdentification Identification { get; private set; }

    public void Initialize(Image screen, ControllerNetworkBus bus)
    {
        Bus = bus;
        OnInitialize(screen);
    }

    [Inject]
    private void Construct(ClientIdentification identification)
    {
        Identification = identification;
    }

    protected abstract void OnInitialize(Image screen);

    public abstract RectTransform GetScreenChild();
    protected void Finish(bool singlePlayer = false)
    {
        Bus.FinishActivity(singlePlayer ? Identification.PlayerType : PlayerTypes.LocalPlayers, this);
    }

    public abstract UniTask OnFinish();
}
