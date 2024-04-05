using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour
{
    [SerializeField] private List<ModelBehaviour> ModelPrefabs;
    
    private BottomBarManager _bottomBarManager;
    private ModelBehaviour _currentModel;    

    private void Start()
    {
        _bottomBarManager = FindObjectOfType<BottomBarManager>();
        if (_bottomBarManager == null)
        {
            Debug.LogError("There is no Bottom Bar Manager in the scene!");
        }
        _bottomBarManager.Initialize();
        
        LoadModel(0);
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
        _currentModel.name = modelPrefab.name;
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
        
        List<ModelView> views = _currentModel.GetModelViews();
        if (views == null)
        {
            Debug.LogError($"Model '{_currentModel}' has no registered views");
            return;
        }
        
        _bottomBarManager.CreateButtonsForViews(views, OnButtonClicked);
    }

    private void OnButtonClicked(ButtonConfigSO buttonConfig)
    {
        _currentModel.RequestViewModification(buttonConfig);
    }
}