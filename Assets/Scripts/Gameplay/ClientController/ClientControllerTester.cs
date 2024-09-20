using Core.Networking.NetworkObjectsFactory;
using Cysharp.Threading.Tasks;
using SickDev.DevConsole;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

[Serializable]
public class ClientControllerTester
{
    [SerializeField] private GameObject _clientControllerPrefab;

    private ControllerNetworkBus _bus;
    private DevConsole _devConsole;
    private ClientIdentification _identification;
    private IObjectsFactory _factory;

    private readonly List<SickDev.CommandSystem.Command> _commands = new();

    [Inject]
    private void Construct(
        ControllerNetworkBus bus,
        DevConsole console,
        ClientIdentification identification,
        IObjectsFactory factory)
    {
        _bus = bus;
        _devConsole = console;
        _identification = identification;
        _factory = factory;

        AddCommand(new(new Action(_bus.ShowTestActivity)));
        AddCommand(new(new Action(_bus.HideTestActivity)));
        AddCommand(new(new Action(_bus.FinishTestActivity)));
        AddCommand(new(new Action<int>(SetPlayerType)));
        AddCommand(new(new Action(StartClientControllerTesting)));
    }

    public void SetPlayerType(int type)
    {
        _identification.ForceSetPlayerType((PlayerTypes)type);
    }

    public void StartClientControllerTesting()
    {
        StartClientControllerTestingAsync();
    }

    private async UniTask StartClientControllerTestingAsync()
    {
        SetPlayerType(1);
        await UniTask.Yield();
        _factory.SpawnLocalObject(_clientControllerPrefab);
        await UniTask.Yield();
        _bus.ShowTestActivity();
    }

    public void AddCommand(SickDev.CommandSystem.Command command)
    {
        _commands.Add(command);
        _devConsole.AddCommand(command);
    }

    public void OnDestroy()
    {
        foreach(var command in _commands)
        {
            _devConsole.RemoveCommand(command);
        }
        _commands.Clear();
    }
}
