using UnityEngine;

namespace Game.Playground.PlayerCharacter.Movement
{
    public interface IPlayerCharacterMovementControllerHolder
    {
        public PlayerCharacterMovementController MovementController { get; }
    }
}
