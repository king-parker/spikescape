using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpikeScape.Gameplay.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float forwardSpeed = 5f;
        [SerializeField] private float turnSpeed = 90f;
        [SerializeField] private float jumpForce = 5f;

        private Rigidbody _rb;
        private InputAction _turnAction;
        private InputAction _jumpAction;
        private InputAction _lookAction;
        private float _turnInput;
        private Vector3 _sideCollisionNormal;

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
            // Turn
            if (Math.Abs(_turnInput) > 0.01f)
            {
                transform.Rotate(_turnInput * turnSpeed * Time.fixedDeltaTime * Vector3.up);
            }

            // Calculate desired velocity
            Vector3 desiredVelocity = transform.forward * forwardSpeed;

            // Adjust for side collisions
            if (_sideCollisionNormal != Vector3.zero)
            {
                desiredVelocity = Vector3.ProjectOnPlane(desiredVelocity, _sideCollisionNormal);
            }

            desiredVelocity.y = _rb.linearVelocity.y; // Preserve vertical velocity (gravity, jumping)

            _rb.linearVelocity = desiredVelocity;
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
            return Physics.Raycast(transform.position, Vector3.down, 0.5f);
        }

        private void OnCollisionStay(Collision collision)
        {
            var numSideCollisions = 0;
            _sideCollisionNormal = Vector3.zero;

            foreach (ContactPoint contact in collision.contacts)
            {
                //Ignore ground collisions
                if (Vector3.Dot(contact.normal, Vector3.up) < 0.7f) // ~45 degrees
                {
                    numSideCollisions++;
                    _sideCollisionNormal += contact.normal;
                }
            }

            if (numSideCollisions > 0)
            {
                _sideCollisionNormal /= numSideCollisions;
                _sideCollisionNormal.y = 0; // Ignore vertical component
                _sideCollisionNormal.Normalize();
            }
            else
            {
                _sideCollisionNormal = Vector3.zero;
            }
        }
    }
}