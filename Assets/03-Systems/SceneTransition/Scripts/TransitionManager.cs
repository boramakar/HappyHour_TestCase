using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HappyTroll
{
    public class TransitionManager : PersistentSingleton<TransitionManager>
    {
        private ITransitionHandler _transitionHandler;
        private bool canHandleTransitions;
        private string _nextScene;
        private string _postLoadingScene;
        private AsyncOperation _loadingOp;
        private Action<float> _progressCallback;
        private bool _allowFadeIn;

        protected override void Awake()
        {
            base.Awake();

            _transitionHandler = GetComponentInChildren<ITransitionHandler>();
            canHandleTransitions = (_transitionHandler != null);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
            EventManager.OnSceneTransitionComplete += HandleFadeoutComplete;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            EventManager.OnSceneTransitionComplete -= HandleFadeoutComplete;
        }

        private void HandleFadeoutComplete()
        {
            _allowFadeIn = true;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            Time.timeScale = 1;
            
            if (loadSceneMode == LoadSceneMode.Additive)
                return;

            if (!canHandleTransitions) return;
            
            _allowFadeIn = false;
            _transitionHandler.FadeOut(EventManager.SceneTransitionCompleteEvent);
        }

        public void ChangeScene(Enums.SceneType sceneType, bool useLoadingScene)
        {
            var sceneName = sceneType switch
            {
                Enums.SceneType.SplashScene => Strings.SplashScene,
                Enums.SceneType.LoadingScene => Strings.LoadingScene,
                Enums.SceneType.MenuScene => Strings.MenuScene,
                Enums.SceneType.GameplayScene => Strings.GameplayScene,
                _ => throw new ArgumentOutOfRangeException(nameof(sceneType), sceneType, null)
            };
            
            if (!useLoadingScene)
            {
                _nextScene = sceneName;
            }
            else
            {
                _postLoadingScene = sceneName;
                _nextScene = Strings.LoadingScene;
            }

            LoadScene(canHandleTransitions);
        }

        private void LoadScene(bool useTransition)
        {
            _loadingOp = SceneManager.LoadSceneAsync(_nextScene);
            _loadingOp.allowSceneActivation = false;
            StartCoroutine(WaitForLoading());
        }

        private IEnumerator WaitForLoading()
        {
            while (_loadingOp.progress < 0.9f)
            {
                _progressCallback?.Invoke(_loadingOp.progress);
                yield return null;
            }
    
            _progressCallback?.Invoke(1f);
            
            // Wait for fadeOut animation to complete to avoid bad transitions
            while (!_allowFadeIn)
                yield return null;
            
            _transitionHandler.FadeIn(AllowSceneChange);
        }

        private void AllowSceneChange()
        {
            _loadingOp.allowSceneActivation = true;
            _progressCallback = null;
        }

        public void StartLoading(Action<float> progressCallback)
        {
            _nextScene = _postLoadingScene;
            _postLoadingScene = "";
            _progressCallback = progressCallback;
            LoadScene(canHandleTransitions);
        }
    }
}