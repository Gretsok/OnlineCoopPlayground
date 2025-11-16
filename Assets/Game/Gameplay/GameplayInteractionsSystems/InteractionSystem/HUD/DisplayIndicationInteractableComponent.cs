using UnityEngine;

namespace Game.Gameplay.GameplayInteractionsSystems.InteractionSystem.HUD
{
    [RequireComponent(typeof(Interactable))]
    public class DisplayIndicationInteractableComponent : MonoBehaviour
    {
        [SerializeField]
        private Transform m_indicationWorldAnchorPoint;

        [SerializeField]
        private Vector3 m_worldRelativeOffset;

        [SerializeField]
        private Vector2 m_screenOffset;
        
        private Interactable m_interactable;

        private void Awake()
        {
            m_interactable = GetComponent<Interactable>();
        }

        private void Start()
        {
            m_interactable.OnSightOfLocalCharacterEntered_ClientsCalled +=
                HandleSightOfLocalCharacterEntered_ClientsCalled;
            m_interactable.OnSightOfLocalCharacterLeft_ClientsCalled +=
                HandleSightOfLocalCharacterLeft_ClientsCalled;
        }
        
        private void HandleSightOfLocalCharacterEntered_ClientsCalled(Interactable a_arg1, Interactor a_arg2)
        {
            InteractionIndicationsHUDCanvas.Instance.DisplayUsing(this);
        }
        
        private void HandleSightOfLocalCharacterLeft_ClientsCalled(Interactable a_arg1, Interactor a_arg2)
        {
            InteractionIndicationsHUDCanvas.Instance.Hide();
        }

        public Vector2 GetRequestedViewportPosition()
        {
            var cameraToUse = Camera.main;
            return cameraToUse?.WorldToViewportPoint(
                m_indicationWorldAnchorPoint.TransformPoint(m_worldRelativeOffset)) + m_screenOffset ?? new Vector2(0.5f, 0.5f);
        }
    }
}
