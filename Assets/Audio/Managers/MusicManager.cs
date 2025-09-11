using UnityEngine;

namespace SpikeScape.Audio.Managers
{
    /// <summary>
    /// Manages background music and related audio functionalities.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;

        [Header("Music Clips")]
        [SerializeField] private AudioClip gameplayMusic;
        [SerializeField] private AudioClip gameOverMusic;

        private void OnEnable()
        {
            Gameplay.Managers.GameManager.OnGameOver += HandleGameOver;
        }

        private void OnDisable()
        {
            Gameplay.Managers.GameManager.OnGameOver -= HandleGameOver;
        }

        private void Start()
        {
            if (musicSource == null)
            {
                musicSource = GetComponent<AudioSource>();
            }

            PlayMusic(gameplayMusic);
        }

        private void HandleGameOver()
        {
            PlayMusic(gameOverMusic);
        }

        private void PlayMusic(AudioClip clip)
        {
            if (musicSource.clip == clip) return;

            if (clip == null)
            {
                Debug.LogError("MusicManager: Attempted to play a null AudioClip.");
                return;
            }

            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
}