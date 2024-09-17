using Core.Networking.NetworkObjectsFactory;
using Cysharp.Threading.Tasks;
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
    private IObjectsFactory _factory;
    private ClientIdentification _identification;

    private SickDev.CommandSystem.Command _showTestActivity, _hideActivity, _finishActivity,
        _setPlayerType;

    public event Action<string, ControllerBusMessage> SpecialDataTransmitted;

    [Inject]
    private void Construct(
        DevConsole devConsole, 
        IObjectsFactory factory, 
        ClientIdentification identification)
    {
        _devConsole = devConsole;
        _factory = factory;
        _identification = identification;

        _showTestActivity = new(new Action(ShowTestActivity));
        _hideActivity = new(new Action(HideTestActivity));
        _finishActivity = new(new Action(FinishTestActivity));
        _setPlayerType = new(new Action<int>(SetPlayerType));
        devConsole.AddCommand(_showTestActivity);
        devConsole.AddCommand(_hideActivity);
        devConsole.AddCommand(_finishActivity);
        devConsole.AddCommand(_setPlayerType);
    }

    #region DevConsoleCommands
    public void ShowTestActivity()
    {
        ShowActivity(_infos[0], _identification.PlayerType);
    }

    public void HideTestActivity()
    {
        HideActivity(_identification.PlayerType);
    }

    public void FinishTestActivity()
    {
        FinishActivity(_identification.PlayerType);
    }

    public void SetPlayerType(int type)
    {
        _identification.ForceSetPlayerType((PlayerTypes)type);
    }
    #endregion

    #region ShowActivity
    public void ShowActivity(ActivityInfo info, PlayerTypes type)
    {
        int index = _infos.ToList().IndexOf(info);
        ShowActivityServerRpc(index, (int)type);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShowActivityServerRpc(int index, int type)
    {
        ShowActivityClientRpc(index, type);
    }

    [ClientRpc]
    private void ShowActivityClientRpc(int index, int type)
    {
        if(!_identification.IsMyType((PlayerTypes)type))
        {
            return;
        }

        if(index < 0 || index >= _infos.Length)
        {
            Debug.LogError($"Something went wrong. Can't show activity with index {index}");
            return;
        }

        var info = _infos[index];
        _controller?.ShowActivity(info);
    }
    #endregion

    #region HideActivity
    public void HideActivity(PlayerTypes type)
    {
        HideActivityServerRpc((int)type);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HideActivityServerRpc(int type)
    {
        HideActivityClientRpc(type);
    }

    [ClientRpc]
    private void HideActivityClientRpc(int type)
    {
        if(_identification.IsMyType((PlayerTypes)type))
        {
            _controller?.HideActivity();
        }
    }
    #endregion

    #region FinishActivity
    public void FinishActivity(PlayerTypes type)
    {
        FinishActivityServerRpc((int)type);
    }

    [ServerRpc(RequireOwnership = false)]
    private void FinishActivityServerRpc(int type)
    {
        FinishActivityClientRpc(type);
    }

    [ClientRpc]
    private void FinishActivityClientRpc(int type)
    {
        if(_identification.IsMyType((PlayerTypes)type)) {
            _controller.FinishActivity();
        }
    }
    #endregion

    public void Interact(ActivityPoint point)
    {

    }

    public void SetClientController(ClientController clientController)
        => _controller ??= clientController;

    public void ResetClientController() => _controller = null;

    #region WaitForTeammate
    public void WaitForTeammateForMiniGame(ActivityInfo info)
    {
        int index = _infos.ToList().IndexOf(info);
        WaitForTeammateServerRpc(index);
    }

    [ServerRpc(RequireOwnership = false)]
    private void WaitForTeammateServerRpc(int index)
    {
        //TODO: Check distance between players
        WaitForTeammateClientRpc(index);
    }

    [ClientRpc]
    private void WaitForTeammateClientRpc(int index)
    {
        if(index < 0 || index >= _infos.Length)
        {
            Debug.LogError($"Error: can't find activityInfo with index {index}");
            return;
        }

        var info = _infos[index];
        _controller.SpawnMiniGame(info);
    }
    #endregion

    #region SpecialData
    public void SpecialDataTransmit(string id, ControllerBusMessage data, PlayerTypes receivers)
    {
        SpecialDataTransmitServerRpc(id, data, (int)receivers);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpecialDataTransmitServerRpc(string id, ControllerBusMessage data, int receivers)
    {
        if(_identification.IsMyType((PlayerTypes)receivers))
        {
            SpecialDataTransmitted?.Invoke(id, data);
        }
        SpecialDataTransmitClientRpc(id, data, receivers);
    }

    [ClientRpc]
    private void SpecialDataTransmitClientRpc(string id, ControllerBusMessage data, int receivers)
    {
        if(_identification.IsMyType((PlayerTypes)receivers)) {
            SpecialDataTransmitted?.Invoke(id, data);
        }
    }
    #endregion

    #region GiveInventoryItem
    public void GiveInventoryItem(InventoryItem item, PlayerTypes receiver)
    {
        GiveInventoryItemServerRpc(item.Id, (int)receiver);
    }

    [ServerRpc(RequireOwnership = false)]
    private void GiveInventoryItemServerRpc(int id, int type)
    {
        GiveInventoryItemClientRpc(id, type);
    }

    [ClientRpc]
    private void GiveInventoryItemClientRpc(int id, int type)
    {
        if(!_identification.IsMyType((PlayerTypes)type))
        {
            return;
        }

        _controller.InventoryModel.AddItem(_controller.InventoryModel.AllItems[id]);
    }
    #endregion

    public override void OnDestroy()
    {
        base.OnDestroy();
        _devConsole.RemoveCommand(_showTestActivity);
        _devConsole.RemoveCommand(_hideActivity);
        _devConsole.RemoveCommand(_finishActivity);
        _devConsole.RemoveCommand(_setPlayerType);
    }
}