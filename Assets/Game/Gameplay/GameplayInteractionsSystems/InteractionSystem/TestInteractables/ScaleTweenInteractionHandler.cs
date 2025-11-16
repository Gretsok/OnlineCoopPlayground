using DG.Tweening;
using UnityEngine;

namespace Game.Gameplay.GameplayInteractionsSystems.InteractionSystem.TestInteractables
{
    public class ScaleTweenInteractionHandler : MonoBehaviour
    {
        public void HandleInteraction()
        {
            transform.DOKill();
            transform.localScale = Vector3.one;
            transform.DOPunchScale(Vector3.one * 0.2f, 0.5f);
        }
    }
}
