using Tools.Orchestration;
using Unity.Netcode;
using UnityEngine;

namespace Game.MainMenu.Orchestration._0_Loading
{
    public class LoadingStateExtensionHandler : OrchestrationStateExtensionHandler
    {
        [SerializeField]
        private NetworkManager m_networkManagerPrefab;

        protected override void HandleStateEntered(OrchestrationState a_state)
        {
            base.HandleStateEntered(a_state);

            if (!NetworkManager.Singleton)
                Instantiate(m_networkManagerPrefab);
            
            MainMenuOrchestrator.Instance.GoToRootineState();
        }
    }
}
