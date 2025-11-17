using Game.Gameplay.LocalHUDContainer;
using UnityEngine;

namespace Game.Gameplay.GameplayInteractionsSystems.InteractionSystem.HUD
{
    public class InteractionIndicationsHUDCanvas : ALocalHUDCanvas
    {
        public static InteractionIndicationsHUDCanvas Instance { get; private set; }
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            
            m_indicationWidget.gameObject.SetActive(false);
        }
        
        [SerializeField]
        private RectTransform m_indicationWidget;

        private DisplayIndicationInteractableComponent m_registeredInteractableComponent;

        public void DisplayUsing(DisplayIndicationInteractableComponent a_interactableComponent)
        {
            m_registeredInteractableComponent = a_interactableComponent;
            var viewportPosition = m_registeredInteractableComponent.GetRequestedViewportPosition();
            m_indicationWidget.anchorMax = viewportPosition;
            m_indicationWidget.anchorMin = viewportPosition;
            m_indicationWidget.gameObject.SetActive(true);
        }

        public void Hide()
        {
            m_registeredInteractableComponent = null;
            m_indicationWidget.gameObject.SetActive(false);
        }

        // We update the position in fixed update to avoid stutters
        // due to the character and camera movement also being in the FixedUpdate
        private void FixedUpdate()
        {
            if (!m_registeredInteractableComponent)
                return;

            var viewportPosition = m_registeredInteractableComponent.GetRequestedViewportPosition();
            m_indicationWidget.anchorMax = viewportPosition;
            m_indicationWidget.anchorMin = viewportPosition;
        }
    }
}
