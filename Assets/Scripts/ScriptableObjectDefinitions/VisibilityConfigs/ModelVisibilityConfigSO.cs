using UnityEngine;

[CreateAssetMenu(menuName = "Wonder Partner's/Visibility Config/New Model Config",  fileName = "_ModelVisibilityConfig")]
public class ModelVisibilityConfigSO : BaseVisibilityConfigSO
{
    [Header("View transition animation config")]
    public TweenConfig ViewTransitionConfig;
}