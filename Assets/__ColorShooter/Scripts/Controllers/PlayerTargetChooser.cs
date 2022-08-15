using System.Collections.Generic;
using System.Linq;
using __ColorShooter.Scripts.Controllers;
using __ColorShooter.Scripts.GameEntities;
using ColorControllers;
using GameEntities;
using UnityEngine;

namespace __ColorShooter.Scripts.ColorControllers
{
    public class PlayerTargetChooser : MonoBehaviour
    {
        [SerializeField] private Color playerColor;
        [SerializeField] private  List<Enemy> currentEnemies = new List<Enemy>();
    
        private EnemyColorList _enemyColorList;
        private EnemyGroup _currentEnemyGroup;

        [SerializeField] GameObject fovStartPoint;
        [SerializeField] private float rotationSpeed;
    
        private  Quaternion _targetRotation;
        private  Quaternion _lookAt;
    
        private float maxAngle = 150;
        private float maxAngleReset = 150;
        private bool _isSpawning = false;

        private void Start()
        {
            _enemyColorList = FindObjectOfType<EnemyColorList>();
        }
    
        private void OnEnable()
        {
            ColorChangeController.StartSpawning += StartSpawning;
            Enemy.OnEnemyDeathCount += OnEnemyDeathCount;
        }
        private void OnDisable()
        {
            ColorChangeController.StartSpawning -= StartSpawning;
            Enemy.OnEnemyDeathCount -= OnEnemyDeathCount;
            _isSpawning = false;
        }

        private void OnEnemyDeathCount()
        {
            _currentEnemyGroup.RemoveEnemy(currentEnemies[0]);
            ChooseEnemyList();
        }

        private void StartSpawning()
        {
            _isSpawning = true;
        }

        private void ChooseEnemyList()
        {
            if (_isSpawning)
            {
                for (int i = 0; i < _enemyColorList.EnemyGroups.Count; i++)
                {
                    if (playerColor == _enemyColorList.EnemyGroups[i].groupColor)
                    {
                        _currentEnemyGroup = _enemyColorList.EnemyGroups[i];
                    }
                }
                currentEnemies =
                    _currentEnemyGroup.enemies.OrderBy((x) => Vector3.Distance(x.transform.position, transform.position)).ToList();
            }
        }

        public void ChangeColor(Color color)
        {
            playerColor = color;
        }
        private void Update()
        {
            PlayerTarget();
            ChooseEnemyList();
        }
        private void PlayerTarget()
        {
            if (currentEnemies.Count > 0)
            {
                if (EnemyInFieldOfView(fovStartPoint) || true)
                {
                    Enemy tempEnemy = currentEnemies[0];
                    //float distance = Vector3.Distance(tempEnemy.transform.position + tempEnemy.transform.forward * 2, transform.position);
                    Vector3 tempPos = tempEnemy.transform.position + tempEnemy.transform.forward * 1;
                    var direction = tempPos - transform.position;
                    _targetRotation = Quaternion.LookRotation(direction);
                    _lookAt = Quaternion.RotateTowards(transform.rotation, _targetRotation, Time.deltaTime * rotationSpeed);
                    transform.rotation = _lookAt;
                }
                else if (EnemyInFieldOfViewNoResetPoint(fovStartPoint))
                {
                    return;
                }
                else
                {
                    _targetRotation = Quaternion.Euler(0, 0, 0);
                    transform.localRotation = Quaternion.RotateTowards(
                        transform.localRotation, _targetRotation, Time.deltaTime * rotationSpeed);
                    //_targetRotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * 10);
                }
            }

        }
        private bool EnemyInFieldOfView(GameObject looker)
        {
            var targetDirection = currentEnemies[0].transform.position - transform.position;
            var angle = Vector3.Angle(targetDirection,looker.transform.forward);
        
            if(angle<maxAngle)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool EnemyInFieldOfViewNoResetPoint(GameObject looker)
        {
            var targetDirection = currentEnemies[0].transform.position - transform.position;
            var angle = Vector3.Angle(targetDirection,looker.transform.forward);
        
            if(angle<maxAngleReset)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
