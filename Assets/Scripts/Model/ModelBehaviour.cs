using System;
using System.Collections.Generic;
using UnityEngine;

public class ModelBehaviour : MonoBehaviour
{
    private ModelMaterialCreator _materialCreator;
    private ModelViewModifier _viewModifier;

    public void Initialize(Action onModelReady)
    {
        transform.localScale = Vector3.zero;
        
        if (TryGetComponent(out _materialCreator))
        {
            _materialCreator.StartMaterialCreation(onModelReady);
        }
        else
        {
            Debug.LogWarning($"There is no MaterialCreator component on model '{name}'");
        }
        
        if (!TryGetComponent(out _viewModifier))
        {
            Debug.LogWarning($"There is no ViewModifier component on model '{name}'");
        }
    }
    
    public void RequestVisibilityModification(bool isVisible)
    {
        if (_viewModifier)
        {
            _viewModifier.SetVisibility(isVisible);
        }
    }
    
    public void RequestViewModification(ButtonConfigSO buttonConfig)
    {
        if (_viewModifier)
        {
            _viewModifier.ModifyView(buttonConfig);
        }
    }

    public void RequestDestroy()
    {
        if (_viewModifier)
        {
            _viewModifier.Destroy();
        }
    }

    public List<ModelView> GetModelViews()
    {
        if (_viewModifier)
        {
            return _viewModifier.Views;
        }

        return null;
    }
}