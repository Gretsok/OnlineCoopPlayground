using System;
using System.Collections.Generic;
using Unity.AppUI.Core;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Movement.IsGroundedControl
{
    public class IsGroundedController : NetworkBehaviour
    {
        [System.Serializable]
        private struct SCheckerData
        {
            public Vector3 RelativePosition;
            public float Depth;
            public LayerMask LayerMask;
        }

        private readonly NetworkVariable<bool> m_isGrounded =
            new NetworkVariable<bool>(false, writePerm: NetworkVariableWritePermission.Owner);
        public bool IsGrounded => m_isGrounded?.Value ?? false;

        public event Action<IsGroundedController> OnGrounded_OwnerCalled;
        public event Action<IsGroundedController> OnGrounded_ServerCalled;
        public event Action<IsGroundedController> OnGrounded_ClientsCalled;
        public event Action<IsGroundedController> OnGroundLeft_OwnerCalled;
        public event Action<IsGroundedController> OnGroundLeft_ServerCalled;
        public event Action<IsGroundedController> OnGroundLeft_ClientsCalled;
        
        public Vector3 LastGroundPoint_OwnerOnly { get; private set; }
        
        
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
            if (!m_relativeSource)
                return;
            
            var isGrounded = false;
            for (int i = 0; i < m_checkersData.Count; i++)
            {
                var data = m_checkersData[i];
                
                var worldPosition = m_relativeSource.TransformPoint(data.RelativePosition);

                if (Physics.Raycast(worldPosition + Vector3.up * 0.05f, Vector3.down, out RaycastHit raycastHit,
                        data.Depth + 0.05f, data.LayerMask))
                {
                    isGrounded = true;
                    LastGroundPoint_OwnerOnly = raycastHit.point;
                    break;
                }
            }

            var previousValue = m_isGrounded.Value;
            
            m_isGrounded.Value = isGrounded;
            
            if (previousValue != isGrounded)
            {
                if (isGrounded)
                {
                    OnGrounded_OwnerCalled?.Invoke(this);
                    NotifyGrounded_ServerRpc();
                    NotifyGrounded_ClientsRpc();
                }
                else
                {
                    OnGroundLeft_OwnerCalled?.Invoke(this);
                    NotifyGroundLeft_ServerRpc();
                    NotifyGroundLeft_ClientsRpc();
                }
            }
        }

        [Rpc(SendTo.Server)]
        private void NotifyGrounded_ServerRpc()
        {
            OnGrounded_ServerCalled?.Invoke(this);
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void NotifyGrounded_ClientsRpc()
        {
            OnGrounded_ClientsCalled?.Invoke(this);
        }
        
        [Rpc(SendTo.Server)]
        private void NotifyGroundLeft_ServerRpc()
        {
            OnGroundLeft_ServerCalled?.Invoke(this);
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void NotifyGroundLeft_ClientsRpc()
        {
            OnGroundLeft_ClientsCalled?.Invoke(this);
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
