using Core.Networking.NetworkPlayersService;
using Core.Serialization;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Core.Interactions
{
    public class NetworkInteractionsService : NetworkBehaviour, IInteractionService
    {
        private IObjectResolver _objectResolver;
        private IDataSerializer _dataSerializer;
        private INetworkPlayersService _playersService;
        
        [Inject]
        private void Construct(
            IObjectResolver objectResolver,
            IDataSerializer dataSerializer,
            INetworkPlayersService playersService)
        {
            _objectResolver = objectResolver;
            _dataSerializer = dataSerializer;
            _playersService = playersService;
        }
        

        #region DoAction
        
        public void DoAction(GameAction action, InteractionSendTo sendTo = InteractionSendTo.Everyone)
        {
            if(action == null) return;

            var serializedAction = _dataSerializer.Serialize(action);
            DoActionServerRpc(serializedAction, sendTo);
        }

        [Rpc(SendTo.Server)]
        private void DoActionServerRpc(string serializedAction, InteractionSendTo sendTo)
        {
            var action = _dataSerializer.Deserialize<GameAction>(serializedAction);
            if(action == null) return;

            switch (sendTo)
            {
                case InteractionSendTo.Server:
                    InjectAndExecuteAction(action);
                    break;
                
                case InteractionSendTo.Players:
                    DoActionRpc(serializedAction, RpcTarget.NotServer);
                    break;
                
                case InteractionSendTo.BigPlayer:
                    if (_playersService.BigPlayer != null)
                        DoActionRpc(serializedAction, RpcTarget.Single(_playersService.BigPlayer.OwnerClientId, RpcTargetUse.Temp));
                    break;
                
                case InteractionSendTo.SmallPlayer:
                    if (_playersService.SmallPlayer != null)
                        DoActionRpc(serializedAction, RpcTarget.Single(_playersService.SmallPlayer.OwnerClientId, RpcTargetUse.Temp));
                    break;
                
                case InteractionSendTo.ServerAndBigPlayer:
                    if (_playersService.BigPlayer != null)
                        DoActionRpc(serializedAction, RpcTarget.Single(_playersService.BigPlayer.OwnerClientId, RpcTargetUse.Temp));
                    InjectAndExecuteAction(action);
                    break;
                
                case InteractionSendTo.ServerAndSmallPlayer:
                    if (_playersService.SmallPlayer != null)
                        DoActionRpc(serializedAction, RpcTarget.Single(_playersService.SmallPlayer.OwnerClientId, RpcTargetUse.Temp));
                    InjectAndExecuteAction(action);
                    break;
                
                case InteractionSendTo.Everyone:
                    InjectAndExecuteAction(action);
                    DoActionRpc(serializedAction, RpcTarget.NotServer);
                    break;
            }

        }
        
        [Rpc(SendTo.SpecifiedInParams)]
        public void DoActionRpc(string serializedAction, RpcParams rpcParams)
        {
            var action = _dataSerializer.Deserialize<GameAction>(serializedAction);
            InjectAndExecuteAction(action);
        }

        private void InjectAndExecuteAction(GameAction action)
        {
            if(action == null) return;
            
            _objectResolver.Inject(action);
            action.Execute();
        }

        #endregion
        
        public bool GetCondition(GameCondition condition)
        {
            if (condition == null) return false;
            _objectResolver.Inject(condition);
            return condition.Execute();
        }
    }
}
