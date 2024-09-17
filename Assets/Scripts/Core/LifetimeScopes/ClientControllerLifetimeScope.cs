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
        builder.RegisterEntryPoint<ClientControllerLifetimeScope>(Lifetime.Singleton).AsSelf();
    }
}
