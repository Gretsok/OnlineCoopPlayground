using UnityEngine;

namespace Game.Playground.PlayerCharacter.Animation
{
    public class PlayerCharacterAnimationController : MonoBehaviour
    {
        private readonly int MOVING_ON_Z = Animator.StringToHash("MovingOnZ");
        private readonly int MOVING_ON_X = Animator.StringToHash("MovingOnX");
        
        [SerializeField]
        private Animator m_animator;

        [SerializeField]
        private float m_speedRoughness = 15f;
        [SerializeField]
        private float m_heightAdaptationRoughness = 15f;

        public void SetForwardSpeed(float a_speed)
        {
            m_animator.SetFloat(MOVING_ON_Z, Mathf.Lerp(m_animator.GetFloat(MOVING_ON_Z), a_speed, m_speedRoughness * Time.deltaTime));
        }

        public void SetHeightAdaptation(float a_heightAdaptation)
        {
            m_animator.SetFloat(MOVING_ON_X, Mathf.Lerp(m_animator.GetFloat(MOVING_ON_X), a_heightAdaptation, m_heightAdaptationRoughness * Time.deltaTime));
        }
    }
}
