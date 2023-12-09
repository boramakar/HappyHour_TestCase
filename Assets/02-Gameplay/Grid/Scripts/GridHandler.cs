using System;
using System.Collections;
using System.Collections.Generic;
using HappyTroll;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

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
    private bool _isGridFilled;
    private AnimationCurve _lineRendererWidthCurve;
    private TextMeshPro[,] _gridContents;
    private char[,] _virtualGrid;

    private void Awake()
    {
        _gameParameters = GameManager.Instance.gameParameters;
        _lineRendererWidthCurve = AnimationCurve.Constant(0, 1, _gameParameters.gridLineWidth);
        _gridRowCount = _gameParameters.rowCount;
        _gridColumnCount = _gameParameters.columnCount;
        _isGridFilled = false;
    }

    private void Start()
    {
        ClearGrid();
        GenerateGrid();
        FillGrid();
        
        GameManager.Instance.currentWordList = JsonUtility.FromJson<WordList>(_gameParameters.wordListJson.text);
    }

    private void OnEnable()
    {
        EventManager.OnGridResize += ResizeGrid;
        EventManager.OnGridRefill += RefillGrid;
    }

    public void OnDisable()
    {
        EventManager.OnGridResize -= ResizeGrid;
        EventManager.OnGridRefill -= RefillGrid;
    }

    private void GenerateGrid()
    {
        _gridCellSize = _gameParameters.gridCellSize;
        // Each method can be run in parallel to make this process faster if needed.
        // Grid generation should not be done often and the current setup should be fast enough for when it's done
        GenerateBorder();
        GenerateRows();
        GenerateColumns();
        GameManager.Instance.mainCamera.GetComponent<MainCameraHandler>().AdjustToGrid(_gridRowCount, _gridColumnCount, _gridCellSize);
        
        _gridContents = new TextMeshPro[_gridColumnCount, _gridRowCount];
        _virtualGrid = new char[_gridColumnCount, _gridRowCount];
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
            lineRenderer.SetPositions(new Vector3[]{startPosition, endPosition});
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
            lineRenderer.SetPositions(new Vector3[]{startPosition, endPosition});
        }
    }

    private void FillGrid()
    {
        // Could use if inside the loop to reduce this to a single block but it will increase the overall cost
        // Could separate this into two different methods but I'm lazy and the responsibilities would be the same
        if(!_isGridFilled)
        {
            var initialRowOffset = -(_gridRowCount - 1) * _gridCellSize * .5f;
            var initialColumnOffset = -(_gridColumnCount - 1) * _gridCellSize * .5f;
            for (var i = 0; i < _gridRowCount; i++)
            {
                for (var j = 0; j < _gridColumnCount; j++)
                {
                    var position = new Vector3(
                        initialColumnOffset + (j * _gridCellSize), 
                        initialRowOffset + (i * _gridCellSize)
                        );
                    var textMesh = Instantiate(gridContentPrefab, position, Quaternion.identity, gridContentParent).GetComponent<TextMeshPro>();
                    // Could refactor into a method but it will cause unnecessary overhead
                    var randomLetter = GetRandomLetter(_gameParameters.useFrequency);
                    textMesh.text = randomLetter.ToString();
                    _gridContents[j,i] = textMesh;
                    _virtualGrid[j,i] = randomLetter;
                }
            }

            _isGridFilled = true;
        }
        else
        {
            for (var i = 0; i < _gridRowCount; i++)
            {
                for (var j = 0; j < _gridColumnCount; j++)
                {
                    // Could refactor into a method but it will cause unnecessary overhead
                    var randomLetter = GetRandomLetter(_gameParameters.useFrequency);
                    _gridContents[j,i].text = randomLetter.ToString();
                    _virtualGrid[j,i] = randomLetter;
                }
            }
        }
    }

    private char GetRandomLetter(bool useFrequency)
    {
        return useFrequency switch
        {
            true => HelperFunctions.GetRandomLetter(),// GetRandomLetterByFrequency() should be used here when frequency table logic is implemented
            false => HelperFunctions.GetRandomLetter()
        };
    }

    private void ResizeGrid(int rowCount, int columnCount)
    {
        _gridRowCount = rowCount;
        _gridColumnCount = columnCount;
        
        ClearGrid();
        GenerateGrid();
        FillGrid();
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

        _isGridFilled = false;
    }

    private void RefillGrid()
    {
        FillGrid();
    }

    public void GetGridDetails(out int columnCount, out int rowCount, out char[] wordsArray, out int2[] wordIndicesArray, out char[] gridArray)
    {
        columnCount = _gridColumnCount;
        rowCount = _gridRowCount;
        var wordList = GameManager.Instance.currentWordList.words;
        wordsArray = string.Join("", wordList).ToCharArray();
        wordIndicesArray = new int2[wordList.Count];
        var index = 0;
        for (var i = 0; i < wordList.Count; i++)
        {
            wordIndicesArray[i] = new int2(index, wordList[i].Length);
            index += wordList[i].Length;
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
}
