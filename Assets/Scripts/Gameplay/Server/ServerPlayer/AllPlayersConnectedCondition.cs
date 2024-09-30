using Core.Interactions;
using System.Runtime.Serialization;
using VContainer;
using System;

namespace Gameplay.QuestSystem.Conditions
{
    [DataContract, Serializable]
    public class AllPlayersConnectedCondition : GameCondition
    {
        private ControllerNetworkBus _bus;

        [Inject]
        private void Construct(ControllerNetworkBus bus)
        {
            _bus = bus;
        }

        public override bool Execute()
        {
            return _bus.SmallPlayer != null && _bus.BigPlayer != null;
        }
    }
}
