using UnityEngine;

namespace Game.Gameplay.VehiclesSystem.GenericVehicles.HangGlider
{
    [CreateAssetMenu(fileName = "HangGliderMovementDataAsset", 
        menuName = "Game/Gameplay/VehiclesSystem/GenericVehicles/HangGlider/HangGliderMovementDataAsset", 
        order = 0)]
    public class HangGliderMovementDataAsset : ScriptableObject
    {
        [field: Header("Simplified gravity/portance/etc.. ratio => Down speed when stabilized")]
        [field: SerializeField]
        public float DownSpeedWhenStabilized { get; private set; } = 2f;

        
        [field: Header("Forward force")]
        
        [field: SerializeField]
        public float MinimalForwardSpeed { get; private set; } = 3f;
        [field: SerializeField]
        public float ForwardAccelerationWhenGoingFullDown { get; private set; } = 20f;
        [field: SerializeField]
        public AnimationCurve AccelerationEvolutionBasedOnAngleFromStraightToDown { get; private set; } = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [field: SerializeField]
        public float ForwardDecelerationWhenGoingUp { get; private set; } = 30f;
        [field: SerializeField]
        public AnimationCurve DecelerationEvolutionBasedOnAngleFromStraightToUp { get; private set; } = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        
        [field: Header("Air Resistance")]
        [field: SerializeField]
        public float AirResistance { get; private set; } = 5f;

        [field: Header("Stabilization")]
        [field: SerializeField]
        public float HorizontalStabilizationRatioPerSecond { get; private set; } = 0.1f;

        [field: SerializeField]
        public float VerticalStabilizationRatioPerSecond { get; private set; } = 0.1f;
        
        [field: Header("Yaw based on Roll")]
        [field: SerializeField]
        public float MaxRotationSpeedWhenFullyOnSide { get; private set; } = 70f;
        [field: SerializeField]
        public AnimationCurve RotationSpeedEvolutionBasedOnAngleFromStraightToSide { get; private set; } = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        
        [field: Header("Roll constraint")]
        [field: SerializeField]
        public AnimationCurve RollingCapacityWhenPitching { get; private set; } = AnimationCurve.Linear(0f, 1f, 1f, 4f);
        [field: SerializeField]
        public AnimationCurve RollingVisualRotationEvolution { get; private set; } = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [field: SerializeField]
        public float RollControlRoughness { get; private set; } = 8f;
        
        [field: Header("Pitch constraint")]
        [field: SerializeField]
        public AnimationCurve PitchingVisualRotationEvolution { get; private set; } = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [field: SerializeField]
        public float PitchControlRoughness { get; private set; } = 8f;
        /*[field: SerializeField]
        [field: Tooltip("This is ignored when you try to stabilize the hang glider from a high pitch.")]
        public AnimationCurve PitchingCapacityWhenRolling { get; private set; } = AnimationCurve.Linear(0f, 1f, 1f, 0f);*/
    }
}