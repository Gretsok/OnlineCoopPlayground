using Game.SteamIntegration;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.Orchestration.SteamLobby.PlayerDisplay
{
    public class LobbyMemberWidget : MonoBehaviour
    {
        [SerializeField]
        private RawImage m_avatarImage;
        [SerializeField]
        private TMP_Text m_nameText;

        public Friend InflatedLobbyMember { get; private set; }
        
        public void InflateMember(Friend a_lobbyMember)
        {
            InflatedLobbyMember = a_lobbyMember;
            m_nameText.text = a_lobbyMember.Name;
            LoadingAndInflatingAvatarFor(a_lobbyMember);
        }

        private async void LoadingAndInflatingAvatarFor(Friend a_lobbyMember)
        {
            var avatarSteamImage = await a_lobbyMember.GetLargeAvatarAsync();
            m_avatarImage.texture = avatarSteamImage.ToTexture2D();
        }
    }
}
