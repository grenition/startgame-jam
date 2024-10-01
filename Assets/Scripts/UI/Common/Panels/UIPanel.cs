using System;
using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace Core.UI
{
    public class UIPanel : MonoBehaviour
    {
        public bool IsShown => gameObject.activeSelf;
        public string Path => _path;

        [FoldoutGroup("Events")] public UnityEvent OnShow;
        [FoldoutGroup("Events")] public UnityEvent OnHide;
        
        [SerializeField, BoxGroup("Main")] private string _path;

        private UIPanelsRegistry _uiPanelsRegistry;

        [Inject]
        private void Construct(UIPanelsRegistry uiPanelsRegistry)
        {
            _uiPanelsRegistry = uiPanelsRegistry;
        }
        
        protected virtual void Awake()
        {
            _path ??= gameObject.name;
            _uiPanelsRegistry.RegisterPanel(this);
        }

        protected virtual void OnDestroy()
        {
            _uiPanelsRegistry.UnregisterPanel(this);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            OnShow?.Invoke();
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            OnHide?.Invoke();
        }

        private void OnValidate()
        {
            _path ??= gameObject.name;
        }
    }
}
