using Core.Networking.RelayService;
using Core.StateMachine.Controllers;
using VContainer;
using VContainer.Unity;

namespace Core.LifetimeScopes
{
    public sealed class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            InstallStateMachines(builder);
            InstallNetworking(builder);
        }
        
        private void InstallStateMachines(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<RootStateMachine>();
        }
        private void InstallNetworking(IContainerBuilder builder)
        {
            builder.Register<RelayService>(Lifetime.Singleton).As<IRelayService>();
        }
    }
}
