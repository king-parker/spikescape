using SpikeScape.Gameplay.Managers;
using UnityEngine;

namespace SpikeScape.Gameplay.DeathFloor
{
    /// <summary>
    /// Represents a death floor in the game that can trigger player death upon contact.
    /// </summary>
    public class DeathFloor : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.GameOver();
            }
        }
    }
}