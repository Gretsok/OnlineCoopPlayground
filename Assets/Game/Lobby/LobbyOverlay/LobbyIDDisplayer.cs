using TMPro;
using UnityEngine;

namespace Game.Lobby.LobbyOverlay
{
    public class LobbyIDDisplayer : MonoBehaviour
    {
        public void UpdateLobbyId()
        {
            GetComponent<TMP_Text>().text = LobbyManager.Instance.Lobby.Id.ToString();
        }
    }
}
