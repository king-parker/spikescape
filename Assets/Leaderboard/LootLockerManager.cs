using LootLocker.Requests;
using UnityEngine;

namespace Spikescape.Leaderboard
{
    public class LootLockerManager : MonoBehaviour
    {
        public static LootLockerManager Instance { get; private set; }

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
                }
                else
                {
                    Debug.LogError("Failed to start LootLocker session: " + response.errorData);
                }
            });
        }
    }
}
