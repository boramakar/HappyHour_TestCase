using System;
using System.Collections;
using System.Collections.Generic;
using HappyTroll;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridHandler : MonoBehaviour
{
    [SerializeField] private LineRenderer borderLineRenderer;
    [SerializeField] private Transform rowsParent;
    [SerializeField] private Transform columnsParent;
    [SerializeField] private Transform gridContentParent;
    [SerializeField] private GameObject lineRendererPrefab;
    [SerializeField] private GameObject gridContentPrefab;

    private GameParameters _gameParameters;
    private int _gridRowCount;
    private int _gridColumnCount;
    private float _gridCellSize;
    private AnimationCurve _lineRendererWidthCurve;
    private TextMeshPro[,] _gridContents;
    private char[,] _virtualGrid;
    private WordList _currentWordList;
    private List<string> _remainingWords;

    private void Awake()
    {
        _gameParameters = GameManager.Instance.gameParameters;
        _lineRendererWidthCurve = AnimationCurve.Constant(0, 1, _gameParameters.gridLineWidth);
        _gridRowCount = _gameParameters.rowCount;
        _gridColumnCount = _gameParameters.columnCount;
    }

    private void Start()
    {
        // This is just a temporary solution to have a single level implementation.
        // Extending this to support multiple levels with dynamic level selection would be simple
        // but it's not important for the scope of this test case
        _currentWordList = JsonUtility.FromJson<WordList>(_gameParameters.wordListJson.text);
        // Capitalize all letters to prevent issues when comparing chars
        for (var i = 0; i < _currentWordList.words.Count; i++)
        {
            _currentWordList.words[i] = _currentWordList.words[i].ToUpper();
        }
        _remainingWords = new List<string>(_currentWordList.words);
        // Do the capitalization for bonus section of the list as well for a real implementation
        // Bonus is not used in this test case so it's not needed right now
        
        ClearGrid();
        GenerateGrid();
        FillGrid();
    }

    private void OnEnable()
    {
        EventManager.OnGridResize += ResizeGrid;
        EventManager.OnGridRefill += RefillGrid;
        EventManager.OnWordSelectionSuccess += HandleWordSelectionSuccess;
        EventManager.OnDisplayResults += HandleFoundWords;
    }

    public void OnDisable()
    {
        EventManager.OnGridResize -= ResizeGrid;
        EventManager.OnGridRefill -= RefillGrid;
        EventManager.OnWordSelectionSuccess -= HandleWordSelectionSuccess;
        EventManager.OnDisplayResults += HandleFoundWords;
    }

    private void HandleFoundWords(List<Tuple<string, int2, int2>> wordDetails)
    {
        foreach (var wordDetail in wordDetails)
        {
            _remainingWords.Remove(wordDetail.Item1);
        }
    }

    private void GenerateGrid()
    {
        _gridCellSize = _gameParameters.gridCellSize;
        // Each method can be run in parallel to make this process faster if needed.
        // Grid generation should not be done often and the current setup should be fast enough for when it's done
        GenerateBorder();
        GenerateRows();
        GenerateColumns();
        GenerateGridContent();

        GameManager.Instance.mainCamera.GetComponent<MainCameraHandler>()
            .AdjustToGrid(_gridRowCount, _gridColumnCount, _gridCellSize);
    }

    private void GenerateBorder()
    {
        var initialYOffset = -_gridRowCount * _gridCellSize * .5f;
        var initialXOffset = -_gridColumnCount * _gridCellSize * .5f;
        var borderPositions = new Vector3[]
        {
            new Vector3(initialXOffset, initialYOffset, 0),
            new Vector3(initialXOffset, -initialYOffset, 0),
            new Vector3(-initialXOffset, -initialYOffset, 0),
            new Vector3(-initialXOffset, initialYOffset, 0)
        };

        borderLineRenderer.widthCurve = _lineRendererWidthCurve;
        borderLineRenderer.positionCount = 4;
        borderLineRenderer.SetPositions(borderPositions);
        borderLineRenderer.loop = true;
    }

    // This function can be combined with GenerateColumns using some parameters for a slightly better implementation
    private void GenerateRows()
    {
        var initialYOffset = -_gridRowCount * _gridCellSize * .5f;
        var xOffset = -_gridColumnCount * _gridCellSize * .5f;

        for (var i = 1; i < _gridRowCount; i++)
        {
            var yOffset = initialYOffset + (i * _gridCellSize);
            var lineRenderer = Instantiate(lineRendererPrefab, rowsParent).GetComponent<LineRenderer>();
            lineRenderer.widthCurve = _lineRendererWidthCurve;
            var startPosition = new Vector3(xOffset, yOffset, 0);
            var endPosition = new Vector3(-xOffset, yOffset, 0);
            lineRenderer.SetPositions(new Vector3[] {startPosition, endPosition});
        }
    }

    private void GenerateColumns()
    {
        var initialXOffset = -_gridColumnCount * _gridCellSize * .5f;
        var yOffset = -_gridRowCount * _gridCellSize * .5f;

        for (var i = 1; i < _gridColumnCount; i++)
        {
            var xOffset = initialXOffset + (i * _gridCellSize);
            var lineRenderer = Instantiate(lineRendererPrefab, columnsParent).GetComponent<LineRenderer>();
            lineRenderer.widthCurve = _lineRendererWidthCurve;
            var startPosition = new Vector3(xOffset, yOffset, 0);
            var endPosition = new Vector3(xOffset, -yOffset, 0);
            lineRenderer.SetPositions(new Vector3[] {startPosition, endPosition});
        }
    }

    private void GenerateGridContent()
    {
        _virtualGrid = new char[_gridColumnCount, _gridRowCount];
        _gridContents = new TextMeshPro[_gridColumnCount, _gridRowCount];

        for (var i = 0; i < _gridContents.Length; i++)
        {
            var row = i / _gridColumnCount;
            var column = i % _gridColumnCount;
            var position = CellToWorldPosition(new int2(column, row));
            _gridContents[column, row] =
                Instantiate(gridContentPrefab, position, Quaternion.identity, gridContentParent)
                    .GetComponent<TextMeshPro>();
        }
    }

    private void FillGrid()
    {
        var usedCells = new bool[_gridColumnCount, _gridRowCount];
        // FILL WORDS FROM LIST
        //
        // This operation cannot be done in parallel due to race condition cause when filling the grid.
        // There might be better way to implement this whole part including PlaceWord method
        //
        // Additionally we don't really need a search algorithm when using this approach to fill the grid as we know
        // where each word starts and ends.
        // Marking words as found when the player finds them would be enough to "find" all unmarked words at O(1) time each
        // for a total complexity of O(n) where n is the number of unmarked words.
        // This would also allow us to not allow for the player to "find" any unintended words.
        // I will not be implementing the system mentioned above because I believe the explanation made here is enough
        // considering the scope of this test case.
        for (var i = 0; i < _currentWordList.words.Count; i++)
        {
            // We can use an Enum for direction to make this and every other direction based code much more readable
            // Currently I just have comments about directions instead
            var direction = Random.Range(0, 4);
            var placementSuccessful = direction switch
            {
                0 => // RIGHT
                    PlaceWord(i,
                        Random.Range(0, _gridRowCount),
                        Random.Range(0, _gridColumnCount - _currentWordList.words[i].Length),
                        0,
                        1,
                        ref usedCells
                        ),
                1 => // LEFT
                    PlaceWord(i,
                        Random.Range(0, _gridRowCount),
                        Random.Range(_currentWordList.words[i].Length, _gridColumnCount),
                        0,
                        -1,
                        ref usedCells
                    ),
                2 => // UP
                    PlaceWord(i,
                        Random.Range(0, _gridRowCount - _currentWordList.words[i].Length),
                        Random.Range(0, _gridColumnCount),
                        1,
                        0,
                        ref usedCells
                    ),
                3 => // DOWN
                    PlaceWord(i,
                        Random.Range(_currentWordList.words[i].Length, _gridRowCount),
                        Random.Range(0, _gridColumnCount),
                        -1,
                        0,
                        ref usedCells
                    ),
                _ => false
            };

            if (!placementSuccessful)
                i--;
        }

        // FILL REST WITH RANDOM LETTERS
        for (var i = 0; i < usedCells.Length; i++)
        {
            var row = i / _gridColumnCount;
            var column = i % _gridColumnCount;
            if (usedCells[column, row]) continue;

            var randomLetter = GetRandomLetter(_gameParameters.useFrequency);
            _gridContents[column, row].text = randomLetter.ToString();
            _virtualGrid[column, row] = randomLetter;
        }
    }

    private bool PlaceWord(int wordIndex, int row, int column, int rowOffset, int columnOffset, ref bool[,] usedCells)
    {
        var word = _currentWordList.words[wordIndex];
        var changeStack = new List<int2>(word.Length);
        var isPlaced = false;
        for (var i = 0; i < word.Length; i++)
        {
            var currentRow = row + (i * rowOffset);
            var currentColumn = column + (i * columnOffset);
            var currentLetter = word[i];

            // Record changes to usedCells in case we fail later on
            if (!usedCells[currentColumn, currentRow])
            {
                changeStack.Add(new int2(currentColumn, currentRow));
                usedCells[currentColumn, currentRow] = true;
                isPlaced = true;
            }
            else if (_virtualGrid[currentColumn, currentRow] == currentLetter)
                isPlaced = true;

            if (isPlaced)
            {
                _gridContents[currentColumn, currentRow].text = currentLetter.ToString();
                _virtualGrid[currentColumn, currentRow] = currentLetter;
                isPlaced = false;
            }
            else
            {
                // Revert changes to usedCells
                foreach (var change in changeStack)
                {
                    usedCells[change.x, change.y] = false;
                }

                return false;
            }
        }

        return true;
    }

    private char GetRandomLetter(bool useFrequency)
    {
        return useFrequency switch
        {
            true => HelperFunctions
                .GetRandomLetter(), // GetRandomLetterByFrequency() should be used here when frequency table logic is implemented
            false => HelperFunctions.GetRandomLetter()
        };
    }

    private void ResizeGrid(int rowCount, int columnCount)
    {
        _gridRowCount = rowCount;
        _gridColumnCount = columnCount;

        ClearGrid();
        GenerateGrid();
    }

    private void ClearGrid()
    {
        for (var i = rowsParent.childCount - 1; i >= 0; i--)
        {
            Destroy(rowsParent.GetChild(i).gameObject);
        }

        for (var i = columnsParent.childCount - 1; i >= 0; i--)
        {
            Destroy(columnsParent.GetChild(i).gameObject);
        }

        for (var i = gridContentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(gridContentParent.GetChild(i).gameObject);
        }
    }

    private void RefillGrid()
    {
        _remainingWords = new List<string>(_currentWordList.words);
        FillGrid();
    }

    public void GetGridDetails(out int columnCount, out int rowCount, out char[] wordsArray,
        out int2[] wordIndicesArray, out char[] gridArray)
    {
        columnCount = _gridColumnCount;
        rowCount = _gridRowCount;
        wordsArray = string.Join("", _remainingWords).ToCharArray();
        wordIndicesArray = new int2[_remainingWords.Count];
        var index = 0;
        for (var i = 0; i < _remainingWords.Count; i++)
        {
            wordIndicesArray[i] = new int2(index, _remainingWords[i].Length);
            index += _remainingWords[i].Length;
        }

        gridArray = new char[_gridColumnCount * _gridRowCount];

        for (var i = 0; i < rowCount; i++)
        {
            for (var j = 0; j < columnCount; j++)
            {
                gridArray[(i * columnCount) + j] = _virtualGrid[j, i];
            }
        }
    }

    public WordList GetCurrentWordList()
    {
        return _currentWordList;
    }

    public List<string> GetRemainingWorGdList()
    {
        return _remainingWords;
    }

    private void HandleWordSelectionSuccess(string word)
    {
        _remainingWords.Remove(word);
    }

    public Vector3 CellToWorldPosition(int2 cellPosition)
    {
        var initialRowOffset = -(_gridRowCount - 1) * _gridCellSize * .5f;
        var initialColumnOffset = -(_gridColumnCount - 1) * _gridCellSize * .5f;
        return new Vector3(
            initialColumnOffset + (cellPosition.x * _gridCellSize),
            initialRowOffset + (cellPosition.y * _gridCellSize)
        );
    }
}