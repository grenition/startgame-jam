using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.UI.Transitions
{
    [RequireComponent(typeof(Animator))]
    public class UIAnimatorPanelTransition : UIPanelTransition
    {
        [SerializeField] private string _closeAnimatorBoolean = "Close";
        
        private bool _hideTransitionEnd = false;
        private Animator _animator;

        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>();
        }

        protected override async UniTask ShowTransition()
        {
            _uiPanel.gameObject.SetActive(true);
            _animator.SetBool(_closeAnimatorBoolean, false);
        }
        protected override async UniTask HideTransition()
        {
            _hideTransitionEnd = false;
            _animator.SetBool(_closeAnimatorBoolean, true);
            
            await UniTask.WaitUntil(() => _hideTransitionEnd);
            
            _uiPanel.gameObject.SetActive(false);
        }

        public void EndHideTransition()
        {
            _hideTransitionEnd = true;
        }
    }
}
