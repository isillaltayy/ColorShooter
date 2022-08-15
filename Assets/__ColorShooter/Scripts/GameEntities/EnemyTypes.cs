using System;
using RoosterHub;
using UnityEngine;

namespace __ColorShooter.Scripts.GameEntities
{
    [CreateAssetMenu(fileName = "New Enemy Type", menuName = "Enemy Type")]
    public class EnemyTypes : ScriptableObject
    {
        public float health;
        public float healthDamage;
        public float speed;
        public float damage;
        public Vector3 enemyScale;
        public float coinValue;
        public float enemyScoreValue;

        private void OnEnable()
        {
            StartUpgrades.OnUpgradePurchased+= OnUpgradePurchased;
        }

        private void OnDisable()
        {
           
            StartUpgrades.OnUpgradePurchased-= OnUpgradePurchased;
        }
        private void OnUpgradePurchased(StartingUpgradeArea.DTO obj)
        {
            switch (obj.upgradeName)
            {
                case "Income":
                    Debug.LogError("Income : " +obj.upgradeNo);
                    Debug.LogError("Before Coin : " + coinValue);
                    coinValue += 5f;
                    Debug.LogError("After Coin : " + coinValue);
                    break;
            }
        }
    }
    
}
