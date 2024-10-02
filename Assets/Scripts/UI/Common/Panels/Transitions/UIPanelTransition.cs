using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.UI.Transitions
{
    [RequireComponent(typeof(UIPanel))]
    public abstract class UIPanelTransition : MonoBehaviour
    {
        protected UIPanel _uiPanel;
        
        protected virtual void Awake()
        {
            _uiPanel = GetComponent<UIPanel>();
            
            _uiPanel.ShowTransition ??= ShowTransition;
            _uiPanel.HideTransition ??= HideTransition;
        }

        protected virtual void OnDestroy()
        {
            if (_uiPanel.ShowTransition == ShowTransition) _uiPanel.ShowTransition = null;
            if (_uiPanel.HideTransition == HideTransition) _uiPanel.HideTransition = null;
        }

        protected abstract UniTask ShowTransition();
        protected abstract UniTask HideTransition();
    }
}
