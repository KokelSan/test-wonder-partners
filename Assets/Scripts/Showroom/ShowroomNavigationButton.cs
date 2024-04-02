using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Animator))]
public class ShowroomNavigationButton : MonoBehaviour
{
    private const string ShowAnimatorTriggerName = "Show";
    private const string HideAnimatorTriggerName = "Hide";

    public Button ActivationButton;
    [SerializeField] private GameObject ActiveElements;
    
    [SerializeField] private TMP_Text DisplayNameText;
    [SerializeField] private Image ActiveImage;
    [SerializeField] private Image InactiveImage;

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
        ActiveElements.SetActive(isActive);
        ActivationButton.interactable = !isActive;
    }
}