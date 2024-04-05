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

public class ViewModifier : MonoBehaviour
{
    [SerializeField] private ModelVisibilityConfigSO ModelVisibilityConfig;
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
            transform.DOLocalMove(view.Position, ModelVisibilityConfig.ViewTransitionConfig.Duration).SetEase(ModelVisibilityConfig.ViewTransitionConfig.Ease);
            transform.DOLocalRotate(view.Rotation, ModelVisibilityConfig.ViewTransitionConfig.Duration).SetEase(ModelVisibilityConfig.ViewTransitionConfig.Ease);
            return;
        }
        Debug.LogError($"Model View not found for buttonConfig '{buttonConfig.name}'");
    }

    
    public void SetVisibility(bool isVisible)
    {
        if (isVisible)
        {
            transform.DOScale(Vector3.one, ModelVisibilityConfig.ShowConfig.Duration).SetEase(ModelVisibilityConfig.ShowConfig.Ease);
            return;
        }
        transform.DOScale(Vector3.zero, ModelVisibilityConfig.HideConfig.Duration).SetEase(ModelVisibilityConfig.HideConfig.Ease);
    }

    public void Destroy()
    {
        transform.DOScale(Vector3.zero, ModelVisibilityConfig.HideConfig.Duration).SetEase(ModelVisibilityConfig.HideConfig.Ease).
            OnComplete(() =>
            {
                Destroy(this);
            });
    }
}