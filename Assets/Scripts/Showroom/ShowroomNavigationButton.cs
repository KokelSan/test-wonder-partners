using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Animator))]
public class ShowroomNavigationButton : MonoBehaviour
{
    private const string ShowAnimatorTriggerName = "Show";
    private const string HideAnimatorTriggerName = "Hide";

    public Button ActivationButton;
    public GameObject ActiveElements;
    public GameObject AnimatedBackground;
    
    [SerializeField] private TMP_Text DisplayNameText;
    [SerializeField] private Image ActiveImage;
    [SerializeField] private Image InactiveImage;

    [SerializeField] private Ease ButtonTransitionEase;
    [SerializeField] private float ButtonTransitionDuration;

    public NavigationLabel Label => _label;
    private NavigationLabel _label;
    private Animator _animator;

    private void Start()
    {
        if (!TryGetComponent(out _animator))
        {
            Debug.LogError($"{name} has no animator");
        }
    }

    public void SetButton(NavigationLabel navLabel, Sprite activeImage, Sprite inactiveImage, bool isActive)
    {
        name = $"{navLabel}_Button";
        _label = navLabel;
        DisplayNameText.text = navLabel.ToString();
        ActiveImage.sprite = activeImage;
        InactiveImage.sprite = inactiveImage;
        SetActive(isActive);
    }

    public void SetActive(bool isActive)
    {
        Vector3 targetScale = isActive ? Vector3.one : Vector3.zero;
        ActiveElements.transform.DOScale(targetScale, ButtonTransitionDuration).SetEase(ButtonTransitionEase);
        ActivationButton.interactable = !isActive;
    }
}