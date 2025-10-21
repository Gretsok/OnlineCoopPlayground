using System;
using UnityEngine;
using UnityEngine.Events;

namespace Tools.UIManagement
{
    public class Panel : MonoBehaviour
    {
        public bool IsInitialized { get; private set; } = false;
        public void Init()
        {
            if (IsInitialized)
                return;
            
            gameObject.SetActive(false);
            IsInitialized = true;
        }

        public bool IsShown { get; private set; }
        public event Action<Panel> OnShown;
        [SerializeField]
        private UnityEvent m_onShown = null;
        public void Show()
        {
            if (IsShown)
                return;
            
            gameObject.SetActive(true);
            IsShown = true;

            OnShown?.Invoke(this);
            m_onShown?.Invoke();
        }

        public event Action<Panel> OnHidden;
        [SerializeField]
        private UnityEvent m_onHidden = null;
        public void Hide()
        {
            if (!IsShown)
                return;
            
            gameObject.SetActive(false);
            IsShown = false;

            OnHidden?.Invoke(this);
            m_onHidden?.Invoke();
        }
    }
}
