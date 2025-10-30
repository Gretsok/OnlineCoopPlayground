using System;
using System.Collections.Generic;
using Game.Lobby;
using Steamworks;
using UnityEngine;

namespace Game.MainMenu.Orchestration.SteamLobby.PlayerDisplay
{
    public class LobbyMembersWidgetsHandler : MonoBehaviour
    {
        [SerializeField]
        private LobbyMemberWidget m_lobbyMemberWidgetPrefab;
        
        private LobbyManager m_lobbyManager;
        
        private readonly List<LobbyMemberWidget> m_lobbyMemberWidgets = new();

        private void Start()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            
            m_lobbyManager = LobbyManager.Instance;

            var lobbyMembersEnumerator = m_lobbyManager.Lobby.Members.GetEnumerator();

            while (lobbyMembersEnumerator.MoveNext())
            {
                var lobbyMember = lobbyMembersEnumerator.Current;
                
                AddLobbyMember(lobbyMember);
            }

            SteamMatchmaking.OnLobbyMemberJoined += HandleLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave += HandleLobbyMemberLeft;
        }

        private void OnDestroy()
        {
            SteamMatchmaking.OnLobbyMemberJoined -= HandleLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave -= HandleLobbyMemberLeft;
        }

        private void HandleLobbyMemberJoined(Steamworks.Data.Lobby a_arg1, Friend a_arg2)
        {
            AddLobbyMember(a_arg2);
        }
        
        private void HandleLobbyMemberLeft(Steamworks.Data.Lobby a_arg1, Friend a_arg2)
        {
            RemoveLobbyMember(a_arg2);
        }

        public void AddLobbyMember(Friend a_lobbyMember)
        {
            if (m_lobbyMemberWidgets.Exists(a_widget => a_widget.InflatedLobbyMember.Id == a_lobbyMember.Id))
                return;
            
            var newWidget = Instantiate(m_lobbyMemberWidgetPrefab, transform);
            newWidget.InflateMember(a_lobbyMember);
            
            m_lobbyMemberWidgets.Add(newWidget);
        }

        public void RemoveLobbyMember(Friend a_lobbyMember)
        {
            var widget = m_lobbyMemberWidgets.Find(a_widget => a_widget.InflatedLobbyMember.Id == a_lobbyMember.Id);

            if (!widget)
                return;
            
            m_lobbyMemberWidgets.Remove(widget);
            Destroy(widget.gameObject);
        }
    }
}
