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
    [SerializeField] private GameObject AnimatedBackground;
    
    [SerializeField] private TMP_Text DisplayNameText;
    [SerializeField] private Image ActiveImage;
    [SerializeField] private Image InactiveImage;

    public ViewLabel Label => _label;
    private ViewLabel _label;

    public void SetButton(BottomBarButtonSO buttonDef, bool isActive, UnityAction onButtonClicked)
    {
        _label = buttonDef.Label;
        name = $"{_label}_Button";
        DisplayNameText.text = _label.ToString();
        ActiveImage.sprite = buttonDef.ActiveIcon;
        InactiveImage.sprite = buttonDef.InactiveIcon;
        ActivationButton.onClick.AddListener(onButtonClicked);
        SetActive(isActive);
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