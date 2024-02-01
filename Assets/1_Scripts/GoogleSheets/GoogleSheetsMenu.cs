using System;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class GoogleSheetsMenu : ScriptableObject
{
    private const string MenuRoot = "GoogleSheets";

    [MenuItem(MenuRoot + "/SelectAppConfig", false, 22)]
    public static void SelectAppConfig()
    {
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GoogleSheetsConfig>(GoogleSheetsConfig.FilePath);
    }

    [MenuItem(MenuRoot + "/GenerateGameDataSheets", false, 11)]
    public static void GenerateGameDataSheets()
    {
    }
}
#endif