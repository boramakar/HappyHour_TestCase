using System.Collections;
using System.Collections.Generic;
using HappyTroll;
using UnityEngine;

public class GridRefillButtonHandler : MonoBehaviour
{
    public void OnClick()
    {
        EventManager.GridRefillEvent();
    }
}
