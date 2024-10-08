using Core.Networking.NetworkObjectsFactory;
using Gameplay.Player.GameObject;
using Gameplay.QuestSystem;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace Core.LifetimeScopes
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<FactoryContainerOverrider>().AsSelf();
            builder.RegisterEntryPoint<GamePlayersResolver>().AsSelf();

            var points = FindObjectsOfType<ActivityPoint>();
            foreach (var point in points)
            {
                autoInjectGameObjects.Add(point.gameObject);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            foreach (var interactionPoint in FindObjectsOfType<InteractionPoint>(true))
                Container?.InjectGameObject(interactionPoint.gameObject);
        }
    }
}
