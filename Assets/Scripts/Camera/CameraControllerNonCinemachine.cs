using UnityEngine;

namespace MG
{
    public class CameraControllerNonCinemachine : MonoBehaviour
    {
        public static CameraControllerNonCinemachine instance = null;
        [SerializeField] private DroneInputs input;
        float lookAngle;
        [SerializeField] float lookSpeed;
        [SerializeField] float cameraFollowSpeed;
        Vector3 currentVelocity = Vector3.zero;

        //Transform mainCamera;

        public float y;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }

            //mainCamera = Camera.main.transform;
        }

        private void Start()
        {
        }

        private void Update()
        {
        }

        public void RotateCamera(Transform target)
        {
            lookAngle += input.Look * lookSpeed;
            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            transform.rotation = targetRotation;
        }

        public void FollowTarget(Transform targetPosition)
        {
            Vector3 target = Vector3.SmoothDamp(transform.position, targetPosition.position, ref currentVelocity, cameraFollowSpeed);
            transform.position = target;
            /*Vector3 desiredPosition = targetPosition + offset;
            Vector3 smoothedPosition = Vector3.Lerp(mainCamera.position, desiredPosition, cameraFollowSpeed * Time.deltaTime);
            mainCamera.position = smoothedPosition;

            mainCamera.LookAt(targetPosition);*/
        }
    }
}