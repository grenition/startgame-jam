using Core.Interactions;
using System;
using System.Runtime.Serialization;
using VContainer;

namespace Gameplay.QuestSystem.Actions
{
    [DataContract, Serializable]
    public class HideActivityAction : GameAction
    {
        private ControllerNetworkBus _bus;

        [Inject]
        private void Construct(ControllerNetworkBus bus)
        {
            _bus = bus;
        }

        public override void Execute()
        {
            _bus.HideActivity(PlayerTypes.All);
        }
    }

}