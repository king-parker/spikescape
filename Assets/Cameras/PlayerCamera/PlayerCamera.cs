using UnityEngine;

namespace SpikeScape.Cameras.PlayerCamera
{
    /// <summary>
    /// A camera that follows the player.
    /// </summary>
    public class PlayerCameraRig : MonoBehaviour
    {
        [SerializeField] private Transform player;         // Player transform
        [SerializeField] private float followSpeed = 5f;   // Smooth follow speed
        [SerializeField] private float rotateSpeed = 120f; // Rotation speed toward player forward

        private void Awake()
        {
            if (player == null) Debug.LogError("PlayerCameraRig: Player transform is not assigned.");
        }

        private void LateUpdate()
        {
            if (player == null) return;

            // Smooth follow
            Vector3 desiredPosition = player.position;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

            // Rotate rig to look along player's forward direction
            Quaternion targetRotation = Quaternion.LookRotation(player.forward, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }
}
