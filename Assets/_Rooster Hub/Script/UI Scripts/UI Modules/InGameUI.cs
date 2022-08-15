using System;
using __ColorShooter.Scripts.Controllers;
using TMPro;
using UnityEngine;

namespace RG.Loader
{
    public class InGameUI : UIObject
    {
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI enemyText;
        
        private int _enemyCount;
        private float _score;
        private float _coin;

        private void Start()
        {
            FinishedLevel.OnEnemyCountChanged += OnEnemyCountChanged;
        }

        private void OnEnable()
        {
            GameManager.Instance.OnGameStart += OnGameStarted;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnGameStart -= OnGameStarted;
        }

        private void OnGameStarted()
        {
            Debug.LogError(FindObjectOfType<FinishedLevel>());
            _enemyCount = FindObjectOfType<FinishedLevel>().EnemyCount;
            enemyText.text = "Last: " +_enemyCount.ToString();
        }
        private void OnEnemyCountChanged(int killedEnemyCount)
        {
            var lastEnemyCount = _enemyCount - killedEnemyCount;
            enemyText.text = "Last: " + lastEnemyCount.ToString();
        }
        private void OnDestroy()
        {
            FinishedLevel.OnEnemyCountChanged -= OnEnemyCountChanged;
        }
    }   
}

