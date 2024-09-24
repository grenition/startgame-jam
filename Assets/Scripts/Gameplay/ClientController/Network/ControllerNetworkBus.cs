using SickDev.DevConsole;
using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using VContainer;

public class ControllerNetworkBus : NetworkBehaviour
{
    [SerializeField] private ActivityInfo[] _infos;
    [SerializeField] private ClientControllerTester _tester;

    private ClientController _controller;
    private ClientIdentification _identification;
    private int _moveDirectionIndex = 0;

    public const string ResourcesPath = "Activities";

    public event Action<string, int[]> SpecialDataTransmitted;
    public event Action<ActivityInfo> ActivityStarted, ActivityFinished;
    public PlayerObject BigPlayer { get; set; }
    public PlayerObject SmallPlayer { get; set; }

    [Inject]
    private void Construct(
        ClientIdentification identification,
        IObjectResolver resolver)
    {
        _identification = identification;
        resolver.Inject(_tester);
    }

    private void Start()
    {
        _infos = Resources.LoadAll<ActivityInfo>(ResourcesPath);
    }

    #region SetClientController
    public void SetClientController(ClientController clientController)
        => _controller ??= clientController;

    public void ResetClientController() => _controller = null;
    #endregion

    #region DevConsoleCommands
    public void ShowTestActivity()
    {
        ShowActivity(_infos[0], _identification.PlayerType);
    }

    public void ShowActivity(int index)
    {
        ShowActivity(_infos[index], _identification.PlayerType);
    }

    public void HideTestActivity()
    {
        HideActivity(_identification.PlayerType);
    }

    public void FinishTestActivity()
    {
        FinishActivity(_identification.PlayerType);
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
        ActivityFinished?.Invoke(_infos[type]);
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

        if (_identification.PlayerType is PlayerTypes.Host)
            return;

        var info = _infos[index];
        _controller.SpawnMiniGame(info);
    }
    #endregion

    #region SpecialData
    public void SendBusMessage(string id, int[] data, PlayerTypes receivers)
    {
        SendBusMessageServerRpc(id, new(data), (int)receivers);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendBusMessageServerRpc(string id, ControllerBusMessage mess, int receivers)
    {
        if(_identification.IsMyType((PlayerTypes)receivers))
        {
            SpecialDataTransmitted?.Invoke(id, mess.Data);
        }
        SendBusMessageClientRpc(id, mess, receivers);
    }

    [ClientRpc(RequireOwnership = false)]
    private void SendBusMessageClientRpc(string id, ControllerBusMessage mess, int receivers)
    {
        if(_identification.IsMyType((PlayerTypes)receivers))
        {
            SpecialDataTransmitted?.Invoke(id, mess.Data);
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

    #region SetMoveDirection
    public void SetMoveDirection(Vector3 direction)
    {
        SetMoveDirectionServerRpc(direction, _moveDirectionIndex++, (int)_identification.PlayerType);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetMoveDirectionServerRpc(Vector3 direction, int index, int type)
    {
        var player = SmallPlayer;
        if((PlayerTypes)type is PlayerTypes.Big)
        {
            player = BigPlayer;
        }

        player?.SetMoveDirection(direction, index);
    }
    #endregion

    #region ActivityStartedInvoke
    public void InvokeActivityStarted(ActivityInfo info)
    {
        int index = _infos.ToList().IndexOf(info);
        InvokeActivityStartedServerRpc(index);
    }

    [ServerRpc(RequireOwnership = false)]
    private void InvokeActivityStartedServerRpc(int index)
    {
        ActivityStarted?.Invoke(_infos[index]);
    }
    #endregion

    public override void OnDestroy()
    {
        _tester.OnDestroy();
        base.OnDestroy();
    }
}