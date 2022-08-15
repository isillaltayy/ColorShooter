using System;
using __ColorShooter.Scripts.Spawners;
using RoosterHub;
using UnityEngine;

namespace __ColorShooter.Scripts.Controllers
{
    public class SpawnSpeedController : MonoBehaviour
    {
        private float _bulletSpawnInterval = 0.8f;
        private float _enemySpawnInterval = 3f;
        public static SpawnSpeedController Instance;
        private void Awake()
        {
            if(Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        private void OnEnable()
        {
            StartUpgrades.OnUpgradePurchased+= OnUpgradePurchased;
            GameManager.Instance.OnGameStart += OnGameStarted;
        }
        private void OnDisable()
        {
            StartUpgrades.OnUpgradePurchased -= OnUpgradePurchased;
            GameManager.Instance.OnGameStart -= OnGameStarted;
        }
        private void OnGameStarted()
        {
            if(EnemySpawnInterval < 0.9f)
                Debug.LogError("EnemySpawnInterval is less than 1f");
            else
                EnemySpawnInterval -= 0.03f;
        }
        private void OnUpgradePurchased(StartingUpgradeArea.DTO obj)
        {
            switch (obj.upgradeName)
            {
                case "Bullet Speed":
                    if (_bulletSpawnInterval < 0.1f)
                    {
                        Debug.LogError("Bullet Spawn Interval is full");
                    }
                    else
                    {
                        _bulletSpawnInterval -= 0.1f;
                        BulletSpawnInterval = _bulletSpawnInterval;
                        Debug.LogError("Bullet Spawn Interval after updated : " + BulletSpawnInterval);
                    }
                    break;
            }
        }
        public float BulletSpawnInterval
        {
            get => PlayerPrefs.GetFloat("BulletSpawnInterval", 1f);
            set => PlayerPrefs.SetFloat("BulletSpawnInterval", value);
        }
        public float EnemySpawnInterval
        {
            get => PlayerPrefs.GetFloat("EnemySpawnInterval", 3f);
            set => PlayerPrefs.SetFloat("EnemySpawnInterval", value);
        }
    }
}
