using UnityEngine;
using SpikeScape.Utility.Gameplay;
using System;

namespace SpikeScape.Gameplay.Objective
{
    /// <summary>
    /// Controls the behavior of objectives in the game.
    /// </summary>
    public class ObjectiveController : MonoBehaviour, ITriggerReceiver
    {
        public static event Action OnObjectiveCollected;

        [SerializeField] private bool startWithRandomSpawn = false;
        [SerializeField] private int beginningSpawnIndex = 0;
        [SerializeField] private Transform[] spawnPoints;

        private int _currentSpawnIndex = -1;

        private void Start()
        {
            if (startWithRandomSpawn)
            {
                MoveToRandomSpawnPoint();
            }
            else
            {
                if (beginningSpawnIndex < 0 || beginningSpawnIndex >= spawnPoints.Length)
                {
                    Debug.LogError("Beginning spawn index is out of range. Defaulting to 0.");
                    beginningSpawnIndex = 0;
                }

                _currentSpawnIndex = beginningSpawnIndex;
            }
        }

        public void OnTriggerReceived(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnObjectiveCollected?.Invoke();

                MoveToRandomSpawnPoint();
            }
        }

        private void MoveToRandomSpawnPoint()
        {
            if (spawnPoints.Length == 0)
            {
                Debug.LogError("No spawn points assigned to ObjectiveController.");
                return;
            }

            // Ensure a different spawn point is selected
            int newIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
            while (newIndex == _currentSpawnIndex && spawnPoints.Length > 1)
            {
                newIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
            }

            Transform newSpawn = spawnPoints[newIndex];
            transform.position = newSpawn.position;
            transform.rotation = newSpawn.rotation;
            _currentSpawnIndex = newIndex;
        }
    }
}
