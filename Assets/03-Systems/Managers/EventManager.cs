#define _ENABLE_LOGS_

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace HappyTroll
{
    public static class EventManager
    {
        public static event Action OnGameManagerReady;
        public static void GameManagerReadyEvent()
        {
#if _ENABLE_LOGS_
            Debug.Log("Event - GameManagerReadyEvent");
#endif
            OnGameManagerReady?.Invoke();
        }

        public static event Action OnGameStart;
        public static void GameStartEvent()
        {
#if _ENABLE_LOGS_
            Debug.Log("Event - GameStartEvent");
#endif
            OnGameStart?.Invoke();
        }

        public static event Action<Enums.MenuPanel> OnMenuNavigation;
        public static void MenuNavigationEvent(Enums.MenuPanel panel)
        {
#if _ENABLE_LOGS_
            Debug.Log($"Event - MenuNavigationEvent: {panel}");
#endif
            OnMenuNavigation?.Invoke(panel);
        }

        public static event Action<Vector2> OnInputPress;
        public static void InputPressEvent(Vector2 position)
        {
#if _ENABLE_LOGS_
            Debug.Log($"Event - InputPressEvent: {position}");
#endif
            OnInputPress?.Invoke(position);
        }

        public static event Action<Vector2> OnInputRelease;
        public static void InputReleaseEvent(Vector2 position)
        {
#if _ENABLE_LOGS_
            Debug.Log($"Event - InputReleaseEvent: {position}");
#endif
            OnInputRelease?.Invoke(position);
        }

        public static event Action OnSceneTransitionComplete;
        public static void SceneTransitionCompleteEvent()
        {
#if _ENABLE_LOGS_
            Debug.Log($"Event - SceneTransitionCompleteEvent");
#endif
            OnSceneTransitionComplete?.Invoke();
        }

        public static event Action OnGameOver;
        public static void GameOverEvent()
        {
#if _ENABLE_LOGS_
            Debug.Log($"Event - GameOverEvent");
#endif
            OnGameOver?.Invoke();
        }

        public static event Action OnGameResume;
        public static void ResumeGameEvent()
        {
#if _ENABLE_LOGS_
            Debug.Log($"Event - ResumeGameEvent");
#endif
            OnGameResume?.Invoke();
        }

        public static event Action OnGamePause;
        public static void PauseGameEvent()
        {
#if _ENABLE_LOGS_
            Debug.Log($"Event - PauseGameEvent");
#endif
            OnGamePause?.Invoke();
        }

        public static event Action<int, int> OnNewHighScore;
        public static void NewHighScoreEvent(int previousScore, int newScore)
        {
#if _ENABLE_LOGS_
            Debug.Log($"Event - NewHighScoreEvent: {previousScore} - {newScore}");
#endif
            OnNewHighScore?.Invoke(previousScore, newScore);
        }

        public static event Action OnGameSuccess;
        public static void GameSuccessEvent()
        {
#if _ENABLE_LOGS_
            Debug.Log($"Event - GameSuccessEvent");
#endif
            OnGameSuccess?.Invoke();
        }

        public static event Action OnGameFail;
        public static void GameFailEvent()
        {
#if _ENABLE_LOGS_
            Debug.Log($"Event - GameFailEvent");
#endif
            OnGameFail?.Invoke();
        }

        public static event Action<Enums.ProgressionType> OnUpdateProgressionMode;
        public static void UpdateProgressionModeEvent(Enums.ProgressionType progressionType)
        {
#if _ENABLE_LOGS_
            Debug.Log($"Event - UpdateProgressionModeEvent: {progressionType}");
#endif
            OnUpdateProgressionMode?.Invoke(progressionType);
        }

        public static event Action<int, int, int> OnWordFound;
        public static void WordFoundEvent(int index, int columnNumber, int rowNumber)
        {
#if _ENABLE_LOGS_
            Debug.Log($"Event - WordFoundEvent: {index} - ({columnNumber},{rowNumber})");
#endif
            OnWordFound?.Invoke(index, columnNumber, rowNumber);
        }

        public static event Action<int, int> OnGridResize;
        public static void GridResizeEvent(int rows, int columns)
        {
#if _ENABLE_LOGS_
            Debug.Log($"Event - GridResizeEvent: ({rows}, {columns})");
#endif
            OnGridResize?.Invoke(rows, columns);
        }
        
        public static event Action OnGridRefill;
        public static void GridRefillEvent()
        {
#if _ENABLE_LOGS_
            Debug.Log($"Event - GridRefillEvent");
#endif
            OnGridRefill?.Invoke();
        }

        public static event Action OnGridSearch;
        public static void GridSearchEvent()
        {
#if _ENABLE_LOGS_
            Debug.Log($"Event - GridSearchEvent");
#endif
            OnGridSearch?.Invoke();
        }

        public static event Action<List<Tuple<string,int2,int2>>> OnDisplayResults;
        public static void DisplayResultsEvent(List<Tuple<string,int2,int2>> foundWords)
        {
            OnDisplayResults?.Invoke(foundWords);
        }

        public static event Action<string> OnWordSelectionSuccess;
        public static void WordSelectionSuccessEvent(string word)
        {
            OnWordSelectionSuccess?.Invoke(word);
        }
    }
}