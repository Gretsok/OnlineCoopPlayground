using Steamworks;
using UnityEngine.EventSystems;

namespace Game.Lobby.LobbyOverlay.PlayerDisplay.PlayersContextActions.RemoveFromFriends
{
    public class RemoveFromFriendsButtonHandler : AContextButtonHandler
    {
        protected override void HandleActionCallback()
        {
            SteamFriends.OpenUserOverlay(ContextActionWidget.AssociatedLobbyMember.Id, "friendremove");
        }
    }
}
