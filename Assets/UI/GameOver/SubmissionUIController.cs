using Spikescape.Gameplay.Managers;
using Spikescape.Leaderboard;
using Spikescape.UI.Common;
using Spikescape.Utility.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spikescape.UI.GameOver
{
    /// <summary>
    /// Represents the user interface for submitting scores at the end of the game, as well as restarting the game.
    /// </summary>
    public class SubmissionUIController : MonoBehaviour
    {
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
        private bool _hasLowestHighScore = false;
        private int _lowestHighScore;
        private bool _scoreSent = false;

        private void Awake()
        {
            nameInputField.onValidateInput += OnValidateInputField;
        }

        private void Start()
        {
            restartButton.onClick.AddListener(() =>
            {
                GameManager.Instance.RestartGame();
            });

            submitScoreButton.onClick.AddListener(OnSubmitScoreButtonClicked);
        }

        private void OnEnable()
        {
            ScoreManager.SendFinalScore += UpdateScore;
            LootLockerManager.OnScoreSubmitted += OnScoreSubmitted;
            LootLockerManager.OnLowestHighScoreFound += RecordLowestHighScore;
        }

        private void OnDisable()
        {
            ScoreManager.SendFinalScore -= UpdateScore;
            LootLockerManager.OnScoreSubmitted -= OnScoreSubmitted;
            LootLockerManager.OnLowestHighScoreFound -= RecordLowestHighScore;
        }

        private char OnValidateInputField(string input, int charIndex, char addedChar)
        {
            // Allow letters, numbers, underscores, and hyphens
            if (char.IsLetterOrDigit(addedChar) || addedChar == '_' || addedChar == '-')
            {
                return addedChar; // Valid character
            }

            // Allow a single space, but not at the start or end, and not consecutive
            if (addedChar == ' ')
            {
                if (charIndex == 0)
                {
                    return '\0'; // Reject space at start
                }
                if (charIndex > 0 && input[charIndex - 1] == ' ')
                {
                    return '\0'; // Reject consecutive spaces
                }
                return addedChar; // Valid space
            }

            // Otherwise, reject the character
            return '\0'; // Null character
        }

        private void OnSubmitScoreButtonClicked()
        {
            if (!_scoreReceived) return;

            string playerName = nameInputField.text.Trim();
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
            _scoreSent = true;
        }

        private void OnScoreSubmitted(bool success)
        {
            MainThreadDispatcher.Run(() =>
            {
                if (!success)
                {
                    toast.Show("Failed to submit score. Please try again.", toastDuration);
                    submitScoreButton.interactable = true; // Re-enable button on failure
                    _scoreSent = false;
                }
            });
        }

        private void UpdateScore(int score, int highScore)
        {
            _finalScore = score;
            _scoreReceived = true;
            TryAllowScoreSubmission();
        }

        private void RecordLowestHighScore(int lowestHighScore)
        {
            _lowestHighScore = lowestHighScore;
            _hasLowestHighScore = true;
            TryAllowScoreSubmission();
        }

        private void TryAllowScoreSubmission()
        {
            if (_scoreSent) return;
            if (!_scoreReceived || !_hasLowestHighScore) return;

            if (_finalScore < _lowestHighScore) { HideSubmissionUI(); }
            else { ShowAndEnableSubmissionUI(); }
        }

        public void HideSubmissionUI()
        {
            nameInputField.gameObject.SetActive(false);
            submitScoreButton.gameObject.SetActive(false);
        }

        public void ShowAndEnableSubmissionUI()
        {
            nameInputField.gameObject.SetActive(true);
            nameInputField.text = string.Empty;
            submitScoreButton.gameObject.SetActive(true);
            submitScoreButton.interactable = true;
        }
    }
}