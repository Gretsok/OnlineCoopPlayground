using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Gameplay.GameplayInteractionsSystems.InteractionSystem
{
    public class InteractablesDetector : MonoBehaviour
    {
        [SerializeField]
        private Vector3 m_centerOffset = new Vector3(0f, 0f, 1f);
        [SerializeField]
        private float m_radius = 3f;
        [SerializeField]
        private float m_forwardAngleToPrioritize = 90f;
        [SerializeField]
        private LayerMask m_layerMask;
        
        public Transform Source { get; private set; }

        private Collider[] m_detectedColliders = new Collider[10];
        
        public void SetSource(Transform a_source)
        {
            Source = a_source;
        }
        
        public Interactable GetInteractableToInteractWith()
        {
            if (!Source)
                return null;
            
            // We get all the interactables in sphere
            List<Interactable> interactables = new List<Interactable>();
            var size = Physics.OverlapSphereNonAlloc((Source ? Source : transform).TransformPoint(m_centerOffset), m_radius, m_detectedColliders, m_layerMask, QueryTriggerInteraction.Collide);
            for (int i = 0; i < size; i++)
            {
                var interactable = m_detectedColliders[i].GetComponent<IInteractableHolder>()?.LinkedInteractable;
                if (interactable)
                    interactables.Add(interactable);
            }

            // We try to only keep the interactables in front of us.
            var filteredInteractables = FilterOutNonForwardInteractables(interactables);

            // If there is none, we drop the filtered result.
            if (filteredInteractables.Count == 0)
                filteredInteractables = interactables;
            
            // We return the closest interactable
            return GetClosestInteractable(filteredInteractables);
        }

        public List<Interactable> FilterOutNonForwardInteractables(List<Interactable> a_interactables)
        {
            List<Interactable> outputInteractables = a_interactables.ToList();
            
            var forward = Source.forward;
            
            for (int i = 0; i < a_interactables.Count; i++)
            {
                var interactable = a_interactables[i];
                var directionToInteractable = (interactable.transform.position - Source.position).normalized;
                
                if (Vector3.Angle(forward, directionToInteractable) <= m_forwardAngleToPrioritize)
                    outputInteractables.Add(interactable);
            }

            return outputInteractables;
        }

        public Interactable GetClosestInteractable(List<Interactable> a_interactables)
        {
            List<Interactable> outputInteractables = a_interactables.ToList();

            Interactable closestInteractable = null;
            float closestDistance = float.PositiveInfinity;

            for (int i = 0; i < a_interactables.Count; i++)
            {
                var interactable = a_interactables[i];

                var distanceToInteractable = Vector3.Distance(interactable.transform.position, Source.position);
                
                if (!closestInteractable
                    || distanceToInteractable < closestDistance)
                {
                    closestInteractable = interactable;
                    closestDistance = distanceToInteractable;
                }
            }
            
            return closestInteractable;
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere((Source ? Source : transform).TransformPoint(m_centerOffset), m_radius);
        }
    }
}
