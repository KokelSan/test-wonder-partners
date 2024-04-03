using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour
{
    public List<Model> Models;
    public BottomBarManager BottomBarManager;

    private int _currentModelIndex = 0;    

    private void Start()
    {
        LoadModel();
    }

    private void LoadModel()
    {
        Model currentModel = Models[_currentModelIndex];
        if (currentModel != null)
        {
            List<ButtonCreationRequest> requests = new List<ButtonCreationRequest>();
            foreach (var view in currentModel.Views)
            {
                requests.Add(new ButtonCreationRequest(view.Label, view.IsStartingView));
            }
            BottomBarManager.CreateButtons(requests, OnButtonClicked);
        }
    }

    private void OnButtonClicked(ViewLabel label)
    {
        Models[_currentModelIndex].PerformTransition(label);       
    }    
}