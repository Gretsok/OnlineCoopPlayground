using Steamworks;
using UnityEngine.EventSystems;

namespace Game.Lobby.LobbyOverlay.PlayerDisplay.PlayersContextActions.SendMessage
{
    public class SendMessageButtonHandler : AContextButtonHandler
    {
        protected override void HandleActionCallback()
        {
            SteamFriends.OpenUserOverlay(ContextActionWidget.AssociatedLobbyMember.Id, "chat");
        }
    }
}
