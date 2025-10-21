using Tools.Orchestration;
using Unity.Netcode;

namespace Game.MainMenu.Orchestration.Rooting
{
    public class RootingStateExtensionHandler : OrchestrationStateExtensionHandler
    {
        protected override void HandleStateEntered(OrchestrationState a_state)
        {
            base.HandleStateEntered(a_state);
            var networkManager = NetworkManager.Singleton;
            
            
            MainMenuOrchestrator.Instance.GoToConnectionScreen();
        }
    }
}
