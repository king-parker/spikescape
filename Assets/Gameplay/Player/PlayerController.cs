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
        private Vector3 _ceilingCollisionNormal;

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
            if (_rb.linearVelocity.y > 0.001f)
            {
                Debug.Log($"[{Time.fixedTime:F4}s] Vertical Velocity: {_rb.linearVelocity.y}");
            }

            // Turn
            if (Math.Abs(_turnInput) > 0.01f)
            {
                transform.Rotate(_turnInput * turnSpeed * Time.fixedDeltaTime * Vector3.up);
            }

            // Calculate desired velocity
            Vector3 desiredVelocity = transform.forward * forwardSpeed;
            desiredVelocity.y = _rb.linearVelocity.y; // Preserve vertical velocity (gravity, jumping)

            // Adjust for collisions
            var collisionNormal = _rb.linearVelocity.y > 0 ? _ceilingCollisionNormal : _sideCollisionNormal;
            if (collisionNormal != Vector3.zero)
            {
                Debug.Log($"[{Time.fixedTime:F4}s] 1. Is jumping: {_rb.linearVelocity.y > 0}");
                Debug.Log($"[{Time.fixedTime:F4}s] 2. Current desired velocity: {desiredVelocity}");
                Debug.Log($"[{Time.fixedTime:F4}s] 3. Adjusting velocity for collision normal: {collisionNormal}");
                desiredVelocity = Vector3.ProjectOnPlane(desiredVelocity, collisionNormal);
                Debug.Log($"[{Time.fixedTime:F4}s] 4. New desired velocity: {desiredVelocity}");
            }

            _rb.linearVelocity = desiredVelocity;
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (IsGrounded())
            {
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                //Debug.Log($"Jumped! Velocity.y={_rb.linearVelocity.y}");
                Debug.Log("[{Time.fixedTime:F4}s] Jumped");
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
            var numCeilingCollisions = 0;
            _ceilingCollisionNormal = Vector3.zero;

            foreach (ContactPoint contact in collision.contacts)
            {
                //Ignore ground collisions
                if (Vector3.Dot(contact.normal, Vector3.up) < 0.7f) // ~45 degrees
                {
                    numSideCollisions++;
                    _sideCollisionNormal += contact.normal;
                }

                if (Vector3.Dot(contact.normal, Vector3.down) > 0.1f)
                {
                    numCeilingCollisions++;
                    _ceilingCollisionNormal += contact.normal;
                }
            }

            if (numSideCollisions > 0)
            {
                //Debug.Log($"Summed side collision normal: {_sideCollisionNormal}, number of collisions: {numSideCollisions}");
                _sideCollisionNormal /= numSideCollisions;
                _sideCollisionNormal.y = 0; // Ignore vertical component
                _sideCollisionNormal.Normalize();
                //Debug.Log($"Side collision normal: {_sideCollisionNormal}");
            }
            else
            {
                _sideCollisionNormal = Vector3.zero;
            }

            if (numCeilingCollisions > 0)
            {
                _ceilingCollisionNormal /= numCeilingCollisions;
                _ceilingCollisionNormal.Normalize();
                //Debug.Log($"Ceiling collision normal: {_ceilingCollisionNormal}");
            }
            else
            {
                _ceilingCollisionNormal = Vector3.zero;
            }
        }
    }
}