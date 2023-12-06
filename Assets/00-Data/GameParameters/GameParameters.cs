using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/SO/Game Parameters", fileName = "GameParameters")]
public class GameParameters : ScriptableObject
{
    // Performance
    [FoldoutGroup("Performance", true)] public int searchJobCount;
    
    // Transition
    [FoldoutGroup("Transition", true)] public float fadeDuration;
    
    // Scoring
    [FoldoutGroup("Scoring", true)] public int scorePerWord;
    
    // Progression
    [FoldoutGroup("Progression", true)] public List<GameLevel> levels;
    
    // Grid
    [FoldoutGroup("Grid", true)] public float gridCellSize;
    [FoldoutGroup("Grid", true)] public float gridLineWidth;
    [FoldoutGroup("Grid", true)] public Material gridMaterial;
    [FoldoutGroup("Grid", true)] public bool useFrequency;
    [FoldoutGroup("Grid", true)] public int rowCount;
    [FoldoutGroup("Grid", true)] public int columnCount;
    
    // Camera
    [FoldoutGroup("Camera", true)] public float cameraZOffset = -10;
    [FoldoutGroup("Camera", true)] public float cameraPadding;
}
