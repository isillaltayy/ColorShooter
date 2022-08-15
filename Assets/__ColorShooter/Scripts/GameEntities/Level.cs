using System.Collections.Generic;
using GameEntities;

namespace __ColorShooter.Scripts.GameEntities
{
    [System.Serializable]
    public class Level
    {
        public int targetEnemyCount;
        public List<Enemy> enemyPrefabs = new List<Enemy>();
    }
}
