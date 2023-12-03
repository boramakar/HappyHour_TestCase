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
        }

        public void RegisterCamera(Camera cam)
        {
            mainCamera = cam;
        }
    }
}