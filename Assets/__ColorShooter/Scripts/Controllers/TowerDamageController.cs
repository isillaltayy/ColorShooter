using System;
using DG.Tweening;
using RoosterHub;
using UnityEngine;
using UnityEngine.UI;

namespace __ColorShooter.Scripts.Controllers
{
    public class TowerDamageController : MonoBehaviour
    {
        [SerializeField] Image healthBar;
        [SerializeField] Renderer _renderer;
       
        private float startHealth =100;
        private float upgradeHealth = 25;
        private float _damage = 10f;
        private float _health;
        private Material[] _materials;

        [SerializeField] private GameObject towerDisableControl;

        public delegate void OnDeathTower(GameObject tower);
        public static event OnDeathTower OnDeath;

        private void OnEnable()
        {
            GameManager.Instance.OnGameStart += OnGameStart;
            StartUpgrades.OnUpgradePurchased+= OnUpgradePurchased;
        }
        private void OnDisable()
        {
            GameManager.Instance.OnGameStart -= OnGameStart;
            StartUpgrades.OnUpgradePurchased -= OnUpgradePurchased;
        }
        
        private void OnUpgradePurchased(StartingUpgradeArea.DTO obj)
        {
            switch (obj.upgradeName)
            {
                case "Stamina":
                    startHealth += upgradeHealth;
                    break;
            }
        }

        private void OnGameStart()
        {
            _health = startHealth;
            healthBar.fillAmount = _health / startHealth;
            foreach (var material in _materials)
            {
                material.SetFloat("_Fill", 0.3f);
            }
            _renderer.materials = _materials;

            towerDisableControl.SetActive(true);
        }

        private void Awake()
        {
            _materials = _renderer.materials;
        }


        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Enemy"))
            {
                TakeDamage();
            }
        }

        private void TakeDamage()
        {
            _health -= _damage;
            healthBar.fillAmount = _health / startHealth;
            OnHitAnimation();
            foreach (var material in _materials)
            {
                float val = healthBar.fillAmount * 0.3f;
                material.SetFloat("_Fill", val);
            }
            _renderer.materials = _materials;
            if (_health <= 0)
            {
                Debug.LogError("Tower disable: " + _health);
                OnDeath?.Invoke(gameObject);
                towerDisableControl.SetActive(false);
            }
        }
        
        void OnHitAnimation()
        {
            float rndX = UnityEngine.Random.RandomRange(.5f, 2f);
            float rndZ = UnityEngine.Random.RandomRange(.8f, 1.79f);
            transform.DOPunchRotation(new Vector3(rndX, 0, rndZ), .2f, 2);
        }
    }
}
