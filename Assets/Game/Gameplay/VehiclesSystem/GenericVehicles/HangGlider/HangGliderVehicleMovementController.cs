using Unity.Netcode;
using UnityEngine;
using System.Linq;
using Tools.Utils;
using Unity.Netcode.Components;


namespace Game.Gameplay.VehiclesSystem.GenericVehicles.HangGlider
{
    public class HangGliderVehicleMovementController : NetworkBehaviour
    {
        private NetworkObject m_model;
        
        [SerializeField]
        private Vector2 m_sensivities = new Vector2(10f, 5f);
        private VehicleSeatsController m_seatsController;
        private Rigidbody m_rigidbody;

        public void SetDependencies(VehicleSeatsController a_seatsController, Rigidbody a_rigidbody,
            NetworkObject a_model)
        {
            m_seatsController = a_seatsController;
            m_rigidbody = a_rigidbody;
            m_model = a_model;
        }
        
        [SerializeField]
        private HangGliderMovementDataAsset m_movementDataAsset;
        
        public enum EState
        {
            Grounded = 0,
            Flight = 1
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
            m_state.Value = EState.Grounded;
        }

        public void ActivateFlightMode()
        {
            m_state.Value = EState.Flight;
        }

        public void ActivateGroundMode()
        {
            m_state.Value = EState.Grounded;
        }
        
        private void FixedUpdate()
        {
            if (!IsServer)
                return;
            if (!m_seatsController)
                return;

            if (m_seatsController.SeatInfos_ServerOnly.Count(a_seat => a_seat.Passenger != null) == 0)
            {
                if (m_state.Value != EState.Grounded)
                    m_state.Value = EState.Grounded;
            }
            else
            {
                if (m_state.Value != EState.Flight)
                    m_state.Value = EState.Flight;
            }

            if (m_state.Value == EState.Grounded)
            {
                UpdateGroundedMode();
            }
            else
            {
                UpdateFlightMode();
            }
        }

        private void UpdateGroundedMode()
        {
            
        }
        
        private void UpdateFlightMode()
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