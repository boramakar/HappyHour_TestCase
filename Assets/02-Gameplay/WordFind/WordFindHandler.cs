using System.Collections;
using System.Collections.Generic;
using HappyTroll;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class WordFindHandler : MonoBehaviour
{
    private GameParameters _gameParameters;

    private void Awake()
    {
        _gameParameters = GameManager.Instance.gameParameters;
    }

    public void SearchWords()
    {
        var searchJobCount = _gameParameters.searchJobCount;
        NativeArray<JobHandle> jobHandles =
            new NativeArray<JobHandle>(searchJobCount, Allocator.Temp);
        var wordsPerJob = GameManager.Instance.currentWordList.Count / searchJobCount;
        for (int i = 0; i < searchJobCount; i++)
        {
            for(int j = 0; j < wordsPerJob; j++)
            {
                jobHandles[i] = SearchWords(i * wordsPerJob, wordsPerJob);
            }
        }
    }

    private JobHandle SearchWords(int startingIndex, int wordCount)
    {
        throw new System.NotImplementedException();
    }
}
