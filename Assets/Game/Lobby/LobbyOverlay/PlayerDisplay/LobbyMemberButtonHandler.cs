using System.Linq;
using Game.Lobby.LobbyOverlay.PlayerDisplay.PlayersContextActions;
using Tools.UIManagement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Lobby.LobbyOverlay.PlayerDisplay
{
    public class LobbyMemberButtonHandler : MonoBehaviour,
        ICrossPanelRequester,
        ISelectHandler, IDeselectHandler
    {
        [SerializeField]
        private ContextActionsForAPlayerPopUp m_contextPopUp;
        
        private void Awake()
        {
            OnDeselect(null);
        }
        
        public void OnSelect(BaseEventData a_eventData)
        {
            m_contextPopUp.StartRequest(this);
        }

        public void OnDeselect(BaseEventData a_eventData)
        {
            m_contextPopUp.StopRequest(this);
        }
    }
}
