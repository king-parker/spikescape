using Spikescape.Leaderboard;
using Spikescape.UI.Leaderboard;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    [SerializeField] private Transform leaderboardContent;
    [SerializeField] private GameObject leaderboardEntryPrefab;

    private void OnEnable()
    {
        LootLockerManager.OnTopScoresReceived += UpdateLeaderboard;
    }

    private void OnDisable()
    {
        LootLockerManager.OnTopScoresReceived -= UpdateLeaderboard;
    }

    public void UpdateLeaderboard(List<LeaderboardEntry> entries)
    {
        // Clear existing entries
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }
        // Populate new entries
        for (int i = 0; i < entries.Count; i++)
        {
            var entryData = entries[i];
            var entryObject = Instantiate(leaderboardEntryPrefab, leaderboardContent);

            var entryController = entryObject.GetComponent<LeaderboardEntryController>();
            entryController.Rank = i + 1;
            entryController.PlayerName = entryData.PlayerName;
            entryController.Score = entryData.Score;
        }
    }
}
