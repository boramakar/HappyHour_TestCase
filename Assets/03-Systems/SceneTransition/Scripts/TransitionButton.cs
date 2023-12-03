using System.Collections;
using System.Collections.Generic;
using HappyTroll;
using UnityEngine;

public class TransitionButton : MonoBehaviour
{
    [SerializeField] private Enums.SceneType transitionScene;
    [SerializeField] private bool useLoadingScene;

    public void OnClick()
    {
        TransitionManager.Instance.ChangeScene(transitionScene, useLoadingScene);
    }
}