using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Lobby.LobbyOverlay.PlayerDisplay.PlayersContextActions
{
    [RequireComponent(typeof(ContextActionWidget))]
    public class AContextButtonHandler : MonoBehaviour, ISubmitHandler, IPointerClickHandler
    {
        private ContextActionWidget m_contextActionWidget;
        public ContextActionWidget ContextActionWidget => m_contextActionWidget;

        protected virtual void Awake()
        {
            m_contextActionWidget = GetComponent<ContextActionWidget>();
        }

        public void OnSubmit(BaseEventData a_eventData)
        {
            HandleActionCallback();
        }

        public void OnPointerClick(PointerEventData a_eventData)
        {
            HandleActionCallback();
        }

        protected virtual void HandleActionCallback()
        {
            
        }
    }
}
