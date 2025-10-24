using Unity.Netcode.Components;

namespace Game.Playground.PlayerCharacter.Animation
{
    public class PlayerCharacterNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
