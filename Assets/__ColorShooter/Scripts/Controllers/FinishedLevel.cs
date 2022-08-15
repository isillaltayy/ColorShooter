using __ColorShooter.Scripts.GameEntities;
using GameEntities;
using UnityEngine;

namespace __ColorShooter.Scripts.Controllers
{
    public class FinishedLevel : MonoBehaviour
    {
        public delegate void OnEnemyCount(int enemyCount);
        public static event OnEnemyCount OnEnemyCountChanged;

        private int _enemyCount;
        private int _enemyKilled;
        private bool _isFinished;

        public bool Finished
        {
            get => _isFinished;
        }
        public int EnemyCount
        {
            get => _enemyCount;
        }
        public int EnemyKilled
        {
            get => _enemyKilled;
        }
        
        // private void Start()
        // {
        //     _enemyCount= FindObjectOfType<LevelManager>().CurrentLevel.targetEnemyCount;
        //     print(_enemyCount);
        //     _enemyKilled = 0;
        // }

        private void OnEnable()
        {
            GameManager.Instance.OnGameStart += OnGameStart;
            //Enemy.OnEnemyDeathCount += OnEnemyDeath;
            Enemy.OnEnemyDeathCount += OnEnemyDeath;
        }
        
        private void OnGameStart()
        {
            _enemyCount= FindObjectOfType<LevelManager>().CurrentLevel.targetEnemyCount;
            print("StartEnemyCount: "+_enemyCount);
            _enemyKilled = 0;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnGameStart -= OnGameStart;
            //Enemy.OnEnemyDeathCount -= OnEnemyDeath;
            Enemy.OnEnemyDeathCount -= OnEnemyDeath;
        }

        private void OnEnemyDeath()
        {
            _enemyKilled++;
          
            if (_enemyKilled == _enemyCount)
            {
                //TODO  
                GameManager.Instance.WinLevel();
                _isFinished = true;
                _enemyKilled = 0;
            }
            OnEnemyCountChanged?.Invoke(_enemyKilled);
        }
    }
}
