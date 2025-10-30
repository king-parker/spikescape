using UnityEngine;

namespace Spikescape.Audio.Managers
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

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 1f;

        private Coroutine _fadeCoroutine;

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
            PlayMusic(gameOverMusic, useFade: true);
        }

        private void PlayMusic(AudioClip clip, bool useFade = false)
        {
            if (musicSource.clip == clip) return;

            if (clip == null)
            {
                Debug.LogError("MusicManager: Attempted to play a null AudioClip.");
                return;
            }

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            if (useFade && musicSource.isPlaying)
            {
                _fadeCoroutine = StartCoroutine(FadeOutIn(clip));
            }
            else
            {
                musicSource.Stop();
                musicSource.clip = clip;
                musicSource.loop = true;
                musicSource.Play();
            }
        }

        private System.Collections.IEnumerator FadeOutIn(AudioClip newClip)
        {
            // Fade out
            var startVolume = musicSource.volume;
            var elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
                yield return null;
            }

            musicSource.Stop();
            musicSource.clip = newClip;
            musicSource.Play();

            // Fade in
            elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(0f, startVolume, elapsed / fadeDuration);
                yield return null;
            }

            musicSource.volume = startVolume;
        }
    }
}