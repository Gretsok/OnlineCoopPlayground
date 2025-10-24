using Game.Playground.PlayerCharacter.Movement.Data;
using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;

namespace Game.Playground.PlayerCharacter.Movement
{
    public class PlayerCharacterMovementController : NetworkBehaviour
    {
        [field: SerializeField]
        public MovementDataAsset MovementDataAsset { get; private set; }

        private Rigidbody m_rigidbody;

        public void SetDependencies(Rigidbody a_rigidbody)
        {
            m_rigidbody = a_rigidbody;
        }

        private readonly NetworkVariable<Vector3> m_directionInput = new NetworkVariable<Vector3>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
        public Vector3 DirectionInput => m_directionInput?.Value ?? Vector3.zero;
        private readonly NetworkVariable<Vector3> m_currentVelocity = new NetworkVariable<Vector3>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
        public Vector3 CurrentVelocity => m_currentVelocity?.Value ?? Vector3.zero;

        public void SetDirectionInput(Vector3 a_directionInput)
        {
            if (!IsSpawned)
                return;
            if (!IsOwner)
                return;
/*
            Debug.Log($"Direction Input: {a_directionInput} | Old direction input : {DirectionInput}");
*/
            m_directionInput.Value = a_directionInput;
        }

        private void Update()
        {
            if (!IsSpawned)
                return;
            if (!IsOwner)
                return;

            if (m_directionInput.Value.sqrMagnitude > 0.3f * 0.3f)
            {
                var maxSpeed = MovementDataAsset.MaxMovementSpeed 
                               * MovementDataAsset.MaxSpeedFactorAccordingToInput.Evaluate(m_directionInput.Value.magnitude);
                m_rigidbody.maxLinearVelocity = maxSpeed;

                var newVelocitySpeed = m_currentVelocity.Value.magnitude + MovementDataAsset.Acceleration * Time.deltaTime;
                if (newVelocitySpeed > maxSpeed)
                {
                    newVelocitySpeed = maxSpeed;
                }

                var newVelocity = m_directionInput.Value.normalized * newVelocitySpeed;
                m_currentVelocity.Value = newVelocity;
                
            }
            else
            {
                var decelerationDelta = MovementDataAsset.Deceleration * Time.deltaTime;
                if (m_currentVelocity.Value.magnitude < decelerationDelta)
                {
                    m_currentVelocity.Value = Vector3.zero;
                }
                else
                {
                    m_currentVelocity.Value -= m_currentVelocity.Value.normalized * decelerationDelta;
                }
            }

            m_rigidbody.linearVelocity = m_currentVelocity.Value + Vector3.down;
            m_rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
