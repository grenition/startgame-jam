using System.Collections.Generic;
using Core.Extensions;

namespace Core.UI
{
    public class UIPanelsRegistry
    {
        public IReadOnlyDictionary<string, UIPanel> RegisteredPanels => _registeredPanels;
        
        private Dictionary<string, UIPanel> _registeredPanels = new();

        public void RegisterPanel(UIPanel panel)
        {
            if(panel== null) return;

            _registeredPanels.Set(panel.Path, panel);
        }
        public void UnregisterPanel(UIPanel panel)
        {
            if(panel == null) return;

            _registeredPanels.Delete(panel.Path);
        }
    }
}
