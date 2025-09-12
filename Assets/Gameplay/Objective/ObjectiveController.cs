using UnityEngine;
using SpikeScape.Utility.Gameplay;
using System;
using SpikeScape.Audio.Managers;

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
        [SerializeField] private Transform spawnPointsParent;

        private Transform[] _spawnPoints;
        private int _currentSpawnIndex = -1;

        private void Awake()
        {
            if (spawnPointsParent == null)
            {
                Debug.LogError("Spawn Points Parent is not assigned in ObjectiveController.");
                _spawnPoints = Array.Empty<Transform>();
                return;
            }

            _spawnPoints = new Transform[spawnPointsParent.childCount];

            for (int i = 0; i < spawnPointsParent.childCount; i++)
            {
                _spawnPoints[i] = spawnPointsParent.GetChild(i);
            }
        }

        private void Start()
        {
            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                Debug.LogError("No spawn points found under SpawnPointsParent.");
                return;
            }

            if (startWithRandomSpawn)
            {
                MoveToRandomSpawnPoint();
            }
            else
            {
                if (beginningSpawnIndex < 0 || beginningSpawnIndex >= _spawnPoints.Length)
                {
                    Debug.LogWarning("Beginning spawn index is out of range. Using custom objective location.");
                }
                else
                {
                    SetPositionByIndex(beginningSpawnIndex);
                }
            }
        }

        public void OnTriggerReceived(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnObjectiveCollected?.Invoke();

                SoundManager.Instance.PlayCollect();

                MoveToRandomSpawnPoint();
            }
        }

        private void MoveToRandomSpawnPoint()
        {
            if (_spawnPoints.Length == 0)
            {
                Debug.LogError("No spawn points assigned to ObjectiveController.");
                return;
            }

            // Ensure a different spawn point is selected
            int newIndex = UnityEngine.Random.Range(0, _spawnPoints.Length);
            while (newIndex == _currentSpawnIndex && _spawnPoints.Length > 1)
            {
                newIndex = UnityEngine.Random.Range(0, _spawnPoints.Length);
            }

            SetPositionByIndex(newIndex);
        }

        private void SetPositionByIndex(int index)
        {
            if (index < 0 || index >= _spawnPoints.Length)
            {
                Debug.LogError($"Spawn index is out of range, not setting position. Index: {index}");
                return;
            }

            Transform spawnPoint = _spawnPoints[index];
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
            _currentSpawnIndex = index;
        }
    }
}
