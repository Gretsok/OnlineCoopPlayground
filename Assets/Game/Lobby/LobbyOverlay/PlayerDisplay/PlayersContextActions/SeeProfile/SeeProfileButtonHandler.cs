using Steamworks;
using UnityEngine.EventSystems;

namespace Game.Lobby.LobbyOverlay.PlayerDisplay.PlayersContextActions.SeeProfile
{
    public class SeeProfileButtonHandler : AContextButtonHandler
    {
        protected override void HandleActionCallback()
        {
            //SteamFriends.OpenOverlay($"steamid:{ContextActionWidget.AssociatedLobbyMember.Id}");
            //SteamFriends.OpenUserOverlay(ContextActionWidget.AssociatedLobbyMember.Id, "steamid");
            
            var url = $"https://steamcommunity.com/profiles/{ContextActionWidget.AssociatedLobbyMember.Id}";
            SteamFriends.OpenWebOverlay(url);
        }
    }
}
