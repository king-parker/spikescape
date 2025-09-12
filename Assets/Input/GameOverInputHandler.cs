using SpikeScape.Gameplay.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpikeScape.Input
{
    /// <summary>
    /// Handles player input during the Game Over state.
    /// </summary>
    public class GameOverInputHandler : MonoBehaviour
    {
        private InputAction _restartAction;

        private void OnEnable()
        {
            _restartAction = InputSystem.actions.FindAction(InputActionNames.GameOver.Restart);

            _restartAction.performed += OnRestart;
        }

        private void OnDisable()
        {
            _restartAction.performed -= OnRestart;
        }

        private void OnRestart(InputAction.CallbackContext context)
        {
            if (GameManager.Instance.CurrentState != GameManager.GameState.GameOver) return;

            GameManager.Instance.RestartGame();
        }
    }
}