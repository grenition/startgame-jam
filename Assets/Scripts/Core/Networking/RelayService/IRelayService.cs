namespace Core.Networking.RelayService
{
    public interface IRelayService
    {
        public void CreateRelay();
        public void JoinRelay(string joinCode);
    }
}
