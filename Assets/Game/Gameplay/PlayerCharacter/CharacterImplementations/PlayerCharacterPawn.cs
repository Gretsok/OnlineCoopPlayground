using Game.Gameplay.GameplayInteractionsSystems.EffectsSystem;
using Game.Gameplay.GameplayInteractionsSystems.HealthHandling;
using Game.Gameplay.PlayerCharacter.Animation;
using Game.Gameplay.VehiclesSystem;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.CharacterImplementations
{
    public class PlayerCharacterPawn : NetworkBehaviour,
        IHealthControllerHolder,
        IEffectsControllerHolder,
        IPlayerCharacterAnimationControllerHolder,
        IPlayerCharacterGameDataRetrieverAndInjectorHolder,
        IVehiclePassengerControllerHolder
    {
        [field: SerializeField]
        public HealthController HealthController { get; private set; }
        [field: SerializeField]
        public EffectsController EffectsController { get; private set; }
        [field: SerializeField]
        public PlayerCharacterAnimationController PlayerCharacterAnimationController { get; private set; }
        [field: SerializeField]
        public PlayerCharacterGameDataRetrieverAndInjector PlayerCharacterGameDataRetrieverAndInjector { get;
            private set;
        }
        [field: SerializeField]
        public VehiclePassengerController VehiclePassengerController { get; private set; }
    }
}
