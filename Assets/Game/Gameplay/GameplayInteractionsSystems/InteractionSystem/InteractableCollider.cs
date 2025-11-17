using UnityEngine;

namespace Game.Gameplay.GameplayInteractionsSystems.InteractionSystem
{
    public class InteractableCollider : MonoBehaviour, IInteractableHolder
    {
        [field: SerializeField]
        public Interactable LinkedInteractable { get; private set; }
    }
}
