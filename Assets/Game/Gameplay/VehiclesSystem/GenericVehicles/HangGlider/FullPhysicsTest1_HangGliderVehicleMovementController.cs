using System;
using System.Linq;
using Tools.Utils;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.VehiclesSystem.GenericVehicles.HangGlider
{
    public class FullPhysicsTest1_HangGliderVehicleMovementController : NetworkBehaviour
    {
        [SerializeField]
        private Vector2 m_sensivities = new Vector2(10f, 5f);
        private VehicleSeatsController m_seatsController;
        private Rigidbody m_rigidbody;

        public void SetDependencies(VehicleSeatsController a_seatsController, Rigidbody a_rigidbody)
        {
            m_seatsController = a_seatsController;
            m_rigidbody = a_rigidbody;
        }
        
        [SerializeField]
        private HangGliderMovementDataAsset m_movementDataAsset;

        private Vector3 m_linearVelocityWithoutGravity;

        public enum EState
        {
            Grounded = 0,
            Flight = 1
        }
        
        private readonly NetworkVariable<EState> m_state = new();

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
            Vector3 hangGliderDirection = m_rigidbody.transform.forward;
            /*Plane planarPlane = new Plane(Vector3.up, m_rigidbody.transform.position);
            Plane perpendicularHangGliderDirectionPlane = new Plane(m_rigidbody.transform.right, m_rigidbody.transform.position);
                
            if (PlanePlaneIntersection(out Vector3 somePoint, out Vector3 planarDirection, planarPlane, perpendicularHangGliderDirectionPlane))
            {
                planarDirection = -planarDirection;
            }
            else
            {
                planarDirection = m_rigidbody.transform.forward.Flatten();
            }*/
            var planarDirection = m_rigidbody.transform.forward.Flatten().normalized;
            float pitchingAngle = Vector3.Angle(hangGliderDirection, planarDirection);
            
            Vector3 hangGliderUp = m_rigidbody.transform.up;

            // Don't mind if you can't comprehend, it surely works (I hope).
            float rollingAngle = Vector3.Angle(hangGliderUp, Vector3.up) - pitchingAngle;
            
            var planarRight = Vector3.Cross(Vector3.up, planarDirection).normalized;

            
            // When the hang glider is glitching headdown, we want to reverse the right to match what it would be the head up.
   

            Quaternion rotation = m_rigidbody.transform.rotation;

            
            for (int i = 0; i < m_seatsController.MaximumSeats; ++i)
            {
                var seat = m_seatsController.SeatInfos_ServerOnly[i];

                if (!seat.Passenger)
                    continue;

                var directionInput = seat.Passenger.VehiclePassengerController.DirectionInput;

                // ROLL
                if (directionInput.x > 0)
                {
                    
                    var newRotation = Quaternion.RotateTowards(rotation,
                        Quaternion.LookRotation(hangGliderDirection, m_rigidbody.transform.right),
                        m_sensivities.x * directionInput.x
                        //* m_movementDataAsset.RollingCapacityWhenPitching.Evaluate(Mathf.Clamp01(Mathf.Abs(pitchingAngle / 90f))) 
                        * Time.deltaTime);
                    var newUp = newRotation * Vector3.up;
                    
                    var newSignedAngleToLimit = SignedAngleOnPlane(newUp, planarRight, hangGliderDirection);
                    var signedAngleToLimit = SignedAngleOnPlane(hangGliderUp, planarRight, hangGliderDirection);
                    
                    if (newSignedAngleToLimit < 0 || signedAngleToLimit >= newSignedAngleToLimit)
                        rotation = newRotation;
                }
                else
                {
                    var newRotation = Quaternion.RotateTowards(rotation, 
                        Quaternion.LookRotation(hangGliderDirection, -m_rigidbody.transform.right), 
                        m_sensivities.x * -directionInput.x
                        //* m_movementDataAsset.RollingCapacityWhenPitching.Evaluate(Mathf.Clamp01(Mathf.Abs(pitchingAngle / 90f))) 
                        * Time.deltaTime);
                    var newUp = newRotation * Vector3.up;
                    
                    var newSignedAngleToLimit = SignedAngleOnPlane(newUp, -planarRight, hangGliderDirection);
                    var signedAngleToLimit = SignedAngleOnPlane(hangGliderUp, -planarRight, hangGliderDirection);
                    
                    if (newSignedAngleToLimit > 0 || signedAngleToLimit <= newSignedAngleToLimit)
                        rotation = newRotation;
                }
                
                
                // PITCH
                if (directionInput.y > 0)
                {
                    var newRotation = Quaternion.RotateTowards(rotation, 
                        Quaternion.LookRotation(-Vector3.up, planarDirection),
                        m_sensivities.y * directionInput.y
                        * Time.deltaTime);
                    
                    var newForward = newRotation * Vector3.forward;
                    /*
                    var newSignedAngleToLimit = SignedAngleOnPlane(newForward, Vector3.down, planarRight);
                    var signedAngleToLimit = SignedAngleOnPlane(hangGliderDirection, Vector3.down, planarRight);
                    
                    if (newSignedAngleToLimit > 0 || signedAngleToLimit <= newSignedAngleToLimit)*/
                        rotation = newRotation;
                }
                else
                {
                    var newRotation = Quaternion.RotateTowards(rotation, 
                        Quaternion.LookRotation(Vector3.up, -planarDirection),
                        m_sensivities.y * -directionInput.y
                            * Time.deltaTime);
                    /*
                    var newForward = newRotation * Vector3.forward;
                    var newSignedAngleToLimit = SignedAngleOnPlane(newForward, Vector3.up, planarRight);
                    var signedAngleToLimit = SignedAngleOnPlane(hangGliderDirection, Vector3.up, planarRight);
                    
                    if (newSignedAngleToLimit < 0 || signedAngleToLimit >= newSignedAngleToLimit)*/
                        rotation = newRotation;
                }
                
            }
            
            m_rigidbody.transform.rotation = rotation;
            
            // Updating some values
            hangGliderDirection = m_rigidbody.transform.forward;
            planarDirection = m_rigidbody.transform.forward.Flatten().normalized;
            
            pitchingAngle = Vector3.Angle(hangGliderDirection, planarDirection);
            
            hangGliderUp = m_rigidbody.transform.up;
            
            planarRight = Vector3.Angle(Vector3.up, hangGliderUp) < 90f ? 
                Vector3.Cross(Vector3.up, planarDirection) 
                : Vector3.Cross(Vector3.down, planarDirection);

            // Don't mind if you can't comprehend it, it surely works (I hope).
            rollingAngle = Vector3.SignedAngle(hangGliderUp, Vector3.up, hangGliderDirection);
            
            
            // Updating yaw
            rotation = m_rigidbody.transform.rotation;

            /*if (rollingAngle > 0)
            {
                rotation = Quaternion.RotateTowards(Quaternion.LookRotation(hangGliderDirection, hangGliderUp),
                    quaternion.LookRotation(planarRight, hangGliderUp), 
                    m_movementDataAsset.RotationSpeedEvolutionBasedOnAngleFromStraightToSide.Evaluate(rollingAngle / 90f)
                    *  m_movementDataAsset.MaxRotationSpeedWhenFullyOnSide 
                    * Time.deltaTime);
            }
            else
            {
                rotation = Quaternion.RotateTowards(Quaternion.LookRotation(hangGliderDirection, hangGliderUp),
                    quaternion.LookRotation(-planarRight, hangGliderUp), 
                    m_movementDataAsset.RotationSpeedEvolutionBasedOnAngleFromStraightToSide.Evaluate(rollingAngle / 90f)
                    *  m_movementDataAsset.MaxRotationSpeedWhenFullyOnSide 
                    * Time.deltaTime);
            }*/
            
            m_rigidbody.transform.rotation = rotation;
            
            
            var linearVelocity = m_linearVelocityWithoutGravity.sqrMagnitude > m_rigidbody.linearVelocity.sqrMagnitude 
                ? m_rigidbody.linearVelocity : m_linearVelocityWithoutGravity;
            linearVelocity = ApplyForwardVelocity(linearVelocity, pitchingAngle);
            linearVelocity = ApplyAirResistance(linearVelocity);
            
            
            // We apply the down speed here so it is not keep between frames.
            //m_rigidbody.linearVelocity = linearVelocity - m_movementDataAsset.DownSpeedWhenStabilized * Vector3.up;
        }


        private Vector3 ApplyForwardVelocity(Vector3 a_linearVelocity, float a_pitchingAngle)
        {
            float acceleration = 0f;
            if (m_rigidbody.transform.forward.y > 0)
            {
                // Going Up
                
                acceleration = -m_movementDataAsset.DecelerationEvolutionBasedOnAngleFromStraightToUp.Evaluate(a_pitchingAngle / 90f) 
                                * m_movementDataAsset.ForwardDecelerationWhenGoingUp 
                                * Time.deltaTime;
            }
            else
            {
                // Going Down
                acceleration = m_movementDataAsset.AccelerationEvolutionBasedOnAngleFromStraightToDown.Evaluate(a_pitchingAngle / 90f) 
                               * m_movementDataAsset.ForwardAccelerationWhenGoingFullDown 
                               * Time.deltaTime;
            }
            a_linearVelocity += a_linearVelocity.normalized * (acceleration * Time.deltaTime);
            return a_linearVelocity;
        }

        private Vector3 ApplyAirResistance(Vector3 a_linearVelocity)
        {
            a_linearVelocity -= a_linearVelocity.normalized * (m_movementDataAsset.AirResistance * Time.deltaTime);
            return a_linearVelocity;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var transformToUse = m_rigidbody ? m_rigidbody.transform : transform;
            
            if (!Application.isPlaying || m_state.Value == EState.Flight)
            {
                Vector3 hangGliderDirection = transformToUse.forward;
                
                Gizmos.color = Color.greenYellow;
                Gizmos.DrawLine(transformToUse.position, transformToUse.position + hangGliderDirection * 10f);
                
            
                /*Plane planarPlane = new Plane(Vector3.up, transformToUse.position);
                Plane perpendicularHangGliderDirectionPlane = new Plane(transformToUse.right, transformToUse.position);
                
                if (!PlanePlaneIntersection(out Vector3 somePoint, out Vector3 planarDirection, planarPlane, perpendicularHangGliderDirectionPlane))
                {
                    planarDirection = transformToUse.forward.Flatten();
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(transformToUse.position, transformToUse.position + planarDirection * 5f);
                }
                else
                {
                    Gizmos.color = Color.darkGreen;
                    Gizmos.DrawLine(transformToUse.position, transformToUse.position - planarDirection * 5f);
                }*/
                var planarDirection = transformToUse.forward.Flatten().normalized;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transformToUse.position, transformToUse.position + planarDirection * 5f);
                
                
                float pitchingAngle = Vector3.Angle(hangGliderDirection, planarDirection);
            
                Vector3 hangGliderUp = transformToUse.up;
                Gizmos.color = Color.black;
                Gizmos.DrawLine(transformToUse.position, transformToUse.position + hangGliderUp * 10f);

                // Don't mind if you can't comprehend, it surely works (I hope).
                float rollingAngle = Vector3.Angle(hangGliderUp, Vector3.up) - pitchingAngle;
            
                var planarRight = Vector3.Cross(Vector3.up, planarDirection).normalized;
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transformToUse.position, transformToUse.position + planarRight * 13f);
                
                var otherRight = Vector3.ProjectOnPlane(transformToUse.right, Vector3.up).normalized;
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transformToUse.position, transformToUse.position + otherRight * 10f);
                
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transformToUse.position, transformToUse.position + transformToUse.right * 7f);
                
                // When the hang glider is glitching headdown, we want to reverse the right to match what it would be the head up.
            }
        }
#endif
        public static bool PlanePlaneIntersection(out Vector3 a_point, out Vector3 a_direction, Plane a_p1, Plane a_p2)
        {
            // Directions
            a_direction = Vector3.Cross(a_p1.normal, a_p2.normal);

            // Si les normales sont parallèles → pas d'intersection (plans parallèles)
            if (a_direction.sqrMagnitude < 1e-6)
            {
                a_point = Vector3.zero;
                return false;
            }

            // Résolution système d'équations :
            // p1.normal · X = -p1.distance
            // p2.normal · X = -p2.distance

            Vector3 n1 = a_p1.normal;
            Vector3 n2 = a_p2.normal;

            float d1 = -a_p1.distance;
            float d2 = -a_p2.distance;

            // Calcul d'un point sur la ligne
            Vector3 n1xn2 = Vector3.Cross(n1, n2);
            Vector3 temp = (d1 * n2 - d2 * n1);

            a_point = Vector3.Cross(temp, n1xn2) / n1xn2.sqrMagnitude;

            return true;
        }

        public static float SignedAngleOnPlane(Vector3 a_firstVector, Vector3 a_secondVector, Vector3 a_planeNormal)
        {
            var projectedFirstVector = Vector3.ProjectOnPlane(a_firstVector, a_planeNormal);
            var projectedSecondVector = Vector3.ProjectOnPlane(a_secondVector, a_planeNormal);

            return Vector3.SignedAngle(projectedFirstVector, projectedSecondVector, a_planeNormal);
        }

    }
}
