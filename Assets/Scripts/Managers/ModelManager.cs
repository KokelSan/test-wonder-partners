using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour
{
    [SerializeField] private bool DeleteCreatedFilesOnQuit = true;
    [SerializeField] private List<ModelBehaviour> ModelPrefabs;
    
    private BottomBarManager _bottomBarManager;
    private ModelBehaviour _currentModel;    

    private void Start()
    {
        LoadModel(0);

        _bottomBarManager = FindObjectOfType<BottomBarManager>();
        if (_bottomBarManager == null)
        {
            Debug.LogError("There is no Bottom Bar Manager in the scene!");
        }
    }

    private void LoadModel(int index)
    {
        if(ModelPrefabs.Count == 0)
        {
            Debug.LogError("Model manager has no model registered!");
            return;
        }

        if(index < 0 || index >= ModelPrefabs.Count)
        {
            Debug.LogError($"Index {index} is outside the models list's boundaries");
            return;
        }

        ModelBehaviour modelPrefab = ModelPrefabs[index];
        if (modelPrefab == null)
        {
            Debug.LogError($"Model prefab at index {index} is null");
            return;
        }

        if(_currentModel != null) _currentModel.RequestDestroy();
        _currentModel = Instantiate(modelPrefab, transform);
        _currentModel.Initialize(OnModelReady);
    }     

    private void OnModelReady()
    {
        _currentModel.RequestVisibilityModification(true);
        RequestBottomBar();
    }

    private void RequestBottomBar()
    {
        if (_bottomBarManager == null)
        {
            Debug.LogError("No Bottom Bar Manager in the scene, buttons creation aborted");
            return;
        }
        
        List<ButtonCreationRequest> requests = new List<ButtonCreationRequest>();
        List<ModelView> views = _currentModel.GetModelViews();
        if (views == null)
        {
            Debug.LogError($"Model '{_currentModel}' has no registered views");
            return;
        }
        
        foreach (ModelView view in views)
        {
            requests.Add(new ButtonCreationRequest(view.Label, view.IsStartingView));
        }
        _bottomBarManager.CreateAndShow(requests, OnButtonClicked);
    }

    private void OnButtonClicked(ViewLabel label)
    {
        _currentModel.RequestViewModification(label);
    }

    private void OnDestroy()
    {
        if(DeleteCreatedFilesOnQuit) FileIOService.DeleteAllCreatedFiles();
    }
}