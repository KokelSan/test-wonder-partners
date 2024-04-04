﻿using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour
{
    public bool DeleteDownloadedTexturesOnQuit;
    public List<Model> ModelPrefabs;
    public BottomBarManager BottomBarManager;

    private Model _currentModel;    

    private void Start()
    {
        LoadModel(0);
    }

    private void LoadModel(int index)
    {
        if(ModelPrefabs.Count == 0)
        {
            Debug.LogError("Model manager has no model registered");
            return;
        }

        if(index < 0 || index >= ModelPrefabs.Count)
        {
            Debug.LogError($"Index {index} is outside the Models list's boundaries");
            return;
        }

        Model modelPrefab = ModelPrefabs[index];
        if (modelPrefab == null)
        {
            Debug.LogError($"Model prefab at index {index} is null");
            return;
        }

        if(_currentModel != null) _currentModel.Destroy();
        _currentModel = Instantiate(modelPrefab, transform);
        if (_currentModel.TryGetComponent(out MaterialCreator materialCreator))
        {
            materialCreator.StartMaterialCreation(OnModelReady);
        }        
    }     

    private void OnModelReady()
    {
        _currentModel.Show();
        RequestButtonsCreation();
    }

    private void RequestButtonsCreation()
    {
        List<ButtonCreationRequest> requests = new List<ButtonCreationRequest>();
        foreach (ModelView view in _currentModel.Views)
        {
            requests.Add(new ButtonCreationRequest(view.Label, view.IsStartingView));
        }
        BottomBarManager.CreateButtons(requests, OnButtonClicked);
    }

    private void OnButtonClicked(ViewLabel label)
    {
        _currentModel.ChangeView(label);
    }

    private void OnDestroy()
    {
        if(DeleteDownloadedTexturesOnQuit) TextureDownloadService.CleanDownloads();
    }
}