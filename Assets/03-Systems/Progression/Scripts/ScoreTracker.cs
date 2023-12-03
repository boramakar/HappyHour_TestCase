using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HappyTroll;
using TMPro;
using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] private GameObject levelDisplay;
    [SerializeField] private GameObject scoreDisplay;
    [SerializeField] private TextMeshProUGUI scoreText;

    private GameParameters _gameParameters;
    private int _score;
    private bool _isScoreTracked;

    private void Awake()
    {
        _gameParameters = GameManager.Instance.gameParameters;
        var progressionMode = ProgressionManager.Instance.GetCurrentProgressionMode();
        switch (progressionMode)
        {
            case Enums.ProgressionType.Level:
                levelDisplay.SetActive(true);
                break;
            case Enums.ProgressionType.HighScore:
                _isScoreTracked = true;
                _score = 0;
                UpdateScoreText();
                scoreDisplay.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(progressionMode), progressionMode, null);
        }
    }

    private void OnEnable()
    {
        if (_isScoreTracked)
            EventManager.OnWordFound += IncreaseScore;
        EventManager.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        if (_isScoreTracked)
            EventManager.OnWordFound -= IncreaseScore;
        EventManager.OnGameOver -= HandleGameOver;
    }

    private void IncreaseScore(string word)
    {
        _score += _gameParameters.scorePerWord;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = _score.ToString();
    }

    private void HandleGameOver()
    {
        ProgressionManager.Instance.CheckHighScore(_score);
    }
}