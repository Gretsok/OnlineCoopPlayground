using System;
using Game.Gameplay.Controls;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Gameplay.CameraSystem
{
    public class CameraController : MonoBehaviour
    {
        [field: SerializeField]
        public CinemachineCamera CameraAnchor { get; private set; }

        private LocalPlayerController m_localPlayerController;
        
        private void Start()
        {
            m_localPlayerController = LocalPlayerController.Instance;

            m_localPlayerController.OnCharacterAssigned += HandleCharacterAssigned;
            HandleCharacterAssigned(m_localPlayerController);
        }

        private void HandleCharacterAssigned(LocalPlayerController a_localPlayerController)
        {
            CameraAnchor.Target.TrackingTarget = a_localPlayerController.AssignedCharacter?.transform;
        }
    }
}
