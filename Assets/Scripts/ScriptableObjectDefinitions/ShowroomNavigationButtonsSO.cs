using System;
using System.Collections.Generic;
using UnityEngine;

public enum NavigationLabel
{
    Front,
    Left,
    Right,
}

[Serializable]
public class NavigationButton
{
    public NavigationLabel Label;
    public Sprite ActiveIcon;
    public Sprite InactiveIcon;
}

[CreateAssetMenu(menuName = "Wonder Partner's/Showroom buttons/New buttons definition",  fileName = "_ShowroomNavigationButtonsSO")]
public class ShowroomNavigationButtonsSO : ScriptableObject
{
    public List<NavigationButton> Buttons;
}