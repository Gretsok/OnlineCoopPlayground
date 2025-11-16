using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Movement.Debug
{
    public class RealVelocityCalculator : MonoBehaviour
    {
        private Vector3[] m_positions = new Vector3[10];
        private int m_index = 0;

        public float SpeedOutput = 0f;
        
        private void FixedUpdate()
        {
            m_positions[m_index] = transform.position;
            m_index = (m_index + 1) % m_positions.Length;
            
            
            SpeedOutput = 0f;

            for (int i = 0; i < m_positions.Length - 1; i++)
            {
                var position = m_positions[i];
                var nextPosition = m_positions[i + 1];
                
                SpeedOutput += Vector3.Distance(position, nextPosition);
            }

            SpeedOutput = SpeedOutput / 10 / Time.fixedDeltaTime;
        }
    }
}
