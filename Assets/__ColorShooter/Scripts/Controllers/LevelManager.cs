using System;
using System.Collections.Generic;
using UnityEngine;

namespace __ColorShooter.Scripts.Controllers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<GameEntities.Level> levels = new List<GameEntities.Level>();
        [SerializeField] private int currentLevelIndex = 0;
        [SerializeField] private GameEntities.Level currentLevel;

        private void OnEnable()
        {
            GameManager.Instance.OnGameStart += OnGameStart;
            GameManager.Instance.OnGameFinishedSuccessfully += OnFinishedSuccessfullySuccesful;
            GameManager.Instance.OnGameOver += OnGameOver;
        }
        private void OnDisable()
        {
            GameManager.Instance.OnGameStart -= OnGameStart;
            GameManager.Instance.OnGameFinishedSuccessfully -= OnFinishedSuccessfullySuccesful;
            GameManager.Instance.OnGameOver -= OnGameOver;
        }

        private void OnFinishedSuccessfullySuccesful()
        {
           LevelWin();
        }
        private void OnGameStart()
        {
            Awake();
        }
        private void OnGameOver()
        {
            GameOver();
        }
        public GameEntities.Level CurrentLevel => currentLevel;

        private void Awake()
        {
            currentLevelIndex= PlayerPrefs.GetInt("CurrentLevel", 0);
            currentLevel = levels[currentLevelIndex];
        }
        public void LevelWin()
        {
            PlayerPrefs.SetInt("CurrentLevel", currentLevelIndex + 1);
            currentLevelIndex++;
            NextLevelStart();
        }

        private void NextLevelStart()
        {
            currentLevelIndex= PlayerPrefs.GetInt("CurrentLevel", 0);
            currentLevel = levels[currentLevelIndex];
            
        }
        public void GameOver()
        {
            Debug.LogError("Game Over");
            PlayerPrefs.SetInt("CurrentLevel", currentLevelIndex);
            LoadSameLevel();
        }

        private void LoadSameLevel()
        {
            currentLevelIndex= PlayerPrefs.GetInt("CurrentLevel", 0);
            currentLevel = levels[currentLevelIndex];
            Debug.LogError("Load Same Level" + currentLevelIndex);
        }
    }
}
