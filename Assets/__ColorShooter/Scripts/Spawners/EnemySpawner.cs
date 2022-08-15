using System;
using System.Collections;
using System.Collections.Generic;
using GameEntities;
using NaughtyAttributes;
using Spawners;
using UnityEngine;
using __ColorShooter.Scripts.Controllers;
using __ColorShooter.Scripts.GameEntities;
using ColorControllers;
using static UnityEngine.Random;
using Random = UnityEngine.Random;

namespace __ColorShooter.Scripts.Spawners
{
    public class EnemySpawner : MonoBehaviour
    {
        [Expandable]
        private Queue<GameObject> enemyQueue;
        private List<PooledObject> enemy;

        [HorizontalLine(1,EColor.Pink)]
        [SerializeField] private float topSpeed;
        [SerializeField] private float bottomSpeed;
        
        [HorizontalLine(1,EColor.Pink)]
        [SerializeField] private int numberOfEnemyPrefabs;
        [SerializeField] private int numberOfEnemy = 100;
        [SerializeField] private float spawnedPointZ;
        private float _enemySpawnInterval;
        private Vector3 _objectsStartPosition;
       
        [HorizontalLine(1,EColor.Pink)]
        [SerializeField] private List<Color> selectedColors = new List<Color>();
        
        [SerializeField] private List<Enemy> _spawnedEnemies = new List<Enemy>();
        private void Awake()
        {
            enemyQueue = new Queue<GameObject>();
            enemy = new List<PooledObject>();
        }
        private void OnEnable()
        {
            ColorPalette.OnColorPaletteSet += SetColor;
            GameManager.Instance.OnGameFinishedSuccessfully += DestroyEnemy;
            GameManager.Instance.OnGameOver += OnGameOver;
        }
        private void OnDisable()
        {
            ColorPalette.OnColorPaletteSet -= SetColor;
            GameManager.Instance.OnGameFinishedSuccessfully -= DestroyEnemy;
            GameManager.Instance.OnGameOver -= OnGameOver;
        }
        private void SetColor(List<Color> colors)
        {
            //enemy color palette
            selectedColors.AddRange(colors);
        }
        private void Start()
        {
            //EnqueueEnemy();
        }
        public void EnqueueEnemy()
        {
            GameEntities.Level level = FindObjectOfType<LevelManager>().CurrentLevel;
            enemyQueue.Clear();
            for (var i = 0; i < numberOfEnemy ; i++)
            {
                string name = level.enemyPrefabs[Random.Range(0, level.enemyPrefabs.Count)].gameObject.name;
                var obj = ObjectPool.instance.GetPooledObject(name);
                enemyQueue.Enqueue(obj.gameObject);
                enemy.Add(obj);
            }
        }
        public void StartSpawning()
        {
            _objectsStartPosition = new Vector3(0, 0, spawnedPointZ);
            StartCoroutine(CreateRoadItems());
        }
        
        private Coroutine _spawningCoroutine;
        private IEnumerator CreateRoadItems()
        {
            yield return new WaitUntil(() => ObjectPool.instance.isPoolSet);
            if (_spawningCoroutine == null)
            {
                _spawningCoroutine = StartCoroutine(StartMovingEnemy());
            }
        }
        private IEnumerator StartMovingEnemy()
        {
            if (!_spawnControl)
            {
                var enemyObj = enemyQueue.Dequeue();

                _objectsStartPosition = new Vector3(Range(-4f, 4f), 0, spawnedPointZ);
                enemyObj.transform.position = _objectsStartPosition;
                enemyObj.transform.rotation = Quaternion.Euler(0, 180, 0);
                enemyObj.gameObject.SetActive(true);

                var enemy = enemyObj.GetComponent<Enemy>();
                _spawnedEnemies.Add(enemy);

                enemy.SetMyColor(selectedColors[Range(0, selectedColors.Count)]);
                enemy.StartMoving();

                enemy.OnFinishedMap += OnEnemyFinishedMap;

                _enemySpawnInterval = SpawnSpeedController.Instance.EnemySpawnInterval;
            }
            yield return new WaitForSeconds(_enemySpawnInterval);
            StartCoroutine(StartMovingEnemy());
        }
        private void OnEnemyFinishedMap(GameObject enemy)
        {
            enemy.transform.position = _objectsStartPosition;
            enemyQueue.Enqueue(enemy);
            enemy.GetComponent<Enemy>().OnFinishedMap -= OnEnemyFinishedMap; 
        }
        private void DestroyEnemy()
        {
            StartCoroutine(EnemyDeathAnimation());
        }
        private void OnGameOver()
        {
            StartCoroutine(EnemyDanceAnimation());  
        }
        private IEnumerator EnemyDanceAnimation()
        {
            _spawnControl = true;
            for (int i = 0; i < _spawnedEnemies.Count; i++)
            {
                Enemy tempEnemy = _spawnedEnemies[i].GetComponent<Enemy>();
                tempEnemy.LevelUnsuccessfullyFinished();
            }
            yield return new WaitForSeconds(4);
            _spawnedEnemies.Clear();
            for (int i = 0; i < enemy.Count; i++)
            {
                ObjectPool.instance.TakeBack(enemy[i]);
            }
            OnDestroy();
        }
        private bool _spawnControl;
        private IEnumerator EnemyDeathAnimation()
        {
            _spawnControl = true;
            for (int i = 0; i < _spawnedEnemies.Count; i++)
            {
                Enemy tempEnemy = _spawnedEnemies[i].GetComponent<Enemy>();
                tempEnemy.LevelSuccessfullyFinished();
                // _spawnedEnemies[i].gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(4);
            _spawnedEnemies.Clear();
            for (int i = 0; i < enemy.Count; i++)
            {
                ObjectPool.instance.TakeBack(enemy[i]);
            }
            OnDestroy();
        }
        private void OnDestroy()
        {
            _spawnControl = false;
            _spawningCoroutine = null;
            StopAllCoroutines();
        }
    }
}
