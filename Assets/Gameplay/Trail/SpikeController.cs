using UnityEngine;
using SpikeScape.Utility.Gameplay;

namespace SpikeScape.Gameplay.Trail
{
    /// <summary>
    /// Controls the behavior of spikes in the game.
    /// </summary>
    public class SpikeController : MonoBehaviour, ITriggerReceiver
    {
        [SerializeField] private float despawnTime = 3f;

        private Coroutine _despawnRoutine;
        private Vector3 _initialScale;

        private void Awake()
        {
            _initialScale = transform.localScale;
        }

        /// <summary>
        /// Animates the spikes to dispear and removes them from the game.
        /// </summary>
        public void Despawn()
        {
            if (_despawnRoutine == null) // Prevent multiple despawn routines
            {
                _despawnRoutine = StartCoroutine(DespawnRoutine());
            }
        }

        public void OnTriggerReceived(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Handle player collision with spikes
                Debug.Log("Player hit spikes!");
                // You can add more logic here, such as reducing player health or restarting the level.
            }
        }

        private System.Collections.IEnumerator DespawnRoutine()
        {
            float elapsedTime = 0f;

            while (elapsedTime < despawnTime)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / despawnTime;

                transform.localScale = Vector3.Lerp(_initialScale, Vector3.zero, t);

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
