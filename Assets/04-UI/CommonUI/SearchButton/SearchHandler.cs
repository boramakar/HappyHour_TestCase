using System;
using System.Collections;
using System.Collections.Generic;
using HappyTroll;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class SearchHandler : MonoBehaviour
{
    [SerializeField] private GridHandler gridHandler;

    private GameParameters _gameParameters;
    
    private int _columnCount;
    private int _rowCount;
    private char[] _wordsArray;
    private int2[] _wordIndicesArray;
    private char[] _gridArray;

    private void Awake()
    {
        _gameParameters = GameManager.Instance.gameParameters;
    }

    public void OnClick()
    {
        SearchWords();
    }

    private void SearchWords()
    {
        gridHandler.GetGridDetails(out _columnCount, out _rowCount, out _wordsArray, out _wordIndicesArray, out _gridArray);
        SearchWordsJob();
    }

    private void SearchWordsJob()
    {
        NativeArray<int2> wordIndices = new NativeArray<int2>(_wordIndicesArray, Allocator.TempJob); // Starting index, word length
        NativeArray<int3> results = new NativeArray<int3>(_wordIndicesArray.Length, Allocator.TempJob); // Direction, column, row
        NativeArray<char> words = new NativeArray<char>(_wordsArray, Allocator.TempJob);
        NativeArray<char> grid = new NativeArray<char>(_gridArray, Allocator.TempJob);

        for (int i = 0; i < _wordIndicesArray.Length; i++)
        {
            results[i] = new int3(-1, -1, -1);
        }

        WordFindParallelJob job = new WordFindParallelJob
        {
            columnCount = _columnCount,
            rowCount = _rowCount,
            wordIndices = wordIndices,
            words = words,
            grid = grid,
            results = results
        };

        // Schedule a parallel-for job. First parameter is how many for-each iterations to perform.
        // The second parameter is the batch size,
        // essentially the no-overhead inner-loop that just invokes Execute(i) in a loop.
        // When there is a lot of work in each iteration then a value of 1 can be sensible.
        // When there is very little work values of 32 or 64 can make sense.
        JobHandle jobHandle = job.Schedule(_wordIndicesArray.Length, _gameParameters.wordsPerJob);

        // Ensure the job has completed.
        // It is not recommended to Complete a job immediately,
        // since that reduces the chance of having other jobs run in parallel with this one.
        // You optimally want to schedule a job early in a frame and then wait for it later in the frame.
        jobHandle.Complete();

        var foundWords = new List<Tuple<string, int2, int2>>();
        var wordsList = gridHandler.GetCurrentWordList().words;
        for (var i = 0; i < _wordIndicesArray.Length; i++)
        {
            if (results[i].x == -1) continue;

            var word = wordsList[i];
            var xCoordinate = results[i].y;
            var yCoordinate = results[i].z;
            switch (results[i].x)
            {
                case -1:
                default:
                    break;
                case 0: // Right
                    foundWords.Add(new Tuple<string, int2, int2>(
                        word,
                        new int2(xCoordinate, yCoordinate),
                        new int2(xCoordinate + word.Length, yCoordinate)
                        ));
                    break;
                case 1: // Left
                    foundWords.Add(new Tuple<string, int2, int2>(
                        word,
                        new int2(xCoordinate, yCoordinate),
                        new int2(xCoordinate - word.Length, yCoordinate)
                    ));
                    break;
                case 2: // Up
                    foundWords.Add(new Tuple<string, int2, int2>(
                        word,
                        new int2(xCoordinate, yCoordinate),
                        new int2(xCoordinate, yCoordinate + word.Length)
                    ));
                    break;
                case 3: // Down
                    foundWords.Add(new Tuple<string, int2, int2>(
                        word,
                        new int2(xCoordinate, yCoordinate),
                        new int2(xCoordinate, yCoordinate - word.Length)
                    ));
                    break;
            }
        }
        
        // Display results
        // EventManager.DisplayResults(this.results);

        // Native arrays must be disposed manually.
        wordIndices.Dispose();
        words.Dispose();
        grid.Dispose();
        results.Dispose();
    }
}