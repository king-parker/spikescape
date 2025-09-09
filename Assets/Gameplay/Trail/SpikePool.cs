using System.Collections.Generic;
using UnityEngine;

namespace SpikeScape.Gameplay.Trail
{
    /// <summary>
    /// Manages a pool of spike objects for efficient reuse.
    /// </summary>
    public class SpikePool : MonoBehaviour
    {
        [SerializeField] private SpikeController spikePrefab;
        [SerializeField] private int poolSize = 20;
        [SerializeField] private int minPoolSize = 3;
        [SerializeField] private int poolBufferSize = 5;

        private readonly Queue<SpikeController> _pool = new();

        public SpikeController GetSpike(Vector3 position, Quaternion rotation)
        {
            // If the pool is getting low, preemptively add more spikes
            if (_pool.Count <= minPoolSize)
            {
                for (int i = 0; i < poolBufferSize; i++) // number of spikes to add
                {
                    AddSpikeToPool();
                }
            }

            var spike = _pool.Count > 0 ? _pool.Dequeue() : AddSpikeToPool();

            spike.transform.SetPositionAndRotation(position, rotation);
            spike.gameObject.SetActive(true);
            return spike;
        }

        public void ReturnSpike(SpikeController spike)
        {
            spike.gameObject.SetActive(false);
            spike.ResetSize();
            spike.MarkDespawnComplete();
            _pool.Enqueue(spike);
        }

        private void Awake()
        {
            // Pre-populate the pool with spike instances
            for (int i = 0; i < poolSize; i++)
            {
                AddSpikeToPool();
            }
        }

        private SpikeController AddSpikeToPool()
        {
            var spike = Instantiate(spikePrefab, transform); // Parent to the pool for organization
            spike.Initialize(this);
            spike.gameObject.SetActive(false);
            _pool.Enqueue(spike);
            return spike;
        }
    }
}
