using System.Linq;
using Game.Networking;
using Unity.Netcode;
using UnityEngine.EventSystems;

namespace Game.Lobby.LobbyOverlay.PlayerDisplay.PlayersContextActions.KickPlayer
{
    public class KickPlayerButtonHandler : AContextButtonHandler
    {
        protected override void HandleActionCallback()
        {
            var networkManager = NetworkManager.Singleton;
            networkManager.DisconnectClient(
                networkManager.ConnectedClients.First(
                    a_clientPair => 
                        a_clientPair.Value.PlayerObject.GetComponent<AbstractConnectedClientObject>().SteamId 
                        == ContextActionWidget.AssociatedLobbyMember.Id).Key,
                "Player kicked by the lobby owner.");
        }
    }
}
