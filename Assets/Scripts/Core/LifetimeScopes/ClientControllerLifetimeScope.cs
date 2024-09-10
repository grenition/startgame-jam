using VContainer;
using VContainer.Unity;
using UnityEngine;

public class ClientControllerLifetimeScope : LifetimeScope
{
    [SerializeField] private ClientController _controller;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_controller);
    }
}
