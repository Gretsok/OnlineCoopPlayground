using System;
using Game.Gameplay.CameraSystem;
using Game.Gameplay.PlayerCharacter.Implementations.Default;
using Game.Networking;
using Game.Playground.Controls;
using Tools.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Gameplay.Controls
{
    public class LocalPlayerController : MonoBehaviour
    {
        [field: SerializeField]
        public DefaultLocalPlayerInputProcessor DefaultLocalPlayerInputProcessor { get; private set; }
        public static LocalPlayerController Instance { get; private set; }
        public AbstractConnectedClientObject LocalClient { get; private set; }
        
        private void Awake()
        {
            Instance = this;
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

    }
}
