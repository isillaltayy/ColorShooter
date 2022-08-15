using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using __ColorShooter.Scripts.GameEntities;
using GameEntities;
using UnityEngine;

namespace ColorControllers
{
    public class EnemyColorList : MonoBehaviour
    {
        [SerializeField] private List<EnemyGroup> enemyGroups = new List<EnemyGroup>();
    
        public List<EnemyGroup> EnemyGroups { get => enemyGroups; }

        private void OnEnable()
        {
            ColorPalette.OnColorPaletteSet += OnColorPaletteSet;
            Enemy.OnEnemyDisabled += OnEnemyDisabled;
        }
        private void OnDisable()
        {
            ColorPalette.OnColorPaletteSet -= OnColorPaletteSet;
            Enemy.OnEnemyDisabled -= OnEnemyDisabled;
        }
        private void OnEnemyDisabled(Enemy enemy)
        {
            for (int i = 0; i < enemyGroups.Count; i++)
            {
                enemyGroups[i].RemoveEnemy(enemy);
            } 
        }
        private void OnColorPaletteSet(List<Color> obj)
        {
            for (int i = 0; i < obj.Count; i++)
            {
                enemyGroups.Add(new EnemyGroup(obj[i]));
            }
        }
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Enemy enemy))
            {
                for (int i = 0; i < enemyGroups.Count; i++)
                {
                    if(enemyGroups[i].groupColor == enemy.EnemyColor)
                    {
                        enemyGroups[i].AddEnemy(enemy);
                    }
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Enemy enemy))
            {
                for (int i = 0; i < enemyGroups.Count; i++)
                {
                    if(enemyGroups[i].groupColor == enemy.EnemyColor)
                    {
                        enemyGroups[i].RemoveEnemy(enemy);
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public Color groupColor;
        public List<Enemy> enemies = new List<Enemy>();

        public EnemyGroup(Color color)
        {
            groupColor = color;
        }

        public void AddEnemy(Enemy enemy)
        {
            if (!enemies.Contains((enemy)))
            {
                enemies.Add(enemy);
            }
        
        }
        public void RemoveEnemy(Enemy enemy)
        {
            if (enemies.Contains((enemy)))
            {
                enemies.Remove(enemy);
            }
        
        }
    }
}