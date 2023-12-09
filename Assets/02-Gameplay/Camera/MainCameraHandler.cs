using System;
using System.Collections;
using System.Collections.Generic;
using HappyTroll;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCameraHandler : MonoBehaviour
{
    private GameParameters _gameParameters;
    
    private void Awake()
    {
        GameManager.Instance.mainCamera = GetComponent<Camera>();
        _gameParameters = GameManager.Instance.gameParameters;
    }

    // This method assumes all grids will be square
    // More complex camera adjustment is beyond the scope of this test case
    public void AdjustToGrid(int rowCount, int columnCount, float cellSize)
    {
        transform.position = new Vector3(0, -rowCount, _gameParameters.cameraZOffset);
        GameManager.Instance.mainCamera.orthographicSize = columnCount * (cellSize + _gameParameters.cameraPadding);
    }
}
