using UnityEngine;

public class FileDeletionManager : MonoBehaviour
{
    [SerializeField] private bool DeleteCreatedFilesOnQuit = true;
    
    private void OnDestroy()
    {
        if(DeleteCreatedFilesOnQuit) FileIOService.DeleteAllCreatedFiles();
    }
}