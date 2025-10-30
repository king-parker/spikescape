using Spikescape.Leaderboard;
using Spikescape.Audio.Managers;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Spikescape.Gameplay.Managers
{
    /// <summary>
    /// Manages overall game state and behavior.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static Action OnGameOver;

        public static GameManager Instance { get; private set; }

        public enum GameState
        {
            MainMenu,
            Playing,
            Paused,
            GameOver
        }

        public GameState CurrentState { get; private set; } = GameState.Playing; // TODO: Default to MainMenu when implemented

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            LootLockerManager.Instance.InitializeLootLocker();
        }

        public void GameOver()
        {
            if (CurrentState == GameState.GameOver) return;

            SoundManager.Instance.PlayGameOver();
            CurrentState = GameState.GameOver;
            Time.timeScale = 0f;

            OnGameOver?.Invoke();
        }

        public void PauseGame()
        {
            if (CurrentState != GameState.Playing) return;
            CurrentState = GameState.Paused;
            Time.timeScale = 0f;
            Debug.Log("Game Paused");
        }

        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused) return;
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            Debug.Log("Game Resumed");
        }

        public void RestartGame()
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;

            // TODO: Replace when main menu is implemented
            // Reload the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}