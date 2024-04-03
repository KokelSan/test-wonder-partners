using UnityEngine;

public enum ViewLabel
{
    Front,
    Left,
    Right,
}

[CreateAssetMenu(menuName = "Wonder Partner's/Bottom bar button/New button definition",  fileName = "_ButtonDef")]
public class BottomBarButtonSO : ScriptableObject
{
    public ViewLabel Label;
    public Sprite ActiveIcon;
    public Sprite InactiveIcon;
}