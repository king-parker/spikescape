using SpikeScape.Audio.Managers;
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
        [SerializeField] private float coyoteTime = 0.2f;
        [SerializeField] private float jumpBufferTime = 0.2f;
        [SerializeField] private float variableJumpHeightMultiplier = 0.5f;

        [Header("Ground Detection")]
        [SerializeField] private float shpereCastRadius = 0.4f;
        [SerializeField] private float sphereCastOffset = 0.1f;

        private Rigidbody _rb;
        private InputAction _turnAction;
        private InputAction _jumpAction;
        private InputAction _lookAction;
        private float _turnInput;
        private Vector3 _nonFloorCollision;
        private bool _isJumping = false;
        private float _coyoteTimerCounter = 0f;
        private float _jumpBufferCounter = 0f;

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
            _jumpAction.canceled += OnJumpCancel;
        }

        private void OnDisable()
        {
            _jumpAction.performed -= OnJump;
            _jumpAction.canceled -= OnJumpCancel;
        }

        private void Update()
        {
            // Read player inputs
            _turnInput = _turnAction.ReadValue<float>();

            // Update jumping state
            if (_isJumping && _rb.linearVelocity.y <= 0f)
            {
                _isJumping = false;
            }

            // Coyote time handling
            if (IsGrounded() && !_isJumping)
            {
                _coyoteTimerCounter = coyoteTime;
            }
            else
            {
                _coyoteTimerCounter -= Time.deltaTime;
            }

            // Jump buffer handling
            if (_jumpBufferCounter > 0f)
            {
                _jumpBufferCounter -= Time.deltaTime;
            }
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

            // Handle jump if buffered and within coyote time
            if (!_isJumping && _jumpBufferCounter > 0f && _coyoteTimerCounter > 0f)
            {
                Jump();
            }
        }

        private void Jump()
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z); // Reset vertical velocity

            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _isJumping = true;

            _jumpBufferCounter = 0f; // Reset jump buffer after jumping
            _coyoteTimerCounter = 0f; // Reset coyote timer after jumping

            SoundManager.Instance.PlayJump();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _jumpBufferCounter = jumpBufferTime;
            }
        }

        private void OnJumpCancel(InputAction.CallbackContext context)
        {
            if (context.canceled && _isJumping && _rb.linearVelocity.y > 0f)
            {
                // Reduce upward velocity for variable jump height
                _rb.linearVelocity = new Vector3(
                    _rb.linearVelocity.x,
                    _rb.linearVelocity.y * variableJumpHeightMultiplier,
                    _rb.linearVelocity.z
                 );
            }
        }

        private bool IsGrounded()
        {
            Vector3 castPosition = transform.position + Vector3.down * sphereCastOffset;
            Collider[] hits = Physics.OverlapSphere(castPosition, shpereCastRadius);

            foreach (var hit in hits)
            {
                if (hit.transform.IsChildOf(transform) != gameObject && !hit.isTrigger)
                {
                    return true;
                }
            }

            return false;
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

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // Draw ground check sphere
            Gizmos.color = Color.green;
            Vector3 castPosition = transform.position + Vector3.down * sphereCastOffset;
            Gizmos.DrawWireSphere(castPosition, shpereCastRadius);
        }
#endif
    }
}