using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BottomBarButton : MonoBehaviour
{
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

    public void SetActive(bool isActive, Ease transitionEase = Ease.Unset, float transitionDuration = 0)
    {
        Vector3 targetScale = isActive ? Vector3.one : Vector3.zero;
        ActiveElements.transform.DOScale(targetScale, transitionDuration).SetEase(transitionEase);
        ActivationButton.interactable = !isActive;
    }

    private void OnDestroy()
    {
        ActivationButton.onClick.RemoveAllListeners();
    }
}