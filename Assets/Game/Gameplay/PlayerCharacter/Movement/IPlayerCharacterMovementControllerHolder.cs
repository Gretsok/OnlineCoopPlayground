using Game.Gameplay.PlayerCharacter.Movement;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Movement
{
    public interface IPlayerCharacterMovementControllerHolder
    {
        public PlayerCharacterMovementController MovementController { get; }
    }
}
