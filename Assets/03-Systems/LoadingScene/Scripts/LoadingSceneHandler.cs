using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HappyTroll
{
    public class LoadingSceneHandler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI percentageText;
        [SerializeField] private TextMeshProUGUI loadingText;
        [SerializeField] private Slider loadingBar;
        [SerializeField] private float animationDuration;
        [SerializeField] private string[] loadingTexts;
        
        private int _animationIndex;

        private void Awake()
        {
            percentageText.text = "0%";
            _animationIndex = 0;
            loadingText.text = loadingTexts[_animationIndex];
            loadingBar.value = 0f;
        }

        private void Start()
        {
            TransitionManager.Instance.StartLoading(UpdateProgress);
            StartCoroutine(LoadingTextAnimation());
        }

        private IEnumerator LoadingTextAnimation()
        {
            while (true)
            {
                var elapsedTime = 0f;
                while (elapsedTime < animationDuration)
                {
                    yield return null;
                    elapsedTime += Time.deltaTime;
                }

                _animationIndex = (_animationIndex + 1) % loadingTexts.Length;
                loadingText.text = loadingTexts[_animationIndex];
            }
        }

        private void UpdateProgress(float loadingOpProgress)
        {
            loadingBar.value = loadingOpProgress;
            percentageText.text = $"{loadingOpProgress * 100}%";
        }
    }
}