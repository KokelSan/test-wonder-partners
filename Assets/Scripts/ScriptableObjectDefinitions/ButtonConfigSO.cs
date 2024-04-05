using UnityEngine;

[CreateAssetMenu(menuName = "Wonder Partner's/Button Config/New Config",  fileName = "_ButtonConfig")]
public class ButtonConfigSO : ScriptableObject
{
    public string DisplayName;
    public Sprite ActiveIcon;
    public Sprite InactiveIcon;
}