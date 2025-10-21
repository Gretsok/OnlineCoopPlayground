using Tools.Orchestration;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Game.MainMenu.Orchestration.Connecting
{
    public class ConnectingToGameHandler : OrchestrationStateExtensionHandler
    {
        protected override void HandleStateEntered(OrchestrationState a_state)
        {
            base.HandleStateEntered(a_state);

            var networkManager = NetworkManager.Singleton;

            if (networkManager.IsServer)
            {
                networkManager.SceneManager.LoadScene("PlaygroundScene", LoadSceneMode.Single);
            }
        }
    }
}
