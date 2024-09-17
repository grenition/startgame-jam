using Core.Networking.NetworkObjectsFactory;
using VContainer;
using VContainer.Unity;

namespace Core.LifetimeScopes
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<FactoryContainerOverrider>().AsSelf();
        }
    }
}
