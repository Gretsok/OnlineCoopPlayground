using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Lobby.LobbyOverlay.PlayerDisplay.PlayersContextActions.PromoteToLobbyLeader
{
    public class PromoteToLobbyLeaderButtonHandler : AContextButtonHandler
    {
        protected override void HandleActionCallback()
        {
            Debug.Log($"Too complicated for now to change lobby leader. We'll see later.");
        }
    }
}
