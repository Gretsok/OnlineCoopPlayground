using Game.Lobby.LobbyOverlay.PlayerDisplay.PlayersContextActions;
using Game.SteamIntegration;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Lobby.LobbyOverlay.PlayerDisplay
{
    public class LobbyMemberWidget : MonoBehaviour
    {
        [SerializeField]
        private RawImage m_avatarImage;
        [SerializeField]
        private TMP_Text m_nameText;
        [SerializeField]
        private ContextActionsForAPlayerPopUp m_contextActionWidgetsPopUp;
        public Friend InflatedLobbyMember { get; private set; }

        public void InflateMember(Friend a_lobbyMember)
        {
            InflatedLobbyMember = a_lobbyMember;
            m_nameText.text = a_lobbyMember.Name;
            m_contextActionWidgetsPopUp.SetAssociatedLobbyMember(a_lobbyMember);
            LoadingAndInflatingAvatarFor(a_lobbyMember);
        }

        private async void LoadingAndInflatingAvatarFor(Friend a_lobbyMember)
        {
            var avatarSteamImage = await a_lobbyMember.GetLargeAvatarAsync();
            m_avatarImage.texture = avatarSteamImage.ToTexture2D();
        }
    }
}
