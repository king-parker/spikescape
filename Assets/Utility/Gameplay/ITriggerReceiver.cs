namespace SpikeScape.Utility.Gameplay
{
    /// <summary>
    /// Interface for objects that can receive trigger events.
    /// </summary>
    public interface ITriggerReceiver
    {
        /// <summary>
        /// Method called when a trigger event occurs.
        /// </summary>
        /// <param name="other">The collider that entered the trigger.</param>
        void OnTriggerReceived(UnityEngine.Collider other);
    }
}