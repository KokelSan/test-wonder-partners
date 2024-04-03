using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModelView
{
    public ViewLabel Label;
    public Vector3 Position;
    public Vector3 Rotation;
    public bool IsStartingView;
}

public class Model : MonoBehaviour
{
    public List<ModelView> Views;
    public float TransitionDuration;

    public void PerformTransition(ViewLabel navLabel)
    {
        ModelView view = Views.Find(navTransf => navTransf.Label == navLabel);
        if (view != null)
        {
            transform.DOLocalMove(view.Position, TransitionDuration);
            transform.DOLocalRotate(view.Rotation, TransitionDuration);
            return;
        }
        Debug.LogError($"Navigation transform not found for label '{navLabel}'");
    }
}