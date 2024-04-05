using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BottomBarButton : MonoBehaviour
{
    [SerializeField] private VisibilityTweenConfigSO VisibilityTweenConfig;
    
    [SerializeField] private Button ActivationButton;
    [SerializeField] private GameObject ActiveElements;
    
    [SerializeField] private TMP_Text DisplayNameText;
    [SerializeField] private Image ActiveImage;
    [SerializeField] private Image InactiveImage;

    public ButtonConfigSO Config => _currentConfig;
    private ButtonConfigSO _currentConfig;

    public void SetButtonForView(ModelView view, UnityAction onButtonClicked)
    {
        ButtonConfigSO buttonConfig = view.ButtonConfig;
        
        name = $"{buttonConfig.DisplayName}_Button";
        DisplayNameText.text = buttonConfig.DisplayName;
        ActiveImage.sprite = buttonConfig.ActiveIcon;
        InactiveImage.sprite = buttonConfig.InactiveIcon;
        ActivationButton.onClick.AddListener(onButtonClicked);
        _currentConfig = view.ButtonConfig;
        
        SetActive(view.IsStartingView);
    }

    public void SetActive(bool isActive)
    {
        if (isActive)
        {
            ActiveElements.transform.DOScale(Vector3.one, VisibilityTweenConfig.ShowConfig.Duration).SetEase(VisibilityTweenConfig.ShowConfig.Ease);
        }
        else
        {
            ActiveElements.transform.DOScale(Vector3.zero, VisibilityTweenConfig.HideConfig.Duration).SetEase(VisibilityTweenConfig.HideConfig.Ease);
        }
        
        ActivationButton.interactable = !isActive;
    }

    private void OnDestroy()
    {
        ActivationButton.onClick.RemoveAllListeners();
    }
}