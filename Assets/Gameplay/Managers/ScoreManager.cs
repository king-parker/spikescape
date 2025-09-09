using SpikeScape.Gameplay.Objective;
using SpikeScape.UI.HUD;
using UnityEngine;

namespace SpikeScape.Gameplay.Managers
{
    /// <summary>
    /// Manages the player's score and related functionalities.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private HUDController hudController;
        [SerializeField] private int pointsPerObjective = 1;

        private int _score;

        private void OnEnable()
        {
            ObjectiveController.OnObjectiveCollected += HandleObjectiveCollected;
        }

        private void OnDisable()
        {
            ObjectiveController.OnObjectiveCollected -= HandleObjectiveCollected;
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

        private void IncreaseScore(int amount)
        {
            _score += amount;
            hudController.UpdateScore(_score);
        }
    }
}
