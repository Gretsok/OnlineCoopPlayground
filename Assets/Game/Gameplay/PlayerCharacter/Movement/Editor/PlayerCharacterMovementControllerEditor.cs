using Game.Gameplay.PlayerCharacter.Movement;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Movement.Editor
{
    [UnityEditor.CustomEditor(typeof(PlayerCharacterMovementController))]
    public class PlayerCharacterMovementControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var castedTarget = (PlayerCharacterMovementController)target;
            
            GUILayout.Space(30);
            GUILayout.Label("----- DEBUG INFOS -----");
            GUILayout.Label($"Current velocity: {castedTarget.CurrentPlanarVelocity}");
            GUILayout.Label($"Direction input: {castedTarget.DirectionInput}");
        }
    }
}
