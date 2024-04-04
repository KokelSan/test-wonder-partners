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
    public TweenConfig TransitionConfig;
    public TweenConfig ShowHideConfig;

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public void ChangeView(ViewLabel newView)
    {
        ModelView view = Views.Find(navTransf => navTransf.Label == newView);
        if (view != null)
        {
            transform.DOLocalMove(view.Position, TransitionConfig.Duration).SetEase(TransitionConfig.Ease);
            transform.DOLocalRotate(view.Rotation, TransitionConfig.Duration).SetEase(TransitionConfig.Ease);
            return;
        }
        Debug.LogError($"Navigation transform not found for label '{newView}'");
    }

    public void Show()
    {
        transform.DOScale(Vector3.one, ShowHideConfig.Duration).SetEase(ShowHideConfig.Ease);
    }

    public void Destroy()
    {
        transform.DOScale(Vector3.zero, ShowHideConfig.Duration).SetEase(ShowHideConfig.Ease).
            OnComplete(() =>
            {
                Destroy(this);
            });
    }
}