using System;
using System.Collections.Generic;
using __ColorShooter.Scripts.Spawners;
using RoosterHub;
using Spawners;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __ColorShooter.Scripts.Controllers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private bool _isDevelopmentMode;
        private EnemySpawner _enemySpawner;
        private BulletSpawner _bulletSpawner;
        public event  Action OnGameStart;
        public event Action OnGameFinishedSuccessfully;
        public event Action OnGameOver;

        public static Action PlayerStartMovement;
        public static GameManager Instance;

        [SerializeField] private bool _isGameFinished;
        public bool IsGameFinished => _isGameFinished;

        [SerializeField] private List<GameObject> _towers;
        [SerializeField] private List<GameObject> _tempTowers;
        public List<GameObject> Towers => _towers;
        

        public GameObject GetTower()
        {
            if (_towers.Count == 0)
            {
                return null;
            }
            else
            {
                var towerIndex= Random.Range(0, _towers.Count);
                return _towers[towerIndex];
            }
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
               Destroy(gameObject);
            _enemySpawner = FindObjectOfType<EnemySpawner>();
            _bulletSpawner = FindObjectOfType<BulletSpawner>();
            _tempTowers.AddRange(_towers);
        }
        private void Start()
        {
            ObjectPool.instance.StartPool();
        }

        private void OnEnable()
        {
            ColorChangeController.StartSpawning += StartBulletSpawning;
            Central.OnGameStartedHandler += StartGame;
            TowerDamageController.OnDeath += OnDeathOfTower;
        }
        private void OnDisable()
        {
            ColorChangeController.StartSpawning -= StartBulletSpawning;
            Central.OnGameStartedHandler -= StartGame;
            TowerDamageController.OnDeath -= OnDeathOfTower;
        }
        
        private void OnDeathOfTower(GameObject tower)
        {
            _towers.Remove(tower);
            if (_towers.Count == 0)
            {
                RoosterHub.Central.Fail();
                GameOver();
            }
        }
        private void Update()
        {
            if (_isDevelopmentMode)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    Debug.LogError("CLICKED");
                    StartGame();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Central.SetIncome(500,true);
            }
        }
        public void WinLevel()
        {
            _isGameFinished = false;
            OnGameFinishedSuccessfully?.Invoke();
            RoosterHub.Central.Win();
        }
        public void GameOver()
        {
            _isGameFinished = false;
            OnGameOver?.Invoke();
            _towers.AddRange(_tempTowers);
        }
        private void StartGame()
        {
            if(_isGameFinished)
                return;
            
            Debug.LogError("Game Started"); 
            _isGameFinished = true;
            OnGameStart?.Invoke();
            PlayerStartMovement?.Invoke();

            SpawnEnemy();
        }

        private void StartBulletSpawning()
        {
            SpawnBullet();
        }
        
        private void SpawnBullet()
        {
            _bulletSpawner.StartSpawning();
        }
        private bool _isEnemySpawned;
        private void SpawnEnemy()
        {
            _enemySpawner.EnqueueEnemy();
            _enemySpawner.StartSpawning();
        }
    }
}
