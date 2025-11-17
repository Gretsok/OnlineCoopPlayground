using Game.Gameplay.LocalHUDContainer;
using TMPro;
using UnityEngine;

namespace Game.Gameplay.GameplayInteractionsSystems.HealthHandling.HUD
{
    public class HealthHUDCanvas : ALocalHUDCanvas
    {
        [SerializeField]
        private TMP_Text m_healthText;
        [SerializeField]
        private RectTransform m_healthBar;

        private void Update()
        {
            if (!LocalMotor)
                return;
            if (LocalMotor is not IHealthControllerHolder healthControllerHolder)
                return;
            
            var healthController = healthControllerHolder.HealthController;
            if (!healthController)
                return;
            
            m_healthText.text = healthController.CurrentHealth.ToString();
            var anchorTemp = m_healthBar.anchorMax;
            anchorTemp.x = (float) healthController.CurrentHealth / healthController.MaxHealth;
            m_healthBar.anchorMax = anchorTemp;
        }
    }
}
