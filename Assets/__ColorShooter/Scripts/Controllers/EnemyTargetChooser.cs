using System.Collections;
using UnityEngine;

namespace __ColorShooter.Scripts.Controllers
{
    public class EnemyTargetChooser : MonoBehaviour
    {
        private  Transform _towerTransform;
        private int _target;
        
        private void OnEnable()
        {
            TowerDamageController.OnDeath += OnDeath;
        }
        private void OnDeath(GameObject tower)
        {
            StartCoroutine(OnDeatStarter(tower));
        }
        private void OnDisable()
        {
            TowerDamageController.OnDeath -= OnDeath;
        }
        private IEnumerator OnDeatStarter(GameObject tower)
        {
            yield return new WaitForEndOfFrame();
            SetTarget();
        }
        private void SetTarget()
        {
            if (GameManager.Instance.GetTower())
            {
                _towerTransform = GameManager.Instance.GetTower().transform;
            }
        }
        public Transform TowerTransform
        {
            get => _towerTransform; 
        }
        private void Start()
        {
            SetTarget();
        }
    }
}
