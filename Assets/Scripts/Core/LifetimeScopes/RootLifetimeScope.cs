using UnityHFSM;
using VContainer;
using VContainer.Unity;

namespace Core.LifetimeScopes
{
    public sealed class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            InstallStateMachine(builder);
        }

        private void InstallStateMachine(IContainerBuilder builder)
        {
            var fsmStateMachine = new UnityHFSM.StateMachine();
            
            builder.RegisterInstance(fsmStateMachine);
        }
    }
}
