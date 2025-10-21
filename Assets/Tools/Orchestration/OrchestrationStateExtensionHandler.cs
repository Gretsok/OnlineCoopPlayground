using UnityEngine;

namespace Tools.Orchestration
{
    public class OrchestrationStateExtensionHandler : MonoBehaviour
    {
        [SerializeField]
        private OrchestrationState m_orchestrationState;

        private void Awake()
        {
            if (!m_orchestrationState)
                m_orchestrationState = GetComponent<OrchestrationState>();
            if (!m_orchestrationState)
                return;

            m_orchestrationState.OnEntered += HandleStateEntered;
            m_orchestrationState.OnLeft += HandleStateLeft;
        }

        private void OnDestroy()
        {
            if (!m_orchestrationState)
                return;
            
            m_orchestrationState.OnEntered -= HandleStateEntered;
            m_orchestrationState.OnLeft -= HandleStateLeft;
        }

        protected virtual void HandleStateEntered(OrchestrationState a_state)
        { }

        protected virtual void HandleStateLeft(OrchestrationState a_state)
        { }

        protected virtual void HandleStateUpdated(OrchestrationState a_state)
        { }
        
        private void Update()
        {
            if (m_orchestrationState)
                HandleStateUpdated(m_orchestrationState);      
        }
    }
}
