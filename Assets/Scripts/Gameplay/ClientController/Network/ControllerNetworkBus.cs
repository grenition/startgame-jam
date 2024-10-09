using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;

public class ControllerNetworkBus : NetworkBehaviour
{
    [SerializeField] private ClientControllerTester _tester;
    [SerializeField] private string _playersTooFarMessage;
    [SerializeField] private AudioMixer _audioMixer;

    private ClientController _controller;
    private ClientIdentification _identification;
    private int _moveDirectionIndex = 0;
    private ActivityInfo[] _infos;

    private List<BusTask> _allPlayersConnectedTasks = new();
    private int _allPlayersConenctedId = 0;

    public const string ResourcesPath = "Activities";

    public event Action<string, int[]> SpecialDataTransmitted;
    public event Action<ActivityInfo> ActivityStarted;
    public event Action<string> ActivityFinished;
    public event Action<PlayerTypes> InteractedOnServer;

    [field: SerializeField] public bool TestMode { get; private set; }
    public PlayerObject BigPlayer { get; set; }
    public PlayerObject SmallPlayer { get; set; }
    public ClientControllerTester Tester => _tester;
    public NetworkBusLevelMessageReceiver MessageReceiver { get; set; } = null;

    [Inject]
    private void Construct(
        ClientIdentification identification,
        IObjectResolver resolver)
    {
        _identification = identification;
        resolver.Inject(_tester);
        _infos = Resources.LoadAll<ActivityInfo>(ResourcesPath);
    }

    public void ResolveInfos(IObjectResolver resolver)
    {
        foreach(var info in _infos)
        {
            resolver.Inject(info);
        }
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
        FinishActivity(_identification.PlayerType, null);
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
    public void FinishActivity(PlayerTypes type, ActivityStarter activity)
    {
        string output = "";
        if(activity != null)
        {
            output = activity.GetType().ToString();
        }
        FinishActivityServerRpc((int)type, output);
    }

    [ServerRpc(RequireOwnership = false)]
    private void FinishActivityServerRpc(int type, string activityName)
    {
        FinishActivityClientRpc(type, activityName);
    }

    [ClientRpc]
    private void FinishActivityClientRpc(int type, string activityName)
    {
        if(_identification.IsMyType((PlayerTypes)type)) {
            _controller.FinishActivity();
        }
        if(_identification.PlayerType is PlayerTypes.Small)
        {
            AfterFinishActivityServerRpc(activityName);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AfterFinishActivityServerRpc(string activityName)
    {
        ActivityFinished?.Invoke(activityName);
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
        if(SmallPlayer != null && BigPlayer != null &&
            Vector3.Distance(SmallPlayer.transform.position, BigPlayer.transform.position) <= 4f)
        {
            WaitForTeammateClientRpc(index);
        }
        else if(TestMode)
        {
            WaitForTeammateClientRpc(index);
        }
        else
        {
            var player = BigPlayer;
            if (player == null) player = SmallPlayer;
            player?.ShowMessage(_playersTooFarMessage);
            CantWaitForTeammateClientRpc();
        }
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

    [ClientRpc]
    private void CantWaitForTeammateClientRpc()
    {
        _controller?.ResetState();
    }
    #endregion

    #region SpecialData
    public void SendBusMessage(string id, int[] data, PlayerTypes receivers)
    {
        int data0 = data.Length > 0 ? data[0] : 0;
        int data1 = data.Length > 1 ? data[1] : 0;
        int data2 = data.Length > 2 ? data[2] : 0;
        int data3 = data.Length > 3 ? data[3] : 0;
        SendBusMessageServerRpc(id, data0, data1, data2, data3, (int)receivers);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendBusMessageServerRpc(string id, int data0, int data1, int data2, int data3, int receivers)
    {
        if(_identification.IsMyType((PlayerTypes)receivers))
        {
            SpecialDataTransmitted?.Invoke(id, new int[] { data0, data1, data2, data3 });
        }
        SendBusMessageClientRpc(id, data0, data1, data2, data3, receivers);
    }

    [ClientRpc(RequireOwnership = false)]
    private void SendBusMessageClientRpc(string id, int data0, int data1, int data2, int data3, int receivers)
    {
        if(_identification.IsMyType((PlayerTypes)receivers))
        {
            SpecialDataTransmitted?.Invoke(id, new int[] { data0, data1, data2, data3 });
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

    #region InteractOnServerInvoke
    public void InvokeInteractOnServer(PlayerTypes type)
    {
        InvokeInteractServerRpc((int)type);
    }

    [ServerRpc(RequireOwnership = false)]
    private void InvokeInteractServerRpc(int type)
    {
        InteractedOnServer?.Invoke((PlayerTypes)type);
    }
    #endregion

    #region SendMessageToLevelReceiver
    public void SendMessageToLevelMessageReceiver(string id, params int[] data)
    {
        int[] totalData = new int[4];
        if(data != null)
        {
            for (int i = 0; i < Mathf.Min(data.Length, totalData.Length); i++)
            {
                totalData[i] = data[i];
            }
        }
        
        SendMessageToLevelMessageReceiverServerRpc(id, totalData[0], totalData[1], totalData[2], totalData[3]);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendMessageToLevelMessageReceiverServerRpc(string id, int data0, int data1, int data2, int data3)
    {
        MessageReceiver?.OnReceiveMessage(id, data0, data1, data2, data3);
    }
    #endregion

    #region ApplySettings
    public void ApplySettings(int quality, float volume)
    {
        ApplySettingsServerRpc(quality, volume);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ApplySettingsServerRpc(int quality, float volume)
    {
        HideActivityServerRpc((int)PlayerTypes.LocalPlayers);
        QualitySettings.SetQualityLevel(quality);
        _audioMixer.SetFloat(SettingsModelView.MasterVolume, volume);
    }
    #endregion

    #region IsAllPlayersConnected
    public void IsAllPlayersConnected(Action<bool> response)
    {
        var task = new BusTask(response, _allPlayersConenctedId++);
        _allPlayersConnectedTasks.Add(task);
        IsAllPlayersConnectedServerRpc(task.Id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void IsAllPlayersConnectedServerRpc(int id)
    {
        bool value = BigPlayer != null && SmallPlayer != null;
        IsAllPlayersConnectedClientRpc(value, id);
    }

    [ClientRpc]
    private void IsAllPlayersConnectedClientRpc(bool value, int id)
    {
        foreach(var task in _allPlayersConnectedTasks)
        {
            if(task.Id == id)
            {
                task.Response?.Invoke(value);
                _allPlayersConnectedTasks.Remove(task);
                break;
            }
        }
    }
    #endregion

    public override void OnDestroy()
    {
        _tester.OnDestroy();
        base.OnDestroy();
    }
}

public class BusTask
{
    public Action<bool> Response;
    public int Id;

    public BusTask(Action<bool> response, int id)
    {
        Response = response;
        Id = id;
    }
}