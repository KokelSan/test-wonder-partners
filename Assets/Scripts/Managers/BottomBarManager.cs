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
    [SerializeField] private List<BottomBarButtonSO> ButtonsDefinition;
    [SerializeField] private HorizontalLayoutGroup ButtonsParent;
    [SerializeField] private BottomBarButton ButtonPrefab;

    [SerializeField] private TweenConfig BottomBarShowHideConfig;
    [SerializeField] private TweenConfig ButtonActivationConfig;
    [SerializeField] private TweenConfig ButtonDeactivationConfig;

    private Vector3 _buttonsParentInitialScale;
    private List<BottomBarButton> _instantiatedButtons = new List<BottomBarButton>();
    private int _currentActiveButtonIndex = 0;

    private void Start()
    {
        _buttonsParentInitialScale = ButtonsParent.transform.localScale;
        ButtonsParent.transform.localScale = Vector3.zero;
    }

    public void CreateButtons(List<ButtonCreationRequest> requests, Action<ViewLabel> onButtonClicked)
    {
        foreach (var request in requests)
        {
            BottomBarButtonSO buttonDef = ButtonsDefinition.Find(button => button.Label == request.Label);
            if (buttonDef != null)
            {
                BottomBarButton bottomBarButton = Instantiate(ButtonPrefab, ButtonsParent.transform);
                int index = _instantiatedButtons.Count;
                bottomBarButton.SetButton(buttonDef, request.IsStartingView, () => OnButtonClicked(index, onButtonClicked));
                if (request.IsStartingView) _currentActiveButtonIndex = index;
                _instantiatedButtons.Add(bottomBarButton);

                continue;
            }
            Debug.LogWarning($"Button with label '{buttonDef.Label}' is not defined");
        }
        ButtonsParent.transform.DOScale(_buttonsParentInitialScale, BottomBarShowHideConfig.Duration).SetEase(BottomBarShowHideConfig.Ease);
    }

    private void OnButtonClicked(int buttonIndex, Action<ViewLabel> onButtonClicked)
    {
        BottomBarButton oldButton = _instantiatedButtons[_currentActiveButtonIndex];
        BottomBarButton newButton = _instantiatedButtons[buttonIndex];

        oldButton.SetActive(false, ButtonDeactivationConfig);
        newButton.SetActive(true, ButtonActivationConfig);
        onButtonClicked?.Invoke(newButton.Label);

        _currentActiveButtonIndex = buttonIndex;
    }
}