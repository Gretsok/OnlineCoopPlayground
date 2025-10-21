using System;
using UnityEngine;
using UnityEngine.Events;

namespace Tools.Orchestration
{
    public class OrchestrationState : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent m_onEntered;

        public event Action<OrchestrationState> OnEntered;
        
        [SerializeField]
        private UnityEvent m_onLeft;
        public event Action<OrchestrationState> OnLeft;

        public bool IsRunning { get; private set; } = false;
        
        public void Enter()
        {
            if (IsRunning)
                return;
            
            m_onEntered?.Invoke();
            OnEntered?.Invoke(this);
            
            IsRunning = true;
        }

        public void Leave()
        {
            if (!IsRunning)
                return;
            
            m_onLeft?.Invoke();
            OnLeft?.Invoke(this);
            
            IsRunning = false;
        }
    }
}
