using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/SO/Game Parameters", fileName = "GameParameters")]
public class GameParameters : ScriptableObject
{
    // Performance
    [FoldoutGroup("Performance", true)] public int wordsPerJob;
    
    // Transition
    [FoldoutGroup("Transition", true)] public float fadeDuration;
    
    // Scoring
    [FoldoutGroup("Scoring", true)] public int scorePerWord; // Functionality not implemented due to scope of this test case
    
    // Progression
    public TextAsset wordListJson;
    [FoldoutGroup("Progression", true)] public List<GameLevel> levels; // Functionality not implemented due to scope of this test case
    
    // Grid
    [FoldoutGroup("Grid", true)] public float gridCellSize;
    [FoldoutGroup("Grid", true)] public float gridLineWidth;
    [FoldoutGroup("Grid", true)] public Material gridMaterial; // Functionality not implemented due to scope of this test case
    [FoldoutGroup("Grid", true)] public bool useFrequency;
    [FoldoutGroup("Grid", true)] public int rowCount;
    [FoldoutGroup("Grid", true)] public int columnCount;
    
    // Camera
    [FoldoutGroup("Camera", true)] public float cameraZOffset;
    [FoldoutGroup("Camera", true)] public float cameraPadding;
}
