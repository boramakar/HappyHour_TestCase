using System;
using System.Collections;
using System.Collections.Generic;
using HappyTroll;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class WordSelectionHandler : MonoBehaviour
{
    [SerializeField] private LayerMask letterLayerMask;
    [SerializeField] private LineRenderer selectionLinePrefab;
    [SerializeField] private GridHandler gridHandler;
    [SerializeField] private Transform selectionLinesParent;
    
    private GameParameters _gameParameters;
    private Camera _raycastCam;
    private List<TextMeshPro> _selectedLetters;
    private Coroutine _coroutine;
    private LineRenderer _currentSelectionLine;

    private void Awake()
    {
        _gameParameters = GameManager.Instance.gameParameters;
        _raycastCam = GameManager.Instance.mainCamera;
        _selectedLetters = new List<TextMeshPro>();
    }

    private void OnEnable()
    {
        EventManager.OnInputPress += StartWordSelection;
        EventManager.OnInputRelease += StopWordSelection;
        EventManager.OnDisplayResults += MarkFoundWord;
        EventManager.OnGridRefill += RemoveSelectionLines;
    }
    
    private void OnDisable()
    {
        EventManager.OnInputPress -= StartWordSelection;
        EventManager.OnInputRelease -= StopWordSelection;
        EventManager.OnDisplayResults -= MarkFoundWord;
        EventManager.OnGridRefill -= RemoveSelectionLines;
    }

    private void StartWordSelection(Vector2 _)
    {
        _currentSelectionLine = Instantiate(selectionLinePrefab, Vector3.zero, Quaternion.identity, selectionLinesParent);
        _coroutine = StartCoroutine(WordSelectionCoroutine());
    }

    private void StopWordSelection(Vector2 _)
    {
        var word = "";
        foreach (var letterTextMesh in _selectedLetters)
        {
            word += letterTextMesh.text;
        }
        StopCoroutine(_coroutine);
        _selectedLetters.Clear();

        if (gridHandler.GetRemainingWorGdList().Contains(word))
        {
            EventManager.WordSelectionSuccessEvent(word);
        }
        else
        {
            Destroy(_currentSelectionLine.gameObject);
        }
    }

    private IEnumerator WordSelectionCoroutine()
    {
        Vector3 selectionDirection = default;
        
        while (true)
        {
            var ray = _raycastCam.ScreenPointToRay(InputManager.Instance.GetPointerPosition());
            if(Physics.Raycast(ray, out var hit, _gameParameters.raycastDistance, letterLayerMask))
            {
                var letterTextMesh = hit.transform.GetComponent<TextMeshPro>();
                var isLetterValid = false;
                
                if (!_selectedLetters.Contains(letterTextMesh))
                {
                    switch (_selectedLetters.Count)
                    {
                        case 0:
                            isLetterValid = true;
                            break;
                        case 1:
                            isLetterValid = true;
                            selectionDirection = letterTextMesh.transform.position - _selectedLetters[0].transform.position;
                            selectionDirection.Normalize();
                            break;
                        default:
                        {
                            var direction = letterTextMesh.transform.position - _selectedLetters[^1].transform.position;
                            if (selectionDirection.Equals(direction.normalized))
                            {
                                isLetterValid = true;
                            }

                            break;
                        }
                    }
                }

                if (isLetterValid)
                {
                    _selectedLetters.Add(letterTextMesh);
                    var positionCount = _currentSelectionLine.positionCount;
                    _currentSelectionLine.positionCount = positionCount + 1;
                    _currentSelectionLine.SetPosition(positionCount, letterTextMesh.transform.position);
                }
            }

            yield return null;
        }
    }

    private void MarkFoundWord(List<Tuple<string, int2, int2>> wordDetails)
    {
        foreach (var wordDetail in wordDetails)
        {
            var lineRenderer = Instantiate(selectionLinePrefab, Vector3.zero, Quaternion.identity, selectionLinesParent);
            var startingPosition = wordDetail.Item2;
            var endingPosition = wordDetail.Item3;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions( new []
            {
                gridHandler.CellToWorldPosition(startingPosition),
                gridHandler.CellToWorldPosition(endingPosition)
            });
        }
    }

    private void RemoveSelectionLines()
    {
        for (var i = selectionLinesParent.childCount - 1; i >= 0; i--)
        {
            Destroy(selectionLinesParent.GetChild(i).gameObject);
        }
    }
}
