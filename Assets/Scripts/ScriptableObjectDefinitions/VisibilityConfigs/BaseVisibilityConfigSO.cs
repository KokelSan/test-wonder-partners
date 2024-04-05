using UnityEngine;
using DG.Tweening;
using System;

[Serializable]
public struct TweenConfig
{
    public Ease Ease;
    public float Duration;
}

[CreateAssetMenu(menuName = "Wonder Partner's/Visibility Config/New Base Config",  fileName = "_BaseVisibilityConfig")]
public class BaseVisibilityConfigSO : ScriptableObject
{
    [Header("Start visibility")]
    public bool HideOnStart = true;
    
    [Header("Show/hide animations config")]
    public TweenConfig ShowConfig;
    public TweenConfig HideConfig;
}