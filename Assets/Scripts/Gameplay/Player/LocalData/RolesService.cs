using Core.Networking.RelayService;
using SickDev.CommandSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using DevConsole = SickDev.DevConsole.DevConsole;

namespace Gameplay.Player.Service
{
    public enum PlayerType
    {
        playerOlder,
        playerYounger
    }
    
    public class RolesService : IInitializable
    {
        public PlayerType CurrentPlayerType { get; private set; }

        private DevConsole _devConsole;
        private IRelayController _relayController;
        
        [Inject]
        private void Construct(DevConsole devConsole, IRelayController relayController)
        {
            _devConsole = devConsole;
            _relayController = relayController;
        }
        
        public void Initialize()
        {
            _devConsole.AddCommand(new FuncCommand<PlayerType, bool>(ChoosePlayerType));
            _relayController.ConnectionPayload = CurrentPlayerType.ToString();
        }
        public bool ChoosePlayerType(PlayerType playerType)
        {
            CurrentPlayerType = playerType;
            _relayController.ConnectionPayload = CurrentPlayerType.ToString();
            Debug.Log($"Selected player type: {playerType}");

            return true;
        }
    }
}
