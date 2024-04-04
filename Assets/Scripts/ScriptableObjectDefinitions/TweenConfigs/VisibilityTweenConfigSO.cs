using UnityEngine;
using DG.Tweening;
using System;

[Serializable]
public struct TweenConfig
{
    public Ease Ease;
    public float Duration;
}

[CreateAssetMenu(menuName = "Wonder Partner's/Tween Config/New Visibility Config",  fileName = "_VisibilityTweenConfig")]
public class VisibilityTweenConfigSO : ScriptableObject
{
    public TweenConfig ShowConfig;
    public TweenConfig HideConfig;
}