using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MG
{
    [RequireComponent(typeof(DroneInputs))]
    public class DroneController : DroneRigidBody
    {
        [SerializeField] Transform cameraObject;
        Vector3 moveDirection;


        [SerializeField] Transform refValue;

        [Header("Control Properties")]
        [SerializeField] private float movementSpeed;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float minMaxPitch = 30f;
        [SerializeField] private float minMaxRoll = 30f;
        [SerializeField] private float yawPower = 4f;
        [SerializeField] private float maxYawPower = 4f;
        [SerializeField] private float minYawPower = 4f;
        [SerializeField] private float lerpSpeed = 2f;
        private DroneInputs input;
        private List<IEngine> engines = new List<IEngine>();

        private float finalPitch;
        private float finalRoll;
        public float yaw;
        private float finalYaw;


        [Header("GroundCheck")]
        [SerializeField] Transform groundCheck;
        [SerializeField] float groundCheckRadius;
        [SerializeField] LayerMask WhatIsGround;
        bool isGrounded;
        private void Awake()//Here
        {
            input = GetComponent<DroneInputs>();
            engines = GetComponentsInChildren<IEngine>().ToList<IEngine>();
        }
        protected override void HandlePhysics()
        {
            //HandleRotation();
            HandleEngines();
            HandleControls();
            CheckGround();

            
           
        }

        private void LateUpdate()
        {
            CameraControllerNonCinemachine.instance.FollowTarget(transform);
            CameraControllerNonCinemachine.instance.RotateCamera(transform);
        }
        void CheckGround()
        {
            isGrounded= Physics.CheckSphere(groundCheck.position, groundCheckRadius, WhatIsGround);
        }

        protected virtual void HandleEngines() 
        {
            //rb.AddForce(Vector3.up * rb.mass * Physics.gravity.magnitude);
            foreach(IEngine engine in engines)
            {
                engine.UpdateEngine(rb,input,isGrounded);
            }
        }
        protected virtual void HandleControls() 
        {
            HandleMovement();
            float pitch = input.Cyclic.y * minMaxPitch;
            float roll = -input.Cyclic.x * minMaxRoll;
            yaw += input.Pedals * yawPower;


            finalPitch = Mathf.Lerp(finalPitch, pitch, Time.deltaTime * lerpSpeed);
            finalRoll = Mathf.Lerp(finalRoll, roll, Time.deltaTime * lerpSpeed);
            finalYaw = Mathf.Lerp(finalYaw, yaw, Time.deltaTime * lerpSpeed);

            /*if (yaw > maxYawPower)
            {
                finalYaw = maxYawPower;
            }
            else if (yaw < minYawPower)
            {
                finalYaw = minYawPower;
            }
            *//*if (yaw > 180 || yaw < -180)
                yaw = 0;*/

            Quaternion rot = Quaternion.Euler(finalPitch,refValue.eulerAngles.y, finalRoll);
            rb.MoveRotation(rot);
        }

        void HandleMovement()
        {
            moveDirection = cameraObject.forward * input.Cyclic.y;
            moveDirection += cameraObject.right * input.Cyclic.x;
            moveDirection.Normalize();
            moveDirection.y = 0;
            moveDirection *= movementSpeed;
            Vector3 movementVelocity = moveDirection;


            rb.velocity = movementVelocity;
        }
        void HandleRotation()
        {
            Vector3 targetDirection = Vector3.zero;
            targetDirection = cameraObject.forward * input.Cyclic.y;
            targetDirection += cameraObject.right * input.Cyclic.x;
            targetDirection.Normalize();
            targetDirection.y = 0;

            /*if (targetDirection == Vector3.zero)
                targetDirection = transform.forward; */

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion droneRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed*Time.deltaTime);
            transform.rotation = droneRotation;
        }
    }
}
