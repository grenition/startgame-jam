using Unity.Netcode;
using VContainer;

namespace Core.Interactions
{
    public class InteractionService : IInteractionService
    {
        private IObjectResolver _objectResolver;
        
        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }
        
        public void DoAction(GameAction action, InteractionSendTo sendTo = InteractionSendTo.Everyone)
        {
            if(action == null) return;
            _objectResolver.Inject(action);
            action.Execute();
        }
        public bool GetCondition(GameCondition condition)
        {
            if (condition == null) return false;
            _objectResolver.Inject(condition);
            return condition.Execute();
        }
    }
}
