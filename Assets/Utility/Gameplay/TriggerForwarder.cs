using UnityEngine;

namespace SpikeScape.Utility.Gameplay
{
    /// <summary>
    /// Script that forwards OnTriggerEnter events to a specified receiver component.
    /// </summary>
    public class TriggerForwarder : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour targetComponent; // The component that will receive the trigger event

        private void OnTriggerEnter(Collider other)
        {
            if (targetComponent is ITriggerReceiver receiver)
            {
                receiver.OnTriggerReceived(other);
            }
            else
            {
                Debug.LogWarning("Target component does not implement ITriggerReceiver.");
            }
        }
    }
}