using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowroomManager : MonoBehaviour
{
    public List<ShowroomModel> Models;
    public ShowroomNavigationButtonsSO ButtonsDefinition;
    public HorizontalLayoutGroup ButtonsParent;
    public ShowroomNavigationButton NavigationButtonPrefab;

    private int _currentModelIndex = 0;
    private List<ShowroomNavigationButton> _instantiatedButtons = new List<ShowroomNavigationButton>();
    private int _currentActiveButtonIndex = 0;

    private void Start()
    {
        LoadModel();
    }

    private void LoadModel()
    {
        ShowroomModel model = Models[_currentModelIndex];
        CreateButtonsForModel(model);
    }

    private void CreateButtonsForModel(ShowroomModel model)
    {
        foreach (ShowroomNavigationTransform navTransform in model.NavigationTransforms)
        {
            NavigationButton navButton = ButtonsDefinition.Buttons.Find(button => button.Label == navTransform.Label);
            if (navButton != null)
            {
                ShowroomNavigationButton showroomButton = Instantiate(NavigationButtonPrefab, ButtonsParent.transform);
                showroomButton.SetButton(navButton.Label, navButton.ActiveIcon, navButton.InactiveIcon, navTransform.IsStartingTransform);
                _instantiatedButtons.Add(showroomButton);

                int index = _instantiatedButtons.Count - 1;
                showroomButton.ActivationButton.onClick.AddListener(() => OnButtonClicked(index));
                if (navTransform.IsStartingTransform) _currentActiveButtonIndex = index;
                
                continue;
            }
            Debug.LogWarning($"Button with label '{navTransform.Label}' required by model '{model.name}' is not defined");
        }
    }    

    private void OnButtonClicked(int buttonIndex)
    {
        ShowroomNavigationButton oldButton = _instantiatedButtons[_currentActiveButtonIndex];
        ShowroomNavigationButton newButton = _instantiatedButtons[buttonIndex];
        Models[_currentModelIndex].PerformTransition(newButton.Label);

        oldButton.SetActive(false);      
        newButton.SetActive(true);

        _currentActiveButtonIndex = buttonIndex;        
    }

    private void OnDestroy()
    {
        foreach (var button in _instantiatedButtons)
        {
            button.ActivationButton.onClick.RemoveAllListeners();           
        }
    }
}