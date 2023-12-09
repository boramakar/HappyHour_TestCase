using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

namespace HappyTroll
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        public GameParameters gameParameters;

        public Camera mainCamera;
        [HideInInspector] public WordList currentWordList; // Should ideally be placed in a different manager

        protected override void Awake()
        {
            base.Awake();

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                Debug.unityLogger.logEnabled = false;
            
            EventManager.OnGameOver += GameOver;
            EventManager.OnGameFail += GameOver;
            EventManager.OnGameSuccess += GameOver;
        }

        private void GameOver()
        {
            
        }

        private void Start()
        {
            EventManager.GameManagerReadyEvent();
            
            // Placeholder for a splash video
            DOVirtual.DelayedCall(2, () => TransitionManager.Instance.ChangeScene(Enums.SceneType.GameplayScene, true));
        }

        public void RegisterCamera(Camera cam)
        {
            mainCamera = cam;
        }
    }
}