using Spikescape.Gameplay.Objective;
using Spikescape.UI.HUD;
using System;
using UnityEngine;

namespace Spikescape.Gameplay.Managers
{
    /// <summary>
    /// Manages the player's score and related functionalities.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public static event Action<int, int> SendFinalScore; // (final score, high score)
        [SerializeField] private HUDController hudController;
        [SerializeField] private int pointsPerObjective = 1;

        private int _score;
        private int _highScore;

        private const string HighScoreKey = "HighScore";

        private void OnEnable()
        {
            ObjectiveController.OnObjectiveCollected += HandleObjectiveCollected;
            GameManager.OnGameOver += HandleGameOver;
        }

        private void OnDisable()
        {
            ObjectiveController.OnObjectiveCollected -= HandleObjectiveCollected;
            GameManager.OnGameOver -= HandleGameOver;
        }

        private void Start()
        {
            // Initialize score and update HUD
            _score = 0;
            hudController.UpdateScore(_score);

            // Load high score from PlayerPrefs
            _highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        }

        private void HandleObjectiveCollected()
        {
            IncreaseScore(pointsPerObjective);
        }

        private void HandleGameOver()
        {
            // Check and update high score if necessary
            if (_score > _highScore)
            {
                _highScore = _score;
                PlayerPrefs.SetInt(HighScoreKey, _highScore);
                PlayerPrefs.Save();
            }

            SendFinalScore?.Invoke(_score, _highScore);
        }

        private void IncreaseScore(int amount)
        {
            _score += amount;
            hudController.UpdateScore(_score);
        }
    }
}
