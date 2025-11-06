using Steamworks;
using UnityEngine.EventSystems;

namespace Game.Lobby.LobbyOverlay.PlayerDisplay.PlayersContextActions.AddToFriends
{
    public class AddToFriendsButtonHandler : AContextButtonHandler
    {
        protected override void HandleActionCallback()
        {
            SteamFriends.OpenUserOverlay(ContextActionWidget.AssociatedLobbyMember.Id, "friendadd");
        }
    }
}
