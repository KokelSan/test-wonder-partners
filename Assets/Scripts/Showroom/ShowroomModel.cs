using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShowroomNavigationTransform
{
    public NavigationLabel Label;
    public Vector3 Position;
    public Vector3 Rotation;
    public bool IsStartingTransform;
}

public class ShowroomModel : MonoBehaviour
{
    public List<ShowroomNavigationTransform> NavigationTransforms;
    public float TransitionDuration;

    public void PerformTransition(NavigationLabel navLabel)
    {
        ShowroomNavigationTransform navTransform = NavigationTransforms.Find(navTransf => navTransf.Label == navLabel);
        if (navTransform != null)
        {
            transform.DOLocalMove(navTransform.Position, TransitionDuration);
            transform.DOLocalRotate(navTransform.Rotation, TransitionDuration);
            return;
        }
        Debug.LogError($"Navigation transform not found for label '{navLabel}'");
    }
}