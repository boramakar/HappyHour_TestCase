using System.Collections;
using System.Collections.Generic;
using HappyTroll;
using UnityEngine;

public class ProgressionModeUpdater : MonoBehaviour
{
    [SerializeField] private Enums.ProgressionType progressionType;

    public void OnClick()
    {
        EventManager.UpdateProgressionModeEvent(progressionType);
    }
}
