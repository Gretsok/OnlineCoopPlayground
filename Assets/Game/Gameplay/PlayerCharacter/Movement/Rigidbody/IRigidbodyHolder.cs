using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Movement
{
    public interface IRigidbodyHolder
    {
        public Rigidbody Rigidbody { get; }
    }
}
