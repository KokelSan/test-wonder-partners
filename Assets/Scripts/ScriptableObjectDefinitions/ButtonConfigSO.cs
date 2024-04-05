using UnityEngine;

[CreateAssetMenu(menuName = "Wonder Partner's/Button Config/New button",  fileName = "_ButtonConfig")]
public class ButtonConfigSO : ScriptableObject
{
    public string DisplayName;
    public Sprite ActiveIcon;
    public Sprite InactiveIcon;
}