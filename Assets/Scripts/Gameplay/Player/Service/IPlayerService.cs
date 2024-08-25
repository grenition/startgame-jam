namespace Gameplay.Player.Service
{
    public enum PlayerType
    {
        monitor,
        playerOlder,
        playerYounger
    }
    
    public interface IPlayerService
    {
        public PlayerType CurrentPlayerType { get; }

        public void ChoosePlayer(PlayerType playerType);
    }
}
