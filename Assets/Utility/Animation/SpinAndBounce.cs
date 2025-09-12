using UnityEngine;

namespace SpikeScape.Utility.Animation
{
    /// <summary>
    /// Spins and bounces a GameObject for visual effect.
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class SpinAndBounce : MonoBehaviour
    {
        [Header("Spin Settings")]
        [SerializeField] private float spinSpeed = 90f; // degrees per second

        [Header("Bounce Settings")]
        [SerializeField] private float bounceHeight = 0.5f; // height of the bounce
        [SerializeField] private float bounceSpeed = 2f; // speed of the bounce

        private Vector3 _initialPosition;

        private void Start()
        {
            // Local so it stays relative to the parent
            _initialPosition = transform.localPosition;
        }

        private void Update()
        {
            // Spin the object around its Y-axis
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.Self);

            // Bounce the object up and down using a sine wave
            float newY = _initialPosition.y + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
            transform.localPosition = new Vector3(_initialPosition.x, newY, _initialPosition.z);
        }
    }
}
