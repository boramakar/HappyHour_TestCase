using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressionDisplay : MonoBehaviour
{
    [SerializeField] private Enums.ProgressionType progressionType;
    
    private TextMeshProUGUI _textMesh;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        _textMesh.text = ProgressionManager.Instance.GetProgressionValue(progressionType);
    }
}
