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

        [SerializeField] private string leaderboardKey = "spikescape_main";
        [SerializeField] private int maxTopScores = 100;

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
                    GetMaxTopScores();
                }
                else
                {
                    Debug.LogError("Failed to submit score: " + response.errorData);
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
                    Debug.Log("Top scores retrieved successfully.");
                    var entries = response.items.Select(item =>
                    {
                        var playerName = JsonUtility.FromJson<ScoreMetadata>(item.metadata).playerName;
                        return new LeaderboardEntry(playerName, item.score);
                    }).ToList();

                    OnTopScoresReceived?.Invoke(entries);
                }
                else
                {
                    Debug.LogError("Failed to retrieve top scores: " + response.errorData);
                    OnTopScoresReceived?.Invoke(new List<LeaderboardEntry>());
                }
            });
        }
    }
}
