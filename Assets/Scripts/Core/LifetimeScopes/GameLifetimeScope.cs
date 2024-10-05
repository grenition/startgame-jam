using Core.Networking.NetworkObjectsFactory;
using Gameplay.Player.GameObject;
using Gameplay.QuestSystem;
using VContainer;
using VContainer.Unity;

namespace Core.LifetimeScopes
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<FactoryContainerOverrider>().AsSelf();
            builder.RegisterEntryPoint<GamePlayersResolver>().AsSelf();
        }

        protected override void Awake()
        {
            base.Awake();

            foreach (var interactionPoint in FindObjectsOfType<InteractionPoint>(true))
                Container?.InjectGameObject(interactionPoint.gameObject);
        }
    }
}
