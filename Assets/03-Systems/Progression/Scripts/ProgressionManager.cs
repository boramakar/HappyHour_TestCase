using System;
using System.Collections;
using System.Collections.Generic;
using HappyTroll;
using UnityEngine;

public class ProgressionManager : PersistentSingleton<ProgressionManager>
{
    private GameParameters _gameParameters;
    private int _currentLevel;
    private int _currentHighScore;
    private Enums.ProgressionType _currentProgressionMode;

    protected override void Awake()
    {
        base.Awake();

        _gameParameters = GameManager.Instance.gameParameters;
        Initialize();
        EventManager.OnGameSuccess += LevelCompleted;
        EventManager.OnUpdateProgressionMode += UpdateProgressionMode;
    }

    private void Initialize()
    {
        _currentLevel = PlayerPrefs.GetInt(Strings.CurrentLevelKey, 0);
        _currentHighScore = PlayerPrefs.GetInt(Strings.HighScoreKey, 0);
    }

    public void CheckHighScore(int score)
    {
        if (score <= _currentHighScore) return;
        
        EventManager.NewHighScoreEvent(_currentHighScore, score);
        _currentHighScore = score;
        SaveProgression();
    }

    private void LevelCompleted()
    {
        _currentLevel++;
        SaveProgression();
    }

    private void SaveProgression()
    {
        // Can be replaced with custom JSON or remote save systems
        // Can implement and use a SaveManager 
        PlayerPrefs.SetInt(Strings.CurrentLevelKey, _currentLevel);
        PlayerPrefs.SetInt(Strings.HighScoreKey, _currentHighScore);
        PlayerPrefs.Save();
    }

    public string GetProgressionValue(Enums.ProgressionType progressionType)
    {
        var key = progressionType switch
        {
            Enums.ProgressionType.Level => Strings.CurrentLevelKey,
            Enums.ProgressionType.HighScore => Strings.HighScoreKey,
            _ => throw new ArgumentOutOfRangeException(nameof(progressionType), progressionType, null)
        };
        return PlayerPrefs.GetInt(key).ToString();
    }

    public GameLevel GetCurrentLevelData()
    {
        return _gameParameters.levels[_currentLevel % _gameParameters.levels.Count];
    }

    public Enums.ProgressionType GetCurrentProgressionMode()
    {
        return _currentProgressionMode;
    }

    private void UpdateProgressionMode(Enums.ProgressionType progressionType)
    {
        _currentProgressionMode = progressionType;
    }

    public int GetCurrentLevel()
    {
        return _currentLevel;
    }
}
