using Unity.Netcode;

namespace Core.Interactions
{
    public enum InteractionSendTo
    {
        Server,
        BigPlayer,
        SmallPlayer,
        Players,
        ServerAndBigPlayer,
        ServerAndSmallPlayer,
        Everyone
    }
    public interface IInteractionService
    {
        public void DoAction(GameAction action, InteractionSendTo sendTo = InteractionSendTo.Everyone);
        public bool GetCondition(GameCondition condition);
    }
}
