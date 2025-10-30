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

        private void OnSubmitScoreButtonClicked()
        {
            Debug.Log("Submit Score button clicked. Checking if score has been received");
            Debug.Log($"_scoreReceived: {_scoreReceived}");
            if (!_scoreReceived) return;

            Debug.Log("Processing score submission...");
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
            Debug.Log($"SubmissionUIController received final score: {score}");
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