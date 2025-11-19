using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Gameplay.VehiclesSystem
{
    public class VehicleLocalCameraController : MonoBehaviour
    {
        [field: SerializeField]
        public CinemachineCamera CameraAnchor { get; private set; }

        private void Awake()
        {
            DeactivateCamera();
        }

        public void ActivateCamera()
        {
            CameraAnchor.gameObject.SetActive(true);
        }

        public void DeactivateCamera()
        {
            CameraAnchor.gameObject.SetActive(false);
        }
    }
}
