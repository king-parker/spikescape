using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpikeScape.Gameplay.Player {
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
            // Move forward
            Vector3 forwardMovement = transform.forward * forwardSpeed * Time.fixedDeltaTime;
            _rb.MovePosition(_rb.position + forwardMovement);

            // Turn
            if (Math.Abs(_turnInput) > 0.01f)
            {
                transform.Rotate(_turnInput * turnSpeed * Time.fixedDeltaTime * Vector3.up);
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
            return Physics.Raycast(transform.position, Vector3.down, 0.5f);
        }
    }
}