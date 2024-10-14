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
    [SerializeField] private string _playersTooFarMessage, _settingsChangedMessage;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private GameObject[] _markers;

    private ClientController _controller;
    private Inventory _inventory;
    private ClientIdentification _identification;
    private CompletedTasks _tasks;
    private int _moveDirectionIndex = 0;
    private ActivityInfo[] _infos;

    private List<BusTask<Action<bool>>> _allPlayersConnectedTasks = new();
    private List<BusTask<Action<int, float>>> _getSettingsTasks = new();
    private List<BusTask<Action<bool>>> _hasItemInAll = new();
    private int _allPlayersConenctedId = 0, _getSettingsId = 0, _hasItemInAllId = 0;

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
    public ActivityInfo BigPlayerActivity { get; private set; } = null;
    public ActivityInfo SmallPlayerActivity { get; private set; } = null;
    public InteractionMarker Marker { get; private set; } = null;

    [Inject]
    private void Construct(
        ClientIdentification identification,
        IObjectResolver resolver,
        CompletedTasks tasks)
    {
        _identification = identification;
        _tasks = tasks;
        resolver.Inject(_tester);
        _infos = Resources.LoadAll<ActivityInfo>(ResourcesPath);
        Marker = new(_markers);
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

    public void SetInventory(Inventory inventory) => _inventory ??= inventory;

    public void ResetInventory() => _inventory = null;
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

    #region Markers
    public void ShowMarker(Vector3 pos, ActivityInfo info)
    {
        int index = _infos.ToList().IndexOf(info);
        if(index >= 0 && index < _infos.Length)
        {
            ShowMarkerServerRpc(pos, index);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShowMarkerServerRpc(Vector3 pos, int index)
    {
        var info = _infos[index];
        Marker.Mark(pos + Vector3.up * info.MarkerYOffset, info);
    }

    public void HideMarker(ActivityInfo info)
    {
        int index = _infos.ToList().IndexOf(info);
        if (index >= 0 && index < _infos.Length)
        {
            HideMarkerServerRpc(index);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void HideMarkerServerRpc(int index)
    {
        var info = _infos[index];
        Marker.DeMark(info);
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
        if (_identification.PlayerType is PlayerTypes.Small) SmallPlayerActivity = null;
        else BigPlayerActivity = null;

        FinishActivityServerRpc((int)type, output, (int)_identification.PlayerType);
    }

    [ServerRpc(RequireOwnership = false)]
    private void FinishActivityServerRpc(int type, string activityName, int senderType)
    {
        FinishActivityClientRpc(type, activityName, senderType);
    }

    [ClientRpc]
    private void FinishActivityClientRpc(int type, string activityName, int senderType)
    {
        if ((PlayerTypes)senderType is PlayerTypes.Small) SmallPlayerActivity = null;
        else BigPlayerActivity = null;

        if (_identification.IsMyType((PlayerTypes)type)) {
            SmallPlayerActivity = null;
            BigPlayerActivity = null;

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
        InvokeActivityStartedServerRpc(index, (int)_identification.PlayerType);
    }

    [ServerRpc(RequireOwnership = false)]
    private void InvokeActivityStartedServerRpc(int index, int type)
    {
        var info = _infos[index];
        if ((PlayerTypes)type is PlayerTypes.Big) BigPlayerActivity = info;
        else SmallPlayerActivity = info;
        ActivityStarted?.Invoke(_infos[index]);
        InvokeActivityStartedClientRpc(index, type);
    }

    [ClientRpc]
    private void InvokeActivityStartedClientRpc(int index, int type)
    {
        var info = _infos[index];
        if ((PlayerTypes)type is PlayerTypes.Big) BigPlayerActivity = info;
        else SmallPlayerActivity = info;
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

    #region GetSettings
    public void GetSettings(Action<int, float> callback)
    {
        var task = new BusTask<Action<int, float>>(callback, _getSettingsId++);
        _getSettingsTasks.Add(task);
        GetSettingsServerRpc(task.Id, (int)_identification.PlayerType);
    }

    [ServerRpc(RequireOwnership = false)]
    private void GetSettingsServerRpc(int id, int type)
    {
        int quality = QualitySettings.GetQualityLevel();
        _audioMixer.GetFloat(SettingsModelView.MasterVolume, out var volume);
        GetSettingsClientRpc(type, id, quality, volume);
    }

    [ClientRpc]
    private void GetSettingsClientRpc(int type, int id, int quality, float volume)
    {
        if (_identification.IsMyType((PlayerTypes)type))
        {
            foreach(var task in _getSettingsTasks)
            {
                if(task.Id == id)
                {
                    task.Response?.Invoke(quality, volume);
                    break;
                }
            }
        }
    }
    #endregion

    #region ApplySettings
    public void ApplySettings(int quality, float volume)
    {
        ApplySettingsServerRpc(quality, volume, (int)_identification.PlayerType);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ApplySettingsServerRpc(int quality, float volume, int type)
    {
        QualitySettings.SetQualityLevel(quality);
        _audioMixer.SetFloat(SettingsModelView.MasterVolume, volume);
        ApplySettingsClientRpc(type);
    }

    [ClientRpc]
    private void ApplySettingsClientRpc(int type)
    {
        if (_identification.IsMyType((PlayerTypes)type) ||
            _identification.PlayerType is PlayerTypes.Host)
            return;

        _controller?.ShowMessage(_settingsChangedMessage);
    }
    #endregion

    #region IsAllPlayersConnected
    public void IsAllPlayersConnected(Action<bool> response)
    {
        var task = new BusTask<Action<bool>>(response, _allPlayersConenctedId++);
        _allPlayersConnectedTasks.Add(task);
        IsAllPlayersConnectedServerRpc(task.Id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void IsAllPlayersConnectedServerRpc(int id)
    {
        bool value = BigPlayer != null && SmallPlayer != null;
        if (TestMode)
            value = true;
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

    #region AddTask
    public void AddTask(ActivityInfo info)
    {
        var index = _infos.ToList().IndexOf(info);
        AddTaskServerRpc(index);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddTaskServerRpc(int index)
    {
        if(index >= 0 && index < _infos.Length)
        {
            var info = _infos[index];
            if(!_tasks.Tasks.Contains(info))
            {
                _tasks.Tasks.Add(info);
            }
            AddTaskClientRpc(index);
        }
    }

    [ClientRpc]
    private void AddTaskClientRpc(int index)
    {
        var info = _infos[index];
        if(!_tasks.Tasks.Contains(info))
        {
            _tasks.Tasks.Add(info);
        }
    }
    #endregion

    #region FinishGame
    public void FinishGame()
    {
        Marker.DeMarkAll();
        FinishGameClientRpc();
    }

    [ClientRpc]
    private void FinishGameClientRpc()
    {
        _controller?.OnFinishGame();
    }
    #endregion

    #region HasItemInAlls
    public void HasItemInAlls(string name, Action<bool> response)
    {
        var task = new BusTask<Action<bool>>(response, _hasItemInAllId++);
        _hasItemInAll.Add(task);
        HasItemInAllsServerRpc(name, (int)_identification.PlayerType, task.Id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HasItemInAllsServerRpc(string name, int type, int id)
    {
        HasItemInAllsClientRpc(name, type, id);
    }

    [ClientRpc]
    private void HasItemInAllsClientRpc(string name, int type, int id)
    {
        if(!_identification.IsMyType((PlayerTypes)type) &&
            _identification.PlayerType is not PlayerTypes.Host)
        {
            var val = _inventory.HasItemByName(name);
            HasItemInAllsResponseServerRpc(type, id, val);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void HasItemInAllsResponseServerRpc(int type, int id, bool value)
    {
        HasItemInAllResponseClientRpc(type, id, value);
    }

    [ClientRpc]
    private void HasItemInAllResponseClientRpc(int type, int id, bool value)
    {
        if(_identification.IsMyType((PlayerTypes)type))
        {
            foreach(var task in _hasItemInAll)
            {
                if (task.Id == id)
                {
                    task.Response?.Invoke(value);
                    _hasItemInAll.Remove(task);
                    break;
                }
            }
        }
    }
    #endregion

    #region FinishOnlyOnePlayerActivity
    public void FinishOnlyOnePlayerActivity(ActivityInfo info)
    {
        var index = _infos.ToList().IndexOf(info);
        if(index >= 0 && index < _infos.Length)
        {
            FinishOnlyOnePlayerActivityServerRpc(index, (int)_identification.PlayerType);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void FinishOnlyOnePlayerActivityServerRpc(int index, int type)
    {
        FinishOnlyOnePlayerActivityClientRpc(index, type);
    }

    [ClientRpc]
    private void FinishOnlyOnePlayerActivityClientRpc(int index, int type)
    {
        if(_identification.IsMyType((PlayerTypes)type) ||
            _identification.PlayerType is PlayerTypes.Host)
        {
            return;
        }

        _controller?.FinishOnlyOnePlayerActivity(_infos[index]);
    }
    #endregion

    public override void OnDestroy()
    {
        _tester.OnDestroy();
        base.OnDestroy();
    }
}

public class BusTask<T>
{
    public T Response;
    public int Id;

    public BusTask(T response, int id)
    {
        Response = response;
        Id = id;
    }
}