using UnityEngine;

namespace Tools.Utils
{
    [RequireComponent(typeof(Animator))]
    public class SimpleAnimationPlayer : MonoBehaviour
    {
        [SerializeField]
        private string m_animationName;
        
        private void Start()
        {
            var animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError($"{name}: Animator component is missing.");
                return;
            }
            animator.Play(m_animationName);
        }
    }
}
