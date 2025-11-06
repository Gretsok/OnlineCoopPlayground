using Steamworks;
using UnityEngine.EventSystems;

namespace Game.Lobby.LobbyOverlay.PlayerDisplay.PlayersContextActions.SeeStats
{
    public class SeeStatsButtonHandler : AContextButtonHandler
    {
        protected override void HandleActionCallback()
        {
            SteamFriends.OpenUserOverlay(ContextActionWidget.AssociatedLobbyMember.Id, "stats");
        }
    }
}
