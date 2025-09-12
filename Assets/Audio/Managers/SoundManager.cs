using UnityEngine;

namespace SpikeScape.Audio.Managers
{
    /// <summary>
    /// Handles sound effects in the game.
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSourcePrefab; // prefab with AudioSource
        [SerializeField] private int poolSize = 5;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip jumpSFX;
        [SerializeField] private AudioClip collectSFX;
        [SerializeField] private AudioClip gameOverSFX;

        private AudioSource[] _sfxPool;
        private int _poolIndex = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Build a small pool so we can overlap sounds
            _sfxPool = new AudioSource[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                _sfxPool[i] = Instantiate(sfxSourcePrefab, transform);
            }
        }

        public void PlayJump() => PlaySFX(jumpSFX);
        public void PlayCollect() => PlaySFX(collectSFX);
        public void PlayGameOver() => PlaySFX(gameOverSFX);

        /// <summary>
        /// Plays a sound effect once with slight randomization in pitch and volume.
        /// </summary>
        public void PlaySFX(AudioClip clip)
        {
            // Randomize pitch and volume slightly for variety
            var volume = Random.Range(0.8f, 1f);
            var pitch = Random.Range(0.9f, 1.1f);

            PlaySFX(clip, volume, pitch);
        }

        /// <summary>
        /// Plays a sound effect once at default volume/pitch.
        /// </summary>
        public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            if (clip == null) return;

            var source = _sfxPool[_poolIndex];
            _poolIndex = (_poolIndex + 1) % _sfxPool.Length;

            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.Play();
        }
    }
}
