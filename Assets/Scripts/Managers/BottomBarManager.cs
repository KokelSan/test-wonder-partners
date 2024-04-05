using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomBarManager : MonoBehaviour
{
    [SerializeField] private BaseVisibilityConfigSO VisibilityConfig;
    
    [Header("Buttons instantiation")]
    [SerializeField] private HorizontalLayoutGroup ButtonsParent;
    [SerializeField] private BottomBarButton ButtonPrefab;
    
    private Vector3 _buttonsParentInitialScale;
    private List<BottomBarButton> _instantiatedButtons = new List<BottomBarButton>();
    private int _currentActiveButtonIndex = 0;

    public void Initialize()
    {
        _buttonsParentInitialScale = ButtonsParent.transform.localScale;
        
        if(!VisibilityConfig.HideOnStart) return;
        ButtonsParent.transform.localScale = Vector3.zero;
    }

    public void CreateButtonsForViews(List<ModelView> views, Action<ButtonConfigSO> modelManagerOnClickAction)
    {
        foreach (var view in views)
        {
            BottomBarButton bottomBarButton = Instantiate(ButtonPrefab, ButtonsParent.transform);
            int index = _instantiatedButtons.Count;
            bottomBarButton.SetButtonForView(view, () => OnButtonClicked(index, modelManagerOnClickAction));
            if (view.IsStartingView) _currentActiveButtonIndex = index;
            _instantiatedButtons.Add(bottomBarButton);
        }
        ButtonsParent.transform.DOScale(_buttonsParentInitialScale, VisibilityConfig.ShowConfig.Duration).SetEase(VisibilityConfig.ShowConfig.Ease);
    }

    private void OnButtonClicked(int buttonIndex, Action<ButtonConfigSO> modelManagerOnClickAction)
    {
        _instantiatedButtons[_currentActiveButtonIndex].SetActive(false);
        BottomBarButton newButton = _instantiatedButtons[buttonIndex];
        newButton.SetActive(true);
        _currentActiveButtonIndex = buttonIndex;
        modelManagerOnClickAction?.Invoke(newButton.Config);
    }
}