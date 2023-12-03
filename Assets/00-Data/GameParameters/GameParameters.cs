using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameParameters : ScriptableObject
{
    // Scene Transition
    public float fadeDuration;
    
    // Scoring
    public int scorePerWord;
    
    // Progression
    public List<GameLevel> levels;
}
