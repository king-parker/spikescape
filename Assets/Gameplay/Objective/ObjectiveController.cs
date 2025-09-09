using UnityEngine;
using SpikeScape.Utility.Gameplay;

namespace SpikeScape.Gameplay.Objective
{
    /// <summary>
    /// Controls the behavior of objectives in the game.
    /// </summary>
    public class ObjectiveController : MonoBehaviour, ITriggerReceiver
    {
        public void OnTriggerReceived(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Objective reached by player!");
                // Add logic for when the player reaches the objective
            }
        }
    }
}
