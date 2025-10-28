using LootLocker.Requests;
using System;
using UnityEngine;

namespace Spikescape.Leaderboard
{
    public class LootLockerManager : MonoBehaviour
    {
        public static LootLockerManager Instance { get; private set; }

        public static Action<bool> OnScoreSubmitted;

        [SerializeField] private string leaderboardKey = "spikescape_main";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void InitializeLootLocker()
        {
            LootLockerSDKManager.StartGuestSession((response) =>
            {
                if (response.success)
                {
                    Debug.Log("LootLocker session started successfully.");
                    OnScoreSubmitted?.Invoke(true);
                }
                else
                {
                    Debug.LogError("Failed to start LootLocker session: " + response.errorData);
                    OnScoreSubmitted?.Invoke(false);
                }
            });
        }

        public void SubmitScore(string playerName, int score)
        {
            var timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var metadata = new ScoreMetadata(playerName, timestamp);
            var metadataJson = JsonUtility.ToJson(metadata);
            var memberID = System.Guid.NewGuid().ToString();

            LootLockerSDKManager.SubmitScore(memberID, score, leaderboardKey, metadataJson, (response) =>
            {
                if (response.success)
                {
                    Debug.Log("Score submitted successfully.");
                }
                else
                {
                    Debug.LogError("Failed to submit score: " + response.errorData);
                }
            });
        }
    }
}
