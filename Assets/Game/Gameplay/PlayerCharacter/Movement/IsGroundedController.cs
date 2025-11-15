using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Movement
{
    public class IsGroundedController : NetworkBehaviour
    {
        [System.Serializable]
        private struct SCheckerData
        {
            public Vector3 RelativePosition;
            public float Depth;
        }

        private readonly NetworkVariable<bool> m_isGrounded =
            new NetworkVariable<bool>(false, writePerm: NetworkVariableWritePermission.Owner);
        public bool IsGrounded => m_isGrounded?.Value ?? false;

        [SerializeField]
        private List<SCheckerData> m_checkersData = new();

        private Transform m_relativeSource;
        
        public void SetRelativeSource(Transform a_relativeSource)
        {
            m_relativeSource = a_relativeSource;
        }

        private void FixedUpdate()
        {
            if (!IsOwner)
                return;
            
            var isGrounded = false;
            for (int i = 0; i < m_checkersData.Count; i++)
            {
                var data = m_checkersData[i];
                
                var worldPosition = m_relativeSource.TransformPoint(data.RelativePosition);

                if (Physics.Raycast(worldPosition + Vector3.up * 0.05f, Vector3.down, out RaycastHit raycastHit,
                        data.Depth + 0.05f))
                {
                    isGrounded = true;
                    break;
                }
            }
            
            m_isGrounded.Value = isGrounded;
        }

        private void OnDrawGizmos()
        {
            for (int i = 0; i < m_checkersData.Count; i++)
            {
                var data = m_checkersData[i];
                
                var worldPosition = (m_relativeSource ? m_relativeSource : transform).TransformPoint(data.RelativePosition);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(worldPosition + Vector3.up * 0.05f, worldPosition + Vector3.down * (data.Depth));
                
                
            }
        }
    }
}
