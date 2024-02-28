using System.Collections.Generic;
using Generated;
using System;
using Cysharp.Threading.Tasks;
using UnityEditor;

public partial class GameDataManager : Singleton<GameDataManager>
{
    public async UniTask LoadGameData()
    {
        await LoadFlowManagementData();
        await LoadChatData();
        await LoadChatProfileData();
    }

    public List<FlowManagementData> FlowManagementDataList { get; private set; }

    private async UniTask LoadFlowManagementData()
    {
        var loadedData = await CodeGenerator.GetGoogleSheetData("FlowManagementData");

        FlowManagementDataList = new List<FlowManagementData>();
        var rows = loadedData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        for (var i = 2; i < rows.Length; i++)
        {
            var columns = rows[i].TrimEnd('\r').Split('\t');
            FlowManagementDataList.Add(new FlowManagementData(Enum.Parse<FlowType>(columns[1]), int.Parse(columns[2]), columns[3], columns[4]));
        }
    }
    public List<ChatData> ChatDataList { get; private set; }

    private async UniTask LoadChatData()
    {
        var loadedData = await CodeGenerator.GetGoogleSheetData("ChatData");

        ChatDataList = new List<ChatData>();
        var rows = loadedData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        for (var i = 2; i < rows.Length; i++)
        {
            var columns = rows[i].TrimEnd('\r').Split('\t');
            ChatDataList.Add(new ChatData(int.Parse(columns[0]), int.Parse(columns[1]), Enum.Parse<ChatProfileType>(columns[2]), columns[3], columns[4]));
        }
    }
    public List<ChatProfileData> ChatProfileDataList { get; private set; }

    private async UniTask LoadChatProfileData()
    {
        var loadedData = await CodeGenerator.GetGoogleSheetData("ChatProfileData");

        ChatProfileDataList = new List<ChatProfileData>();
        var rows = loadedData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        for (var i = 2; i < rows.Length; i++)
        {
            var columns = rows[i].TrimEnd('\r').Split('\t');
            ChatProfileDataList.Add(new ChatProfileData(Enum.Parse<ChatProfileType>(columns[0]), columns[1], columns[2]));
        }
    }
}
