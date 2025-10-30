using Spikescape.Leaderboard;
using Spikescape.UI.Common;
using Spikescape.Gameplay.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spikescape.UI.GameOver
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

        [Header("Buttons")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button submitScoreButton;

        [Header("Input Fields")]
        [SerializeField] private TMP_InputField nameInputField;

        [Header("Toast Notification")]
        [SerializeField] private ToastController toast;
        [SerializeField] private float toastDuration = 2f;

        private int _finalScore;
        private bool _scoreReceived = false;

        private void Awake()
        {
            canvasGroup.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            GameManager.OnGameOver += ShowUI;
            ScoreManager.SendFinalScore += UpdateScore;
            // TODO: Subscribe to LootLockerManager.OnScoreSubmitted to handle submission result
        }

        private void OnDisable()
        {
            GameManager.OnGameOver -= ShowUI;
            ScoreManager.SendFinalScore -= UpdateScore;
        }

        private void Start()
        {
            restartButton.onClick.AddListener(() =>
            {
                GameManager.Instance.RestartGame();
            });

            submitScoreButton.onClick.AddListener(OnSubmitScoreButtonClicked);

            _finalScore = 0;
            _scoreReceived = false;
        }

        public void ShowUI()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.gameObject.SetActive(true);
            StartCoroutine(FadeInRoutine());
        }

        public void UpdateScore(int score, int highScore)
        {
            scoreText.text = $"Score: {score}";
            _finalScore = score;
            _scoreReceived = true;
            // TODO: Remove high from method parameters
            // highScoreText.text = $"High Score: {highScore}";
        }

        private void OnSubmitScoreButtonClicked()
        {
            if (!_scoreReceived) return;

            string playerName = nameInputField.text;
            string validationError = NameValidator.ValidateName(playerName);

            if (validationError != null)
            {
                toast.Show(validationError, toastDuration);
                nameInputField.text = string.Empty; // Clear invalid input
                submitScoreButton.interactable = true; // Make sure button is interactable
                return;
            }

            LootLockerManager.Instance.SubmitScore(playerName, _finalScore);
            submitScoreButton.interactable = false;
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
