using System;
using System.Data.Common;
//using Packages.Rider.Editor.UnitTesting;
using RG.Handlers;
using RG.Loader;
using RoosterHub;
using UnityEngine;

namespace RG.Core
{
    public class TheLoop : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
            LoadScene();
        }

        private void Start()
        {
            
        }

        private void OnEnable()
        {
            RoosterEventHandler.OnClickedNextLevelButton += LoadNewLevel;
            RoosterEventHandler.OnClickedRestartLevelButton += RestartLevel;
            RoosterEventHandler.OnCollectCoin += CollectCoin;
            RoosterEventHandler.OnWinGame += WinLevel;
            RoosterEventHandler.OnFailGame += FailLevel;
            RoosterEventHandler.OnLevelLoaded += TransitionControl;
        }

        void Test()
        {
          RoosterHub.Scoreboard.SetScore(500);
        }
        private void OnDisable()
        {
            RoosterEventHandler.OnClickedNextLevelButton -= LoadNewLevel;
            RoosterEventHandler.OnClickedRestartLevelButton -= RestartLevel;
            RoosterEventHandler.OnCollectCoin -= CollectCoin;
            RoosterEventHandler.OnWinGame -= WinLevel;
            RoosterEventHandler.OnFailGame -= FailLevel;
            RoosterEventHandler.OnLevelLoaded -= TransitionControl;
        }
        
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Central.Win();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                Central.Fail();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                Central.SetIncome(100, true);
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                Debug.LogError(Central.SpendIncome(30));
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                Test();
            }
            
            
           
            
            
        }

        void WinLevel()
        {
            RoosterAnalyticSender.SendWinEvent();

            IncreaseLevel();

            UI_Loop.OnChangeUI?.Invoke(true);  // End Game UI, WIN
            RoosterEventHandler.OnEndGameHandler?.Invoke(true); // Oyun başarıyla tamamlandı.
        }

        void FailLevel()
        {
            RoosterAnalyticSender.SendFailEvent();

            UI_Loop.OnChangeUI?.Invoke(true); // End Game UI , FAIL
            RoosterEventHandler.OnEndGameHandler?.Invoke(false); // Oyun başarısız
        }

        void CollectCoin(int coin, bool realtimeUpdate)
        {
            if (realtimeUpdate)
            {
                GamePrefs.CollectedCoin += coin;
                RoosterEventHandler.OnUpdateCoinText?.Invoke();
            }
            else
            {
                GamePrefs.CollectedCoin += coin;
            }
        }

        private void LoadScene()
        {
            var _loadType = GetComponent<ILoader>();
            if (_loadType == null)
            {
                _loadType = gameObject.AddComponent<LoadGameScene>();
                _loadType.Load();
            }
            else
            {
                _loadType.Load();
            }
        }

        private void IncreaseLevel()
        {
            GamePrefs.LevelNo++;
        }

        private void LoadNewLevel()
        {
            LoadScene();
        }

        private void RestartLevel()
        {
            RoosterAnalyticSender.SendRestartEvent();
            LoadScene();
        }

        private void TransitionControl()
        {
            RoosterLogger.Log("Level Loaded",Color.blue);
            if (GetComponent<UI_Loop>().useTransition)
            {
                RoosterEventHandler.OnShowTransition?.Invoke(false);
            }
            
            UI_Loop.OnChangeUI?.Invoke(true); // Transition ile birlikte UI değişimi
        }
    }
}