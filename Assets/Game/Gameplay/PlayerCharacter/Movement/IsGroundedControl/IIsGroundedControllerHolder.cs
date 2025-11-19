
using Game.Gameplay.PlayerCharacter.Movement.IsGroundedControl;

namespace Game.Gameplay.PlayerCharacter.Movement
{
    public interface IIsGroundedControllerHolder
    {
        public IsGroundedController IsGroundedController { get; }
    }
}