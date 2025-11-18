using Game.Gameplay.PlayerCharacter.MotorImplementations.Default;
using Game.Gameplay.VehiclesSystem;
using Game.Networking;
using UnityEngine;

namespace Game.Gameplay.LocalControls
{
    public class LocalPlayerController : MonoBehaviour
    {
        [field: SerializeField]
        public DefaultLocalPlayerInputProcessor DefaultLocalPlayerInputProcessor { get; private set; }
        [field: SerializeField]
        public VehicleLocalPlayerInputProcessor VehicleLocalPlayerInputProcessor { get; private set; }
        public ALocalPlayerInputProcessor CurrentLocalPlayerInputProcessor { get; private set; }
        
        public static LocalPlayerController Instance { get; private set; }
        public AbstractConnectedClientObject LocalClient { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SwitchToProcessor(DefaultLocalPlayerInputProcessor);
        }

        public void AssignClient(AbstractConnectedClientObject a_client)
        {
            if (!a_client.IsOwner)
            {
                Debug.LogError($"Trying to assign a character that is not owned locally.");
                return;
            }
            
            LocalClient = a_client;
        }

        public void SwitchToProcessor(ALocalPlayerInputProcessor a_localPlayerInputProcessor)
        {
            if (CurrentLocalPlayerInputProcessor)
                CurrentLocalPlayerInputProcessor.Deactivate();
            CurrentLocalPlayerInputProcessor = a_localPlayerInputProcessor;
            if (CurrentLocalPlayerInputProcessor)
                CurrentLocalPlayerInputProcessor.Activate();
        }
    }
}
