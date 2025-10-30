using LootLocker.Requests;
using Spikescape.Gameplay.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Spikescape.Leaderboard
{
    public class LootLockerManager : MonoBehaviour
    {
        public static LootLockerManager Instance { get; private set; }

        public static Action<bool> OnScoreSubmitted;
        public static Action<List<LeaderboardEntry>> OnTopScoresReceived;
        public static Action<int> OnLowestHighScoreFound;

        [SerializeField] private string leaderboardKey = "spikescape_main";
        [SerializeField] private int maxTopScores = 100;
        [SerializeField] private float leaderboardRefreshDelay = 1.0f;

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

        private void OnEnable()
        {
            GameManager.OnGameOver += GetMaxTopScores;
        }

        private void OnDisable()
        {
            GameManager.OnGameOver -= GetMaxTopScores;
        }

        public void InitializeLootLocker()
        {
            LootLockerSDKManager.StartGuestSession((response) =>
            {
                if (response.success)
                {
                    Debug.Log("LootLockerManager: LootLocker session started successfully.");
                }
                else
                {
                    Debug.LogError("LootLockerManager: Failed to start LootLocker session: " + response.errorData);
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
                    Debug.Log("LootLockerManager: Score submitted successfully.");
                    OnScoreSubmitted?.Invoke(true);
                    StartCoroutine(RefreshLeaderboardAfterDelay(leaderboardRefreshDelay));
                }
                else
                {
                    Debug.LogError("LootLockerManager: Failed to submit score: " + response.errorData);
                    OnScoreSubmitted?.Invoke(false);
                }
            });
        }

        public void GetMaxTopScores()
        {
            GetTopScores(maxTopScores);
        }

        public void GetTopScores(int count)
        {
            LootLockerSDKManager.GetScoreList(leaderboardKey, count, 0, (response) =>
            {
                if (response.success)
                {
                    Debug.Log("LootLockerManager: Top scores retrieved successfully.");
                    var entries = response.items.Select(item =>
                    {
                        var playerName = JsonUtility.FromJson<ScoreMetadata>(item.metadata).playerName;
                        return new LeaderboardEntry(playerName, item.score);
                    }).ToList();

                    OnTopScoresReceived?.Invoke(entries);

                    if (entries.Count > 0)
                    {
                        OnLowestHighScoreFound?.Invoke(entries.Last().Score);
                    }
                }
                else
                {
                    Debug.LogError("LootLockerManager: Failed to retrieve top scores: " + response.errorData);
                    OnTopScoresReceived?.Invoke(new List<LeaderboardEntry>());
                }
            });
        }

        private System.Collections.IEnumerator RefreshLeaderboardAfterDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            GetMaxTopScores();
        }
    }
}
