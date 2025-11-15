using Unity.Cinemachine;
using UnityEngine;

namespace Game.Gameplay.CameraSystem
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { get; private set; }
        
        [field: SerializeField]
        public CinemachineCamera CameraAnchor { get; private set; }

        private CinemachineOrbitalFollow m_orbitalFollow;
        [SerializeField]
        private Vector2 m_sensivities = new Vector2(0.1f, 0.2f);

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            m_orbitalFollow = CameraAnchor.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineOrbitalFollow;
        }
        

        public void AssignCameraTarget(Transform a_cameraTarget)
        {
            CameraAnchor.Target.TrackingTarget = a_cameraTarget;
        }

        private Vector2 m_lookAroundInput;
        public void SetLookAroundInput(Vector2 a_lookAroundInput)
        {
            m_lookAroundInput = a_lookAroundInput;
        }

        private void LateUpdate()
        {
            if (!m_orbitalFollow)
                return;
            m_orbitalFollow.HorizontalAxis.Value += m_sensivities.x * m_lookAroundInput.x * Time.deltaTime;
            m_orbitalFollow.HorizontalAxis.Validate();
            m_orbitalFollow.VerticalAxis.Value -= m_sensivities.y * m_lookAroundInput.y * Time.deltaTime;
            m_orbitalFollow.VerticalAxis.Validate();
        }
    }
}
