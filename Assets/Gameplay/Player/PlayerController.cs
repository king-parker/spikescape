using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpikeScape.Gameplay.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float forwardImpulse = 10f;
        [SerializeField] private float maxHorizontalSpeed = 5f;
        [SerializeField] private float turnSpeed = 90f;
        [SerializeField] private float slideForce = 10f;

        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float groundCheckDistance = 0.5f;

        private Rigidbody _rb;
        private InputAction _turnAction;
        private InputAction _jumpAction;
        private InputAction _lookAction;
        private float _turnInput;
        private Vector3 _nonFloorCollision;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            _turnAction = InputSystem.actions.FindAction(InputActionNames.Player.Turn);
            _jumpAction = InputSystem.actions.FindAction(InputActionNames.Player.Jump);
            _lookAction = InputSystem.actions.FindAction(InputActionNames.Player.Look);

            _jumpAction.performed += OnJump;
        }

        private void OnDisable()
        {
            _jumpAction.performed -= OnJump;
        }

        private void Update()
        {
            _turnInput = _turnAction.ReadValue<float>();
        }

        private void FixedUpdate()
        {
            // Turning
            if (Math.Abs(_turnInput) > 0.01f)
            {
                transform.Rotate(_turnInput * turnSpeed * Time.fixedDeltaTime * Vector3.up);
            }

            // Constant forward impulse
            _rb.AddForce(transform.forward * forwardImpulse, ForceMode.Acceleration);


            Vector3 horizontalVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

            if (_nonFloorCollision != Vector3.zero)
            {
                // Apply sliding force along the collision surface
                Vector3 collisionTangent = Vector3.ProjectOnPlane(horizontalVelocity, _nonFloorCollision);
                _rb.AddForce(collisionTangent.normalized * slideForce, ForceMode.Acceleration);

                _nonFloorCollision = Vector3.zero; // Reset after applying force
            }

            // Limit horizontal speed
            if (horizontalVelocity.magnitude > maxHorizontalSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxHorizontalSpeed;
                _rb.linearVelocity = new Vector3(horizontalVelocity.x, _rb.linearVelocity.y, horizontalVelocity.z);
            }
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (IsGrounded())
            {
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
        }

        private void OnCollisionStay(Collision collision)
        {
            var numCollisions = 0;
            _nonFloorCollision = Vector3.zero;

            foreach (ContactPoint contact in collision.contacts)
            {
                // Ignore ground collisions
                if (Vector3.Dot(contact.normal, Vector3.up) < 0.7f) // Approximately 45 degrees
                {
                    numCollisions++;
                    _nonFloorCollision += contact.normal;
                }
            }

            if (numCollisions > 0)
            {
                _nonFloorCollision /= numCollisions;
                _nonFloorCollision.Normalize();
            }
        }
    }
}