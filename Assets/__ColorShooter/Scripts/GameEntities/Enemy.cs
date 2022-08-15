using System;
using System.Collections;
using __ColorShooter.Scripts.Controllers;
using DG.Tweening;
using NaughtyAttributes;
using RoosterHub;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __ColorShooter.Scripts.GameEntities
{
    public class Enemy : MonoBehaviour
    {
        [Expandable]

        [HorizontalLine(1, EColor.Pink)] [SerializeField]
        private EnemyTypes _enemyType = null;
        //enemy personal properties
        private float _speed;
        private float _damage;
        private float _healthDamage;
        private float _health;
        private float _coinValue;
        private float _scoreValue;

        private Vector3 _firstScaleValue;
        
        [HorizontalLine(1,EColor.Pink)]
        //enemy movement properties
        [SerializeField] private bool isMoving;
        [SerializeField] internal bool isActive;
        [SerializeField] internal bool isFirst;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Renderer _ragdoll;
        [SerializeField] internal Transform _normalChar;
        [SerializeField] internal Transform _ragdollChar;
        [SerializeField] private GameObject rotationDestination; 
        private float _enemyEndPosition;
        private bool _isDead;

        [HorizontalLine(1,EColor.Pink)]
        //enemy particles properties
        [SerializeField] private Color _color;
        [SerializeField] new Renderer deathEffectColor;
        [SerializeField] new Renderer deathEffectTowerColor;
        [SerializeField] private ParticleSystem deathParticleSystem;
        [SerializeField] private ParticleSystem deathTowerParticleSystem;

        public int id;
        public delegate void MapEnd(GameObject enemy);
        public event MapEnd OnFinishedMap; //queue'ya geri eklemen lazım
        
        public delegate void EnemyDeath(GameObject enemy);
        public static event Action OnEnemyDeathCount; //enemy text sayısı artmalı
        
        public static  event  Action<Enemy> OnEnemyDisabled;
        
        public Color EnemyColor
        {
            get => _color;
        }

        private void OnEnable()
        {
            CloseRagdoll(_normalChar,_ragdollChar);
        }


        private void OnDisable()
        {   
            OnEnemyDisabled?.Invoke(this);
            transform.DOKill();
        }

        private void Awake()
        {
            transform.localScale = _enemyType.enemyScale;
            _speed = _enemyType.speed;
            _damage = _enemyType.damage;
            _healthDamage = _enemyType.healthDamage;
            _health = _enemyType.health;
            _coinValue = _enemyType.coinValue;
            _scoreValue = _enemyType.enemyScoreValue;
            _firstScaleValue = gameObject.transform.localScale;
        }

        private void Start()
        {
            id = Random.Range(0, 10000);
        }

        private void FixedUpdate()
        {
            if (!isMoving) return;

            if (transform.position.z < _enemyEndPosition)
                Finished();
        }

        public void StartMoving()
        {
            gameObject.GetComponent<Collider>().enabled = true;
            gameObject.transform.localScale = _firstScaleValue;
            isMoving = true;
            _speed = _enemyType.speed;
            transform.DOMoveZ(rotationDestination.transform.position.z, _speed).SetEase(Ease.Linear).OnStart(()=>Debug.LogError("DoTween started")).OnComplete(() =>
            {
                transform.DOMove(GetComponent<EnemyTargetChooser>().TowerTransform.position, _speed / 2f)
                    .SetEase(Ease.Linear).SetSpeedBased();
                transform.DORotate(GetComponent<EnemyTargetChooser>().TowerTransform.eulerAngles, .1f)
                    .SetEase(Ease.Linear);
                ResetValues();
            }).SetSpeedBased();
        }

        public void LevelSuccessfullyFinished()
        {
            OpenRagdoll(_normalChar,_ragdollChar);
        }
        public void LevelUnsuccessfullyFinished()
        {
            Debug.LogError("Level unsuccessfully finished");
            OpenRagdoll(_normalChar,_ragdollChar);
            //TODO: levelun başarısız olduğu zaman enemy'lerin dans animasyonu
        }
        public void CompareColor(Color ballColor)
        {
            if (!_isDead)
            {
                if (ballColor == _renderer.material.color)
                {
                    StartCoroutine(WaitUntilRagdollMoves());
                }
                else
                {
                    print("Color is different");
                }
            }
        }

        private IEnumerator WaitUntilRagdollMoves()
        {
            ParticlePlay();
            print("health: "+_health);
            print("healthDamage: "+_healthDamage);
            _health -= _healthDamage;
            if (_health == 0)
            {
                print("Enemy is dead");
                _isDead = true;
                OnEnemyDeathCount?.Invoke();
                OpenRagdoll(_normalChar, _ragdollChar);
            }
            yield return new WaitForSeconds(3);
            Finished();
        }

        private void SetCoinValue()
        {
            RoosterHub.Central.SetIncome((int)_coinValue, true);
        }

        private void OpenRagdoll(Transform _normalCharachter , Transform _ragdollCharachter)
        {
            Transform[] allChildren = _normalCharachter.transform.GetComponentsInChildren<Transform>();
            Transform[] allChildrenRagdoll = _ragdollCharachter.transform.GetComponentsInChildren<Transform>();
            for (int j = 0; j < allChildren.Length; j++)
            {
                for (int z = 0; z < allChildrenRagdoll.Length; z++)
                {
                    if (allChildren[j].name == allChildrenRagdoll[z].name)
                    {
                        allChildrenRagdoll[z].position = allChildren[j].position;
                        allChildrenRagdoll[z].rotation = allChildren[j].rotation;
                       //allChildrenRagdoll[z].localScale = allChildren[j].localScale;
                        break;
                    }
                }
            }
            _normalCharachter.gameObject.SetActive(false);
            _ragdollCharachter.gameObject.SetActive(true);

            transform.DOKill();
            
            _ragdollCharachter.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<Collider>().enabled = false;
        }

        private void CloseRagdoll(Transform normalChar, Transform ragdollChar)
        {
            ragdollChar.gameObject.SetActive(false);
            normalChar.gameObject.SetActive(true);
        }
        private void ParticlePlay()
        {
            deathEffectColor.material.color = _renderer.material.color;
            deathParticleSystem.Play();
            SetCoinValue();
            RoosterHub.Haptic.SoftImpact();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Tower"))
            {
                Debug.LogError("Enemy is dead by tower");   
                ParticleTowerPlay();
                RoosterHub.Haptic.HardImpact();
                transform.DOScale(0, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    Invoke(nameof(Finished), deathTowerParticleSystem.main.duration);
                });
            }
        }
        private void ParticleTowerPlay()
        {
            deathEffectTowerColor.material.color = _renderer.material.color;
            deathTowerParticleSystem.Play();
        }
        private void Finished()
        {
            ResetValues();
            OnFinishedMap?.Invoke(gameObject);
            OnEnemyDisabled?.Invoke(this);
            gameObject.SetActive(false);
            isActive = false;
        }

        private void ResetValues()
        {
            CloseRagdoll(_normalChar,_ragdollChar);
            _speed = 0f;
            isMoving = false;
            _isDead = false;
            _health = _enemyType.health;
        }
        public void SetMyColor(Color myColor)
        {
            myColor.a = 1;
            _renderer.material.color = myColor;
            _ragdoll.material.color = myColor;
            _color = myColor;
        }
    }
}
