using Unity.Netcode;
using UnityEngine;
using System.Linq;
using Game.Gameplay.PlayerCharacter.Movement.IsGroundedControl;
using Tools.Utils;
using Unity.Netcode.Components;


namespace Game.Gameplay.VehiclesSystem.GenericVehicles.HangGlider
{
    public class HangGliderVehicleMovementController : NetworkBehaviour
    {
        private NetworkObject m_model;

        [Header("Grounded mode")]
        [SerializeField]
        private float m_heightFromTheGroundWhenGrounded = 1.8f;

        [SerializeField]
        private float m_groundedHeightSetterRoughness = 18f;
        [SerializeField]
        private float m_groundedRotationSetterRoughness = 18f;

        [SerializeField]
        private float m_groundedWalkMaxSpeed = 7f;
        [SerializeField]
        private float m_groundedWalkAccelerationPerUser = 3f;
        [SerializeField]
        private float m_groundedRotationSpeedPerUser = 30f;

        [SerializeField]
        private float m_groundedNoInputDeceleration = 1f;
        [SerializeField]
        private float m_groundedWalkDecelerationPerUser = 3f;
        
        [Header("In fly mode")]
        [SerializeField]
        private Vector2 m_sensivities = new Vector2(10f, 5f);
        private VehicleSeatsController m_seatsController;
        private Rigidbody m_rigidbody;

        private IsGroundedController m_frontIsGroundedController;
        
        public void SetDependencies(VehicleSeatsController a_seatsController, Rigidbody a_rigidbody,
            NetworkObject a_model,
            IsGroundedController a_frontIsGroundedController)
        {
            m_seatsController = a_seatsController;
            m_rigidbody = a_rigidbody;
            m_model = a_model;
            
            m_frontIsGroundedController = a_frontIsGroundedController;
        }
        
        [SerializeField]
        private HangGliderMovementDataAsset m_movementDataAsset;
        
        public enum EState
        {
            Empty = 0,
            Flight = 1,
            Grounded = 2,
        }
        
        private readonly NetworkVariable<EState> m_state = new();

        private readonly NetworkVariable<float> m_horizontalValue = new(0.5f);
        public float HorizontalValue => m_horizontalValue.Value;
        private readonly NetworkVariable<float> m_verticalValue = new(0.5f);
        public float VerticalValue => m_verticalValue.Value;
        private readonly NetworkVariable<float> m_forwardSpeed = new();

        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();
            
            // Let it as grounded so it automatically switch to empty mode to be properly initialized. We could use a better mean later.
            m_state.Value = EState.Grounded;
        }

        public void ActivateEmptyMode()
        {
            if (m_state.Value == EState.Empty)
                return;
            m_state.Value = EState.Empty;

            m_rigidbody.useGravity = true;
            m_rigidbody.excludeLayers &= ~LayerMask.GetMask("PlayerCharacter");
        }
        
        public void ActivateFlightMode()
        {
            if (m_state.Value == EState.Flight)
                return;
            m_state.Value = EState.Flight;
            
            m_rigidbody.useGravity = false;
            m_rigidbody.excludeLayers &= LayerMask.GetMask("PlayerCharacter");
        }

        public void ActivateGroundMode()
        {
            if (m_state.Value == EState.Grounded)
                return;
            m_state.Value = EState.Grounded;
            
            m_rigidbody.useGravity = false;
            m_rigidbody.excludeLayers &= LayerMask.GetMask("PlayerCharacter");
        }

        
        private void FixedUpdate()
        {
            if (!IsServer)
                return;
            if (!m_seatsController)
                return;
            
            UpdateCurrentState();
            
            if (m_state.Value == EState.Grounded)
            {
                UpdateGroundedMode_ServerOnly();
            }
            else if (m_state.Value == EState.Flight)
            {
                UpdateFlightMode_ServerOnly();
            }
            else
            {
                UpdateEmptyMode_ServerOnly();
            }
        }


        private void UpdateCurrentState()
        {
            if (m_seatsController.SeatInfos_ServerOnly.Count(a_seat => a_seat.Passenger != null) == 0)
            {
                ActivateEmptyMode();
            }
            else if (m_frontIsGroundedController.IsGrounded)
            {
                ActivateGroundMode();
            }
            else
            {
                ActivateFlightMode();
            }
        }

        
        private void UpdateEmptyMode_ServerOnly()
        {
            
        }
        
        private void UpdateGroundedMode_ServerOnly()
        {
            // It is owner only, but the server should be the owner.
            var groundPoint = m_frontIsGroundedController.LastGroundPoint_OwnerOnly;
            
            var targetPoint = m_rigidbody.transform.position.Flatten(groundPoint.y + m_heightFromTheGroundWhenGrounded);
            
            m_rigidbody.transform.position = Vector3.Lerp(m_rigidbody.transform.position, targetPoint, m_groundedHeightSetterRoughness * Time.deltaTime);
            var flattenForward = m_rigidbody.transform.forward.Flatten();
            if (flattenForward.sqrMagnitude == 0)
                flattenForward = m_rigidbody.transform.up;
            m_rigidbody.transform.rotation = Quaternion.Lerp(m_rigidbody.transform.rotation, Quaternion.LookRotation(m_rigidbody.transform.forward.Flatten(), Vector3.up), m_groundedRotationSetterRoughness * Time.deltaTime);
            m_model.transform.localRotation = Quaternion.Lerp(m_model.transform.localRotation, Quaternion.identity, m_groundedRotationSetterRoughness * Time.deltaTime);

            Vector2 totalDirectionInput = default;
            for (int i = 0; i < m_seatsController.MaximumSeats; ++i)
            {
                var seat = m_seatsController.SeatInfos_ServerOnly[i];

                if (!seat.Passenger)
                    continue;

                var directionInput = seat.Passenger.VehiclePassengerController.DirectionInput;

                totalDirectionInput += new Vector2(directionInput.x, directionInput.y);
                 
            }
            
            m_rigidbody.transform.rotation = Quaternion.RotateTowards(m_rigidbody.transform.rotation, Quaternion.LookRotation(m_rigidbody.transform.right, m_rigidbody.transform.up),
                Time.deltaTime * m_groundedRotationSpeedPerUser * totalDirectionInput.x);

            var forwardSpeed = m_rigidbody.linearVelocity.magnitude;
            
            if (totalDirectionInput.y > 0f)
            {
                forwardSpeed +=
                    m_groundedWalkAccelerationPerUser * Time.deltaTime;
            }
            else
            {
                var deltaDeceleration = -m_groundedNoInputDeceleration * Time.deltaTime;
                if (totalDirectionInput.y < 0f)
                {
                    deltaDeceleration -= 
                        -totalDirectionInput.y * m_groundedWalkDecelerationPerUser * Time.deltaTime;
                }

                if (deltaDeceleration >= m_rigidbody.linearVelocity.magnitude)
                    m_rigidbody.linearVelocity = Vector3.zero;
                forwardSpeed += deltaDeceleration;
            }
            
            
            if (forwardSpeed > m_groundedWalkMaxSpeed)
            {
                forwardSpeed = m_groundedWalkMaxSpeed;
            }
            
            m_rigidbody.linearVelocity = m_rigidbody.transform.forward * forwardSpeed;
        }
        
        private void UpdateFlightMode_ServerOnly()
        {
            // We get the forward speed by subtracting down speed from velocity.
            m_forwardSpeed.Value = new Vector3(m_rigidbody.linearVelocity.x, Mathf.Min(0f, m_rigidbody.linearVelocity.y + m_movementDataAsset.DownSpeedWhenStabilized), m_rigidbody.linearVelocity.z).magnitude;
            
            for (int i = 0; i < m_seatsController.MaximumSeats; ++i)
            {
                var seat = m_seatsController.SeatInfos_ServerOnly[i];

                if (!seat.Passenger)
                    continue;

                var directionInput = seat.Passenger.VehiclePassengerController.DirectionInput;
                
                var targetHorizontalValue = Mathf.Clamp01(m_horizontalValue.Value + directionInput.x * m_sensivities.x * Time.deltaTime);
                var targetVerticalValue = Mathf.Clamp01(m_verticalValue.Value + directionInput.y * m_sensivities.y * Time.deltaTime);
                
                if (targetHorizontalValue > 0.5f && directionInput.x < 0.3f)
                {
                    targetHorizontalValue -=
                        Mathf.Min(m_movementDataAsset.HorizontalStabilizationRatioPerSecond * Time.deltaTime,
                            targetHorizontalValue - 0.5f);
                }
                else if (targetHorizontalValue < 0.5f && directionInput.x > -0.3f)
                {
                    targetHorizontalValue += 
                        Mathf.Min(m_movementDataAsset.HorizontalStabilizationRatioPerSecond * Time.deltaTime, 
                            0.5f - targetHorizontalValue);
                }

                if (targetVerticalValue > 0.5f && directionInput.y < 0.3f)
                {
                    targetVerticalValue -=
                        Mathf.Min(m_movementDataAsset.VerticalStabilizationRatioPerSecond * Time.deltaTime,
                            targetVerticalValue - 0.5f);
                }
                else if (targetVerticalValue < 0.5f && directionInput.y > -0.3f)
                {
                    targetVerticalValue += 
                        Mathf.Min(m_movementDataAsset.VerticalStabilizationRatioPerSecond * Time.deltaTime, 
                            0.5f - targetVerticalValue);
                }
                
                m_horizontalValue.Value = Mathf.Lerp(m_horizontalValue.Value, targetHorizontalValue, m_movementDataAsset.RollControlRoughness * Time.deltaTime);
                m_verticalValue.Value = Mathf.Lerp(m_verticalValue.Value, targetVerticalValue, m_movementDataAsset.PitchControlRoughness * Time.deltaTime);
                

                if (m_horizontalValue.Value > 0.5f)
                {
                    m_model.transform.localRotation = Quaternion.Slerp(
                        Quaternion.LookRotation(Vector3.forward, Vector3.up),
                        Quaternion.Lerp(
                            Quaternion.LookRotation(Vector3.forward, Vector3.up),
                            Quaternion.LookRotation(Vector3.forward, Vector3.right),
                        0.4f),
                         m_movementDataAsset.RollingVisualRotationEvolution.Evaluate((m_horizontalValue.Value - 0.5f) * 2f));
                }
                else
                {
                    
                    m_model.transform.localRotation = Quaternion.Slerp(
                        Quaternion.Lerp(
                            Quaternion.LookRotation(Vector3.forward, Vector3.up),
                            Quaternion.LookRotation(Vector3.forward, Vector3.left),
                            0.4f),
                        Quaternion.LookRotation(Vector3.forward, Vector3.up),
                        1f - m_movementDataAsset.RollingVisualRotationEvolution.Evaluate(1f - m_horizontalValue.Value * 2f));
                }

                var planarDirection = m_rigidbody.transform.forward.Flatten().normalized;
                if (m_verticalValue.Value > 0.5f)
                {
                    m_rigidbody.transform.rotation = Quaternion.Slerp(
                        Quaternion.LookRotation(planarDirection, Vector3.up),
                        Quaternion.Lerp(
                            Quaternion.LookRotation(planarDirection, Vector3.up),
                            Quaternion.LookRotation(Vector3.down, planarDirection),
                            0.8f),
                        m_movementDataAsset.PitchingVisualRotationEvolution.Evaluate((m_verticalValue.Value - 0.5f) * 2f));
                }
                else
                {
                    m_rigidbody.transform.rotation = Quaternion.Slerp(
                        Quaternion.Lerp(
                            Quaternion.LookRotation(planarDirection, Vector3.up),
                            Quaternion.LookRotation(Vector3.up, -planarDirection),
                            0.8f),
                        Quaternion.LookRotation(planarDirection, Vector3.up),
                        1f - m_movementDataAsset.PitchingVisualRotationEvolution.Evaluate(1f - m_verticalValue.Value * 2f));
                }
            }

            var rotationWorkingValue = (m_horizontalValue.Value - 0.5f) * 2f;
            var rotationSpeed = m_movementDataAsset.MaxRotationSpeedWhenFullyOnSide *
                                m_movementDataAsset.RotationSpeedEvolutionBasedOnAngleFromStraightToSide.Evaluate(
                                    Mathf.Abs(rotationWorkingValue));
            m_rigidbody.transform.Rotate(Vector3.up, Mathf.Sign(rotationWorkingValue) * rotationSpeed * Time.deltaTime);
            
            
            var linearVelocity = m_model.transform.forward * Mathf.Max(m_forwardSpeed.Value, m_movementDataAsset.MinimalForwardSpeed);
            linearVelocity = ApplyForwardVelocity(linearVelocity, Mathf.Abs((m_verticalValue.Value - 0.5f) * 2f));
            linearVelocity = ApplyAirResistance(linearVelocity);
            
            m_forwardSpeed.Value = linearVelocity.magnitude;
            // We apply the down speed here so it is not keep between frames.
            m_rigidbody.linearVelocity = linearVelocity - m_movementDataAsset.DownSpeedWhenStabilized * Vector3.up;
            Debug.Log($"Speed: {m_rigidbody.linearVelocity.magnitude} | Forward value: {m_forwardSpeed.Value}");
        }
        
        private Vector3 ApplyForwardVelocity(Vector3 a_linearVelocity, float a_pitchingRatio)
        {
            float acceleration = 0f;
            if (m_rigidbody.transform.forward.y > 0)
            {
                // Going Up
                
                acceleration = -m_movementDataAsset.DecelerationEvolutionBasedOnAngleFromStraightToUp.Evaluate(a_pitchingRatio) 
                               * m_movementDataAsset.ForwardDecelerationWhenGoingUp;
            }
            else
            {
                // Going Down
                acceleration = m_movementDataAsset.AccelerationEvolutionBasedOnAngleFromStraightToDown.Evaluate(a_pitchingRatio) 
                               * m_movementDataAsset.ForwardAccelerationWhenGoingFullDown;
            }
            Debug.Log($"HangGlider acceleration: {acceleration}");
            a_linearVelocity += a_linearVelocity.normalized * (acceleration * Time.deltaTime);
            return a_linearVelocity;
        }

        private Vector3 ApplyAirResistance(Vector3 a_linearVelocity)
        {
            Debug.Log($"Air resisting velocity: {(m_movementDataAsset.AirResistance * Time.deltaTime)} | is {m_movementDataAsset.AirResistance} per second." +
                      $"\n Velocity was {a_linearVelocity} now will be {a_linearVelocity - a_linearVelocity.normalized * (m_movementDataAsset.AirResistance * Time.deltaTime)}");
            a_linearVelocity -= a_linearVelocity.normalized * (m_movementDataAsset.AirResistance * Time.deltaTime);

            return a_linearVelocity;
        }
    }
}