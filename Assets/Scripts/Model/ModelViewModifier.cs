using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModelView
{
    public ButtonConfigSO ButtonConfig;
    public Vector3 Position;
    public Vector3 Rotation;
    public bool IsStartingView;
}

public class ModelViewModifier : MonoBehaviour
{
    [SerializeField] private ModelTweenConfigSO tweenConfig;
    public List<ModelView> Views;

    private Dictionary<ButtonConfigSO, ModelView> _buttonToViewDict = new Dictionary<ButtonConfigSO, ModelView>();

    private void Start()
    {
        transform.localPosition = Views.Find(view => view.IsStartingView).Position;
        transform.localScale = Vector3.zero;
        
        _buttonToViewDict.Clear();
        foreach (ModelView view in Views)
        {
            _buttonToViewDict.Add(view.ButtonConfig, view);
        }
    }
    
    public void ModifyView(ButtonConfigSO buttonConfig)
    {
        if (_buttonToViewDict.TryGetValue(buttonConfig, out ModelView view))
        {
            transform.DOLocalMove(view.Position, tweenConfig.ViewTransitionConfig.Duration).SetEase(tweenConfig.ViewTransitionConfig.Ease);
            transform.DOLocalRotate(view.Rotation, tweenConfig.ViewTransitionConfig.Duration).SetEase(tweenConfig.ViewTransitionConfig.Ease);
            return;
        }
        Debug.LogError($"Model View not found for buttonConfig '{buttonConfig.name}'");
    }

    
    public void SetVisibility(bool isVisible)
    {
        if (isVisible)
        {
            transform.DOScale(Vector3.one, tweenConfig.ShowConfig.Duration).SetEase(tweenConfig.ShowConfig.Ease);
            return;
        }
        transform.DOScale(Vector3.zero, tweenConfig.HideConfig.Duration).SetEase(tweenConfig.HideConfig.Ease);
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