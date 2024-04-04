using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
    [SerializeField] private ModelTweenConfigSO tweenConfig;
    public List<ModelView> Views;

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public void ChangeView(ViewLabel newView)
    {
        ModelView view = Views.Find(navTransf => navTransf.Label == newView);
        if (view != null)
        {
            transform.DOLocalMove(view.Position, tweenConfig.ViewTransitionConfig.Duration).SetEase(tweenConfig.ViewTransitionConfig.Ease);
            transform.DOLocalRotate(view.Rotation, tweenConfig.ViewTransitionConfig.Duration).SetEase(tweenConfig.ViewTransitionConfig.Ease);
            return;
        }
        Debug.LogError($"Model View not found for label '{newView}'");
    }

    public void Show()
    {
        transform.DOScale(Vector3.one, tweenConfig.ShowConfig.Duration).SetEase(tweenConfig.ShowConfig.Ease);
    }

    public void Destroy()
    {
        transform.DOScale(Vector3.zero, tweenConfig.HideConfig.Duration).SetEase(tweenConfig.HideConfig.Ease).
            OnComplete(() =>
            {
                Destroy(this);
            });
    }
}