using UnityEditor;
using UnityEngine;

namespace Game.Lobby.Editor
{
    [CustomEditor(typeof(LobbyManager))]
    public class LobbyManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var castedTarget = (LobbyManager)target;
            
            GUILayout.Label($"Is in lobby: {castedTarget.IsInLobby}");
            
            if (!castedTarget.IsInLobby)
                return;
            
            GUILayout.Label($"Lobby ID: {castedTarget.Lobby.Id}");
            GUILayout.Label($"Owner is: {castedTarget.Lobby.Owner.ToString()}");
            GUILayout.Label("Lobby members: ");
            
            var memberEnumerator = castedTarget.Lobby.Members.GetEnumerator();

            while (memberEnumerator.MoveNext())
            {
                var member = memberEnumerator.Current;
                GUILayout.Label(member.ToString());
            }
            
            memberEnumerator.Dispose();
        }
    }
}
