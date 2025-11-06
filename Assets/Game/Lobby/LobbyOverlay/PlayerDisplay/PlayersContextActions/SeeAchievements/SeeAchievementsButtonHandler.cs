using Steamworks;
using UnityEngine.EventSystems;

namespace Game.Lobby.LobbyOverlay.PlayerDisplay.PlayersContextActions.SeeAchievements
{
    public class SeeAchievementsButtonHandler : AContextButtonHandler
    {
        protected override void HandleActionCallback()
        {
            SteamFriends.OpenUserOverlay(ContextActionWidget.AssociatedLobbyMember.Id, "achievements");
        }
    }
}
