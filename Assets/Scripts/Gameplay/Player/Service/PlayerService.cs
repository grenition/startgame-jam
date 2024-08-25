namespace Gameplay.Player.Service
{
    public class PlayerService : IPlayerService
    {
        public PlayerType CurrentPlayerType { get; private set; }

        public void ChoosePlayer(PlayerType playerType)
        {
            CurrentPlayerType = playerType;
        }
    }
}
