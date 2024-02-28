using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Generated;
using UnityEngine;

public class PlaySceneManager : MonoBehaviourSingleton<PlaySceneManager>
{
    [SerializeField] private GameObject _loadingBlock;
    
    private readonly Queue<FlowManagementData> _flowDataQueue = new();
    private bool _isForceQuit = false;
    
    private void Awake()
    {
        _loadingBlock.SetActive(true);
    }

    private void Start()
    {
        _isForceQuit = false;
        PlayGameAsync().Forget();
    }
    
    private async UniTask PlayGameAsync()
    {
        await GameDataManager.Instance.LoadGameData();
        
        var savedFlow = LocalDataManager.Instance.GetIntPlayerPrefs(LocalDataType.SaveFlow);
        if (savedFlow == null)
        {
            savedFlow = 0;
            LocalDataManager.Instance.SaveIntPlayerPrefs(LocalDataType.SaveFlow, savedFlow.Value);
        }

        foreach (var data in GameDataManager.Instance.FlowManagementDataList)
        {
            if (data.FlowOrder <= savedFlow) continue;
            _flowDataQueue.Enqueue(data);
        }

        try
        {
            while (true)
            {
                var flowData = _flowDataQueue.Dequeue();
                await PlayFlowAsync(flowData);
                if (_isForceQuit) break;

                LocalDataManager.Instance.SaveIntPlayerPrefs(LocalDataType.SaveFlow, flowData.FlowOrder);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }
        finally
        {
            _flowDataQueue.Clear();
        }
    }

    private async UniTask PlayFlowAsync(FlowManagementData data)
    {
        switch (data.FlowType)
        {
            default:
            {
                Debug.LogError("case 대응해주세요!!");
                _isForceQuit = true;
                break;
            }
        }
    }
}
