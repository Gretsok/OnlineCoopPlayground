using UnityEngine;

namespace Game.Playground.PlayerCharacter.Animation.Editor
{
    public class ZooCharacterMover : MonoBehaviour
    {
        [SerializeField]
        private float m_movingSpeed = 5f;
        [SerializeField]
        private float m_durationToLoop = 3f;

        private Vector3 m_startingPosition;
        private float m_lastStartOfLoopTime;
    
        private void Start()
        {
            m_startingPosition = transform.position;
            m_lastStartOfLoopTime = Time.time;
        }

        private void Update()
        {
            if (Time.time - m_lastStartOfLoopTime > m_durationToLoop)
            {
                m_lastStartOfLoopTime = Time.time;
                transform.position = m_startingPosition;
            }
            transform.position += transform.forward * (m_movingSpeed * Time.deltaTime);
        }
    }
}
