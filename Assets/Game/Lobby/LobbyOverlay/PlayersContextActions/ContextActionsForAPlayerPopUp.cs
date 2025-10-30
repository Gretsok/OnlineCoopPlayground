using System.Collections.Generic;
using Steamworks;
using UnityEngine;

namespace Game.MainMenu.Orchestration.SteamLobby
{
    public class ContextActionsForAPlayerPopUp : MonoBehaviour
    {
        [SerializeField]
        private List<ContextActionWidget> m_contextActionWidgets;
        
        public Friend ContextualizedMember { get; private set; }
        

        public void ShowContextMenuFor(Friend a_lobbyMember)
        {
            ContextualizedMember = a_lobbyMember;
            m_contextActionWidgets.ForEach(a_widget => a_widget.UpdateWidgetDisplayStateFor(a_lobbyMember));
            gameObject.SetActive(true);
        }

        public void HideContextMenu()
        {
            gameObject.SetActive(false);
            ContextualizedMember = default;
        }
    }
}
