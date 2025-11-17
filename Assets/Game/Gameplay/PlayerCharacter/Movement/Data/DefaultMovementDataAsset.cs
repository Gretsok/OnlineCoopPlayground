using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Movement.Data
{
    [CreateAssetMenu(fileName = "PlayerCharacterMovementData", menuName = "Game/Playground/DefaultPlayerMotor/MovementData")]
    public class DefaultMovementDataAsset : ScriptableObject
    {
        [field: SerializeField]
        public float MaxMovementSpeed { get; private set; }
        [field: SerializeField]
        public AnimationCurve MaxSpeedFactorAccordingToInput { get; private set; } = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [field: SerializeField]
        public float Acceleration { get; private set; } = 8f;
        [field: SerializeField]
        public float Deceleration { get; private set; } = 15f;
        [field: SerializeField]
        public float JumpVelocity { get; private set; } = 30f;
    }
}
