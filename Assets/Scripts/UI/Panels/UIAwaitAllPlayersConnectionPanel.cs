using Core.Networking.NetworkPlayersService;
using Core.UI;
using UniRx;
using VContainer;

namespace UI.ServerPlayer
{
    public class UIAwaitAllPlayersConnectionPanel : UIPanel
    {
        private INetworkPlayersService _playersService;
        
        [Inject]
        private void Construct(INetworkPlayersService playersService)
        {
            _playersService = playersService;
        }
        
        protected override void Awake()
        {
            base.Awake();
            
            _playersService.AllPlayersConnected
                .Do(value => UpdatePanelState())
                .Subscribe()
                .AddTo(this);

            UpdatePanelState();
        }

        public void UpdatePanelState()
        {
            if (_playersService.AllPlayersConnected.Value)
                Hide();
            else
                Show();
        }
    }
}
