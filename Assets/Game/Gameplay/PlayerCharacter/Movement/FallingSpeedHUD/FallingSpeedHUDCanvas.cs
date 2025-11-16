using System.Collections;
using Game.Gameplay.Controls;
using TMPro;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Movement.FallingSpeedHUD
{
    public class FallingSpeedHUDCanvas : MonoBehaviour
    {
        [SerializeField]
        private float m_maxFallingSpeed = 5f;

        [SerializeField]
        private GameObject m_container;
        
        [SerializeField]
        private TMP_Text m_fallingSpeedText;

        [SerializeField]
        private RectTransform m_fallingSpeedFiller;

        private PlayerCharacter m_localCharacter;
        
        IEnumerator Start()
        {
            var localPlayerController = LocalPlayerController.Instance;
            yield return new WaitUntil(() => localPlayerController.AssignedCharacter != null);
            localPlayerController.OnCharacterAssigned += HandleNewCharacterAssigned;
            HandleNewCharacterAssigned(localPlayerController);
        }

        private void HandleNewCharacterAssigned(LocalPlayerController a_localPlayerController)
        {
            m_localCharacter = a_localPlayerController.AssignedCharacter;
        }

        private void Update()
        {
            if (!m_localCharacter)
                return;

            var verticalSpeed = m_localCharacter.MovementController.VerticalSpeed;
            if (m_localCharacter.IsGroundedController.IsGrounded || verticalSpeed > 0)
            {
                m_container.SetActive(false);
                return;
            }
            m_container.SetActive(true);
            
            
            m_fallingSpeedText.text = $"{verticalSpeed:0} m/s";
            var anchorTemp = m_fallingSpeedFiller.anchorMin;
            anchorTemp.y = 1f - Mathf.Clamp01(-verticalSpeed / m_maxFallingSpeed);
            m_fallingSpeedFiller.anchorMin = anchorTemp;
        }
    }
}
