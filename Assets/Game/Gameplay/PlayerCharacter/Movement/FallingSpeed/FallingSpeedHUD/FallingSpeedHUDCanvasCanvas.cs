using Game.Gameplay.LocalHUDContainer;
using TMPro;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Movement.FallingSpeed.FallingSpeedHUD
{
    public class FallingSpeedHUDCanvasCanvas : ALocalHUDCanvas
    {
        [SerializeField]
        private float m_maxFallingSpeed = 5f;

        [SerializeField]
        private GameObject m_container;
        
        [SerializeField]
        private TMP_Text m_fallingSpeedText;

        [SerializeField]
        private RectTransform m_fallingSpeedFiller;
        

        private void Update()
        {
            if (!LocalMotor)
                return;
            
            if (LocalMotor is not IFallingSpeedControllerHolder fallingSpeedControllerHolder)
                return;
            if (LocalMotor is not IIsGroundedControllerHolder isGroundedControllerHolder)
                return;

            var verticalSpeed = fallingSpeedControllerHolder.FallingSpeedController.VerticalSpeed;
            if (isGroundedControllerHolder.IsGroundedController.IsGrounded || verticalSpeed > 0)
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
