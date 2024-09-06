namespace Gameplay.Player.Service
{
    public enum PlayerType
    {
        monitor,
        playerOlder,
        playerYounger
    }
    
    public interface ILocalDataService
    {
        public PlayerType CurrentPlayerType { get; }

        public void ChoosePlayerType(PlayerType playerType);
    }
}
