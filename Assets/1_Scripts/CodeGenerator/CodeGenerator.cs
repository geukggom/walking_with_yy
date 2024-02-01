using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public static class CodeGenerator
{
    public const string GeneratedGameDataNameSpace = "Generated";
    private static Encoding UTF8NoBom => new UTF8Encoding(false);
    private const string Data = "Data";
    
    public static async UniTask Generate()
    {
        var loadedDataList = await GetDataForGenerate();
        await EnumCodeGenerator.Generate(loadedDataList);
        await DataCodeGenerator.Generate(loadedDataList);
    }

    public static async UniTask WriteGameDataToFileAsync(string filePath, StringBuilder contents)
    {
        var directoryName = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        await File.WriteAllTextAsync(filePath, contents.ToString(), CodeGenerator.UTF8NoBom);

        Debug.Log("Complete Generate GameData");
    }
    
    private static async UniTask<List<SheetInfo>> GetDataForGenerate()
    {
        var config = AssetDatabase.LoadAssetAtPath<GoogleSheetsConfig>(GoogleSheetsConfig.FilePath);
        var googleSheetsDataList = config.GoogleSheetsDataList;
        var googleSheetsCount = googleSheetsDataList.Length;
        var loadedData = new List<SheetInfo>();
        for (var i = 0; i < googleSheetsCount; i++)
        {
            var text = await LoadGoogleSheetsData(googleSheetsDataList[i].SheetId);
            var rows = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (rows.Length < 2)
            {
                Debug.LogError($"{googleSheetsDataList[i].SheetName} is not match columns with type and name.");
                break;
            }

            var columnTypes = rows[0].TrimEnd('\r').Split('\t', StringSplitOptions.RemoveEmptyEntries);
            var columnNames = rows[1].TrimEnd('\r').Split('\t', StringSplitOptions.RemoveEmptyEntries);
            if (columnTypes.Length != columnNames.Length)
            {
                Debug.LogError($"{googleSheetsDataList[i].SheetName} is not match columns with type and name.");
                break;
            }

            var enumDic = new Dictionary<int, List<string>>();
            for (var j = 0; j < columnTypes.Length; j++)
            {
                if (columnTypes[j].ToLower() != DataCodeGenerator.TypeEnum) continue;

                enumDic.Add(j, new List<string>());
            }

            for (var j = 2; j < rows.Length; j++)
            {
                var columns = rows[j].TrimEnd('\r').Split('\t');
                for (var k = 0; k < columns.Length; k++)
                {
                    if (!enumDic.ContainsKey(k)) continue;
                    if (enumDic[k].Contains(columns[k])) continue;

                    enumDic[k].Add(columns[k]);
                }
            }

            loadedData.Add(new SheetInfo
            {
                SheetName = googleSheetsDataList[i].SheetName + Data,
                ColumnTypes = columnTypes,
                ColumnNames = columnNames,
                EnumList = enumDic,
            });
        }

        return loadedData;
    }

    private static async UniTask<string> LoadGoogleSheetsData(int sheetId)
    {
        var config = AssetDatabase.LoadAssetAtPath<GoogleSheetsConfig>(GoogleSheetsConfig.FilePath);
        var www = UnityWebRequest.Get(config.GetGoogleSheetsAddress(sheetId));
        await www.SendWebRequest().ToUniTask();
        return www.downloadHandler.text;
    }

    public static async UniTask<string> GetGoogleSheetData(string sheetName)
    {
        sheetName = sheetName.Replace(Data, "");
        var config = AssetDatabase.LoadAssetAtPath<GoogleSheetsConfig>(GoogleSheetsConfig.FilePath);
        foreach (var data in config.GoogleSheetsDataList)
        {
            if (!data.SheetName.Equals(sheetName)) continue;
            return await LoadGoogleSheetsData(data.SheetId);
        }

        return string.Empty;
    }
}
