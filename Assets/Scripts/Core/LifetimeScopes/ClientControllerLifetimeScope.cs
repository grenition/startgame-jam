using Core.Networking.NetworkObjectsFactory;
using VContainer;
using VContainer.Unity;
using UnityEngine;

public class ClientControllerLifetimeScope : LifetimeScope
{
    [SerializeField] private ClientController _controller;
    [SerializeField] private Inventory _inventory;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_controller);
        builder.RegisterInstance(_inventory);
        builder.RegisterEntryPoint<ObjectsFactory>(Lifetime.Scoped).As<IObjectsFactory>();
        
        Debug.Log($"{nameof(ClientControllerLifetimeScope)} builded!");
    }
}
