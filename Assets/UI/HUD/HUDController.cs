using TMPro;
using UnityEngine;

namespace SpikeScape.UI.HUD
{
    /// <summary>
    /// Manages the Heads-Up Display (HUD) elements in the game.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        private void Start()
        {
            UpdateScore(0); // Initialize score display
        }

        public void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
            }
        }
    }
}
