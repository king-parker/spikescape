using SpikeScape.Gameplay.Objective;
using SpikeScape.UI.HUD;
using System;
using UnityEngine;

namespace SpikeScape.Gameplay.Managers
{
    /// <summary>
    /// Manages the player's score and related functionalities.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public static event Action<int> SendFinalScore;
        [SerializeField] private HUDController hudController;
        [SerializeField] private int pointsPerObjective = 1;

        private int _score;

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
            _score = 0;
            hudController.UpdateScore(_score);
        }

        private void HandleObjectiveCollected()
        {
            IncreaseScore(pointsPerObjective);
        }

        private void HandleGameOver()
        {
            SendFinalScore?.Invoke(_score);
        }

        private void IncreaseScore(int amount)
        {
            _score += amount;
            hudController.UpdateScore(_score);
        }
    }
}
