using SickDev.DevConsole;
using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using VContainer;

public class ControllerNetworkBus : NetworkBehaviour
{
    [SerializeField] private ActivityInfo[] _infos;

    private ClientController _controller;
    private DevConsole _devConsole;

    [Inject]
    private void Construct(ClientController controller, DevConsole devConsole)
    {
        _controller = controller;
        _devConsole = devConsole;

        devConsole.AddCommand(new(new Action(ShowTestActivity)));
    }

    #region DevConsoleCommands
    public void ShowTestActivity()
    {
        ShowActivity(_infos[0]);
    }
    #endregion

    #region ShowActivity
    public void ShowActivity(ActivityInfo info)
    {
        int index = _infos.ToList().IndexOf(info);
        ShowActivityServerRpc(index);
    }

    [ServerRpc]
    private void ShowActivityServerRpc(int index)
    {
        ShowActivityClientRpc(index);
    }

    [ClientRpc]
    private void ShowActivityClientRpc(int index)
    {
        if(index < 0 || index >= _infos.Length)
        {
            Debug.LogError($"Somwthing went wrong. Can't show activity with index {index}");
            return;
        }

        var info = _infos[index];
        _controller.ShowActivity(info);
    }
    #endregion

    public void HideActivity()
    {

    }

    public void Interact(ActivityPoint point)
    {

    }
}
