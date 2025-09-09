using System.Collections.Generic;
using UnityEngine;

namespace SpikeScape.Gameplay.Trail
{
    /// <summary>
    /// Spawns the spike trail behind the player at regular intervals.
    /// </summary>
    public class SpikeTrailSpawner : MonoBehaviour
    {
        [Header("Spike Trail Settings")]
        [SerializeField] private GameObject spikeTrailPrefab;
        [SerializeField] private float spawnDistance = 1f;
        [SerializeField] private int maxSpikes = 10;
        [SerializeField] private int spikeIncreasePerObjective = 1;

        [Header("Spike Pool Settings")]
        [SerializeField] private SpikePool spikePool;

        private Vector3 _lastSpawnPosition;
        private readonly List<SpikeController> _spawnedSpikes = new();

        private void OnEnable()
        {
            Objective.ObjectiveController.OnObjectiveCollected += HandleObjectiveCollected;
        }

        private void OnDisable()
        {
            Objective.ObjectiveController.OnObjectiveCollected -= HandleObjectiveCollected;
        }

        private void Start()
        {
            _lastSpawnPosition = transform.position;
            SpawnSpike(_lastSpawnPosition);
        }

        private void Update()
        {
            float distanceSinceLastSpawn = Vector3.Distance(transform.position, _lastSpawnPosition);
            if (distanceSinceLastSpawn >= spawnDistance)
            {
                var direction = (transform.position - _lastSpawnPosition).normalized;
                var spawnPosition = _lastSpawnPosition + direction * spawnDistance;
                SpawnSpike(spawnPosition);
                _lastSpawnPosition = transform.position;
            }
        }

        private void SpawnSpike(Vector3 spawnPosition)
        {
            var spike = spikePool.GetSpike(spawnPosition, Quaternion.identity);
            _spawnedSpikes.Add(spike);

            // Remove oldest spike if exceeding maxSpikes
            if (_spawnedSpikes.Count > maxSpikes)
            {
                _spawnedSpikes[0].Despawn();
                _spawnedSpikes.RemoveAt(0);
            }
        }

        private void HandleObjectiveCollected()
        {
            IncreaseMaxSpikes();
        }

        private void IncreaseMaxSpikes()
        {
            maxSpikes += spikeIncreasePerObjective;
        }
    }
}
