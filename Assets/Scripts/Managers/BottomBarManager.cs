using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct ButtonCreationRequest
{
    public ViewLabel Label;
    public bool IsStartingView;

    public ButtonCreationRequest(ViewLabel label, bool isStartingView)
    {
        Label = label;
        IsStartingView = isStartingView;
    }
}

public class BottomBarManager : MonoBehaviour
{
    [SerializeField] private VisibilityTweenConfigSO VisibilityTweenConfig;
    [SerializeField] private bool HideOnStart;
    
    [Header("Buttons instantiation")]
    [SerializeField] private HorizontalLayoutGroup ButtonsParent;
    [SerializeField] private BottomBarButton ButtonPrefab;
    
    [Header("Buttons definition")]
    [SerializeField] private List<BottomBarButtonSO> ButtonsDefinition;
    
    private Vector3 _buttonsParentInitialScale;
    private List<BottomBarButton> _instantiatedButtons = new List<BottomBarButton>();
    private int _currentActiveButtonIndex = 0;

    private void Start()
    {
        if(!HideOnStart) return;
        
        _buttonsParentInitialScale = ButtonsParent.transform.localScale;
        ButtonsParent.transform.localScale = Vector3.zero;
    }

    public void CreateAndShow(List<ButtonCreationRequest> requests, Action<ViewLabel> onClickActionForModelManager)
    {
        foreach (var request in requests)
        {
            BottomBarButtonSO buttonDef = ButtonsDefinition.Find(button => button.Label == request.Label);
            if (buttonDef != null)
            {
                BottomBarButton bottomBarButton = Instantiate(ButtonPrefab, ButtonsParent.transform);
                int index = _instantiatedButtons.Count;
                bottomBarButton.SetButton(buttonDef, request.IsStartingView, () => OnButtonClicked(index, onClickActionForModelManager));
                if (request.IsStartingView) _currentActiveButtonIndex = index;
                _instantiatedButtons.Add(bottomBarButton);

                continue;
            }
            Debug.LogWarning($"Button with label '{buttonDef.Label}' is not defined");
        }
        ButtonsParent.transform.DOScale(_buttonsParentInitialScale, VisibilityTweenConfig.ShowConfig.Duration).SetEase(VisibilityTweenConfig.ShowConfig.Ease);
    }

    private void OnButtonClicked(int buttonIndex, Action<ViewLabel> onClickActionForModelManager)
    {
        _instantiatedButtons[_currentActiveButtonIndex].SetActive(false);
        BottomBarButton newButton = _instantiatedButtons[buttonIndex];
        newButton.SetActive(true);
        _currentActiveButtonIndex = buttonIndex;
        onClickActionForModelManager?.Invoke(newButton.Label);
    }
}