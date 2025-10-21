using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tools.Orchestration
{
    public class OrchestrationStateMachine : MonoBehaviour
    {
        #region Run Handling
        public bool IsRunning { get; private set; }

        public event Action<OrchestrationStateMachine> OnStartedRunning;
        [SerializeField]
        private UnityEvent m_onStartedRunning;
        public void StartRunning()
        {
            if (IsRunning)
                return;
            
            m_orchestrationQueue.Clear();
            
            CurrentState = m_defaultState;
            CurrentState?.Enter();
            
            IsRunning = true;
            
            OnStartedRunning?.Invoke(this);
            m_onStartedRunning?.Invoke();
        }

        public event Action<OrchestrationStateMachine> OnStoppedRunning;
        [SerializeField]
        private UnityEvent m_onStoppedRunning;
        public void StopRunning()
        {
            if (!IsRunning)
                return;
            
            m_orchestrationQueue.Clear();
            
            CurrentState?.Leave();
            
            IsRunning = false;
            
            OnStoppedRunning?.Invoke(this);
            m_onStoppedRunning?.Invoke();
        }

        private void Start()
        {
            if (m_automaticallyStartRunning)
                StartRunning();
        }

        [SerializeField]
        private bool m_automaticallyStartRunning = true;
        [SerializeField]
        private OrchestrationState m_defaultState;
        #endregion
        
        public OrchestrationState CurrentState { get; private set; }
        
        private readonly Queue<OrchestrationState> m_orchestrationQueue = new();

        public void QueueState(OrchestrationState a_nextState)
        {
            m_orchestrationQueue.Enqueue(a_nextState);
        }

        public void SetNextState(OrchestrationState a_nextState)
        {
            m_orchestrationQueue.Clear();
            m_orchestrationQueue.Enqueue(a_nextState);
        }
        
        public event Action<OrchestrationStateMachine, OrchestrationState, OrchestrationState> OnStateSwitched;
        private void SwitchToState(OrchestrationState a_newState)
        {
            var oldState = CurrentState;
            CurrentState?.Leave();
            CurrentState = a_newState;
            CurrentState?.Enter();
            
            OnStateSwitched?.Invoke(this, oldState, a_newState);
        }

        private void Update()
        {
            if (!IsRunning)
                return;
            
            if (m_orchestrationQueue.Count == 0)
                return;
            
            var newState = m_orchestrationQueue.Dequeue();
            SwitchToState(newState);
        }
    }
}
