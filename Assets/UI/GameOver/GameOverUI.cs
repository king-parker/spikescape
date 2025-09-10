using SpikeScape.Gameplay.Managers;
using TMPro;
using UnityEngine;

namespace SpikeScape.UI.GameOver
{
    /// <summary>
    /// Represents the user interface displayed when the game is over.
    /// </summary>
    /// <remarks>This class is responsible for managing the visual and interactive elements  shown to the
    /// player at the end of the game. It is typically used to display  game results, such as scores, and provide
    /// options to restart or exit the game.</remarks>
    public class GameOverUI : MonoBehaviour
    {
        [Header("Fade Animation Settings")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 1.0f;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI scoreText;

        private void Awake()
        {
            canvasGroup.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            GameManager.OnGameOver += ShowUI;
            ScoreManager.SendFinalScore += UpdateScore;
        }

        private void OnDisable()
        {
            GameManager.OnGameOver -= ShowUI;
            ScoreManager.SendFinalScore -= UpdateScore;
        }

        public void ShowUI()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.gameObject.SetActive(true);
            StartCoroutine(FadeInRoutine());
        }

        public void UpdateScore(int score)
        {
            scoreText.text = $"Score: {score}";
        }

        private System.Collections.IEnumerator FadeInRoutine()
        {
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }
    }
}
