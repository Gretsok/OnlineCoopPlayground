using Unity.Netcode.Components;

namespace Game.Gameplay.PlayerCharacter.Animation
{
    public class PlayerCharacterNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
