using System;
using Alchemy.Inspector;
using Core.Interactions;
using UnityEngine;
using VContainer;

namespace Debugging
{
    [Serializable]
    public class TestAction : GameAction
    {
        public string valueText = "text text text";
        public int valueNumber = 12;
        
        public override void Execute()
        {
            Debug.Log(this);
        }

        public override string ToString()
        {
            return base.ToString() + $" {nameof(valueText)}:{valueText} {nameof(valueNumber)}:{valueNumber}";
        }
    }
    public class TestActions : MonoBehaviour
    {
        private IInteractionService _interactionService;
        
        [Inject]
        private void Construct(IInteractionService interactionService)
        {
            _interactionService = interactionService;
        }
        
        [Button]
        private void TestEveryone()
        {
            _interactionService.DoAction(new TestAction(), InteractionSendTo.Everyone);
        }
        
        [Button]
        private void TestServerOnly()
        {
            _interactionService.DoAction(new TestAction(), InteractionSendTo.Server);
        }
        
        [Button]
        private void TestPlayersOnly()
        {
            _interactionService.DoAction(new TestAction(), InteractionSendTo.Players);
        }
        
        [Button]
        private void TestBigPlayerAndServer()
        {
            _interactionService.DoAction(new TestAction(), InteractionSendTo.ServerAndBigPlayer);
        }

        [Button]
        private void TestSmallPlayerAndServer()
        {
            _interactionService.DoAction(new TestAction(), InteractionSendTo.ServerAndSmallPlayer);
        }
        
        [Button]
        private void TestBigPlayer()
        {
            _interactionService.DoAction(new TestAction(), InteractionSendTo.BigPlayer);
        }
        
        [Button]
        private void TestSmallPlayer()
        {
            _interactionService.DoAction(new TestAction(), InteractionSendTo.SmallPlayer);
        }
    }
}
