using System;
using System.Collections;
using System.Collections.Generic;
using HappyTroll;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class WordFindPopupHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI wordDisplayTextMesh;
    [SerializeField] private GameObject wordDisplayPopup;

    private void OnEnable()
    {
        EventManager.OnDisplayResults += DisplayFoundWords;
        EventManager.OnWordSelectionSuccess += DisplaySelectedWord;
    }

    private void OnDisable()
    {
        EventManager.OnDisplayResults -= DisplayFoundWords;
        EventManager.OnWordSelectionSuccess -= DisplaySelectedWord;
    }

    private void DisplayFoundWords(List<Tuple<string, int2, int2>> foundWords)
    {
        if (foundWords.Count == 0)
        {
            wordDisplayTextMesh.text = Strings.NoFoundWordsText;
        }
        else
        {
            wordDisplayTextMesh.text = foundWords[0].Item1;

            for (var i = 1; i < foundWords.Count; i++)
            {
                wordDisplayTextMesh.text += $"\n{foundWords[i].Item1}";
            }
        }
        
        wordDisplayPopup.SetActive(true);
    }

    private void DisplaySelectedWord(string word)
    {
        wordDisplayTextMesh.text = word;
        wordDisplayPopup.SetActive(true);
    }
}