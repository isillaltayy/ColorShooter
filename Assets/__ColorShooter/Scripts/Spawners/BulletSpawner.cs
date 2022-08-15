using System.Collections;
using System.Collections.Generic;
using __ColorShooter.Scripts.Controllers;
using GameEntities;
using NaughtyAttributes;
using Spawners;
using UnityEngine;
using static UnityEngine.Random;

namespace __ColorShooter.Scripts.Spawners
{
    public class BulletSpawner : MonoBehaviour
    {
        [Expandable]
        private Queue <GameObject> _bulletQueue;
        private List<PooledObject> _bullet;
    
        [HorizontalLine(1,EColor.Pink)]
        [SerializeField] private float topSpeed;
        [SerializeField] private float bottomSpeed;

        [HorizontalLine(1,EColor.Pink)]
        [SerializeField] private int numberOfBullets=100;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float offset;
        [SerializeField] private float _spawnInterval;
        
        private Vector3 _objectsStartPosition;
        private Vector3 _objectsEndPosition;

        
        private void Awake()
        {
            _bulletQueue = new Queue<GameObject>();
            _bullet = new List<PooledObject>();
        }

        private void Start()
        {
            EnqueueBullet();
        }

        private void OnEnable()
        {
            GameManager.Instance.OnGameFinishedSuccessfully += OnDestroy;
            GameManager.Instance.OnGameOver += OnDestroy;
        }
        
        private void OnDisable()
        {
            GameManager.Instance.OnGameFinishedSuccessfully -= OnDestroy;
            GameManager.Instance.OnGameOver -= OnDestroy;
        }

        public void StartSpawning()
        {
            _objectsEndPosition = new Vector3(0, 1.4f, 60f);
            _objectsStartPosition = new Vector3(0, 1.4f, 0f);
            StartCoroutine(CreateBulletItems());
        }

        private Coroutine co;
        private IEnumerator CreateBulletItems()
        {
            yield return new WaitUntil(() => ObjectPool.instance.isPoolSet);
            if (co == null)
            {
                co = StartCoroutine(StartMovingBullet());
            }
        }
        private IEnumerator StartMovingBullet()
        {  
            var bulletObj = _bulletQueue.Dequeue();
            _objectsStartPosition = new Vector3(0, 1.3f, 0f) + spawnPoint.position + spawnPoint.forward * offset;
            
            bulletObj.transform.position = _objectsStartPosition;
            bulletObj.gameObject.SetActive(true);
        
            var bullet = bulletObj.gameObject.GetComponent<Bullet>();
            var speed = Range(bottomSpeed, topSpeed);
            bullet.StartMoving(spawnPoint ,_objectsEndPosition.z, speed);
        
            bullet.OnFinishedMap += FinishedMap;

            _spawnInterval = SpawnSpeedController.Instance.BulletSpawnInterval;
            Debug.LogError("_spawnInterval in bulletSpawner" + _spawnInterval);
            yield return new WaitForSeconds(_spawnInterval);
            StartCoroutine(StartMovingBullet());
        }
        private void FinishedMap(GameObject bullet)
        {
            bullet.transform.position = _objectsStartPosition;
            _bulletQueue.Enqueue(bullet);
        }
        private void EnqueueBullet()
        {
            for (var i = 0; i < numberOfBullets ; i++)
            {
                var obj = ObjectPool.instance.GetPooledObject("Bullet");
                obj.transform.position = _objectsStartPosition;
                _bulletQueue.Enqueue(obj.gameObject);
                _bullet.Add(obj);
            }
        }
        private void OnDestroy()
        {
            co = null;
            StopAllCoroutines();
        }
    }
}

