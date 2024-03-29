using UnityEngine;

[CreateAssetMenu(menuName = "Wonder Partner's/New Model Config",  fileName = "_ModelConfigSO")]
public class ModelConfigSO : ScriptableObject
{
    public GameObject Model;
    public string BaseMapURL;
    public string EmissiveURL;
    public string MetallicRoughnessURL;
    public string NormalURL;
    public string OcclusionURL;
}
