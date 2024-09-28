using System;
using System.Runtime.Serialization;
using Core.Interactions;
using Core.Networking.NetworkPlayersService;
using UnityEngine;
using VContainer;

namespace Gameplay.Server
{
    [DataContract, Serializable]
    public class LoadServerSceneAction : GameAction
    {
        [DataMember, SerializeField] public string sceneKey;
        
        private INetworkPlayersService _playersService;
        [Inject]
        private void Construct(INetworkPlayersService playersService)
        {
            _playersService = playersService;
        }
        
        public override void Execute()
        {
            var serverPlayer = _playersService.ServerPlayer;
            if(serverPlayer == null || !serverPlayer.IsLocalPlayer) return;

            serverPlayer.LoadServerScene(sceneKey);
        }

        public LoadServerSceneAction() { }
        public LoadServerSceneAction(string sceneKey) => this.sceneKey = sceneKey;
    }
}
