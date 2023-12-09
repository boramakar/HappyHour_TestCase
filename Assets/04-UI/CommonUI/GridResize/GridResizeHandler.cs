using System.Collections;
using System.Collections.Generic;
using HappyTroll;
using TMPro;
using UnityEngine;

public class GridResizeHandler : MonoBehaviour
{
    [SerializeField] private GameObject popup;
    [SerializeField] private TMP_InputField columnCountTextMesh;
    [SerializeField] private TMP_InputField rowCountTextMesh;

    public void TogglePopup(bool isActive)
    {
        popup.SetActive(isActive);
    }
    
    public void ResizeGrid()
    {
        EventManager.GridResizeEvent(int.Parse(rowCountTextMesh.text), int.Parse(columnCountTextMesh.text));
    }
}
