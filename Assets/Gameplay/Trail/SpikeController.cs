using UnityEngine;
using SpikeScape.Utility.Gameplay;

namespace SpikeScape.Gameplay.Trail
{
    /// <summary>
    /// Controls the behavior of spikes in the game.
    /// </summary>
    public class SpikeController : MonoBehaviour, ITriggerReceiver
    {
        public void OnTriggerReceived(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Handle player collision with spikes
                Debug.Log("Player hit spikes!");
                // You can add more logic here, such as reducing player health or restarting the level.
            }
        }
    }
}
