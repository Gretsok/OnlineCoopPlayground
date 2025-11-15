using UnityEngine;

namespace Game.Gameplay.PlayerCharacter.Animation
{
    public class PlayerCharacterAnimationController : MonoBehaviour
    {
        private readonly int MOVING_ON_Z = Animator.StringToHash("MovingOnZ");
        private readonly int MOVING_ON_X = Animator.StringToHash("MovingOnX");
        private readonly int IS_GROUNDED = Animator.StringToHash("IsGrounded");
        private readonly int IS_DANCING = Animator.StringToHash("IsDancing");
        
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

        public void SetIsGrounded(bool a_isGrounded)
        {
            m_animator.SetBool(IS_GROUNDED, a_isGrounded);
        }
        
        public void StartPlayingDance()
        {
            m_animator.applyRootMotion = true;
            m_animator.SetBool(IS_DANCING, true);
        }

        public void StopPlayingDance()
        {
            m_animator.SetBool(IS_DANCING, false);
            m_animator.applyRootMotion = false;
            m_animator.transform.SetLocalPositionAndRotation(default, default);
        }
    }
}
