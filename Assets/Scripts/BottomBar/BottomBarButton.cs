using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BottomBarButton : MonoBehaviour
{
    [SerializeField] private BaseVisibilityConfigSO VisibilityConfig;
    
    [SerializeField] private GameObject ActiveElements;
    [SerializeField] private Button ActivationButton;
    
    [SerializeField] private TMP_Text DisplayNameText;
    [SerializeField] private Image ActiveImage;
    [SerializeField] private Image InactiveImage;

    public ButtonConfigSO Config => _currentConfig;
    private ButtonConfigSO _currentConfig;

    public void SetButtonForView(ModelView view, UnityAction onButtonClicked)
    {
        _currentConfig = view.ButtonConfig;
        
        name = $"{_currentConfig.DisplayName}_Button";
        DisplayNameText.text = _currentConfig.DisplayName;
        ActiveImage.sprite = _currentConfig.ActiveIcon;
        InactiveImage.sprite = _currentConfig.InactiveIcon;
        ActivationButton.onClick.AddListener(onButtonClicked);
        
        SetActive(view.IsStartingView);
    }

    public void SetActive(bool isActive)
    {
        if (isActive)
        {
            ActiveElements.transform.DOScale(Vector3.one, VisibilityConfig.ShowConfig.Duration).SetEase(VisibilityConfig.ShowConfig.Ease);
        }
        else
        {
            ActiveElements.transform.DOScale(Vector3.zero, VisibilityConfig.HideConfig.Duration).SetEase(VisibilityConfig.HideConfig.Ease);
        }
        
        ActivationButton.interactable = !isActive;
    }

    private void OnDestroy()
    {
        ActivationButton.onClick.RemoveAllListeners();
    }
}