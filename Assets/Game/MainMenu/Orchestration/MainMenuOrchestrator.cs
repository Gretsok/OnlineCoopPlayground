using Tools.Orchestration;
using UnityEngine;

namespace Game.MainMenu.Orchestration
{
    public class MainMenuOrchestrator : MonoBehaviour
    {
        #region Singleton
        public static MainMenuOrchestrator Instance { get; private set; }

        private void Awake()
        {
            if (Instance)
                Destroy(Instance.gameObject);
            
            Instance = this;
        }
        #endregion
        
        [SerializeField]
        private OrchestrationStateMachine m_orchestrationStateMachine;

        [Header("States")]
        [SerializeField]
        private OrchestrationState m_connectionScreenState;
        [SerializeField]
        private OrchestrationState m_disconnectionHandlingScreenState;
        [SerializeField]
        private OrchestrationState m_connectingScreenState;
        
        public void GoToConnectionScreen()
        {
            if (!m_connectionScreenState)
            {
                Debug.LogError($"Cannot go to connection screen. No connection screen state available.");
                return;
            }
            
            m_orchestrationStateMachine.SetNextState(m_connectionScreenState);
        }

        public void GoToDisconnectionHandlingScreen()
        {
            if (!m_disconnectionHandlingScreenState)
            {
                Debug.LogError($"Cannot go to disconnection handling screen. No disconnection handling screen state available.");
                return;
            }
            m_orchestrationStateMachine.SetNextState(m_disconnectionHandlingScreenState);
        }

        public void GoToConnectingScreen()
        {
            if (!m_connectingScreenState)
            {
                Debug.LogError($"Cannot go to connecting screen. No connecting screen state available.");
                return;
            }
            m_orchestrationStateMachine.SetNextState(m_connectingScreenState);
        }
    }
}
