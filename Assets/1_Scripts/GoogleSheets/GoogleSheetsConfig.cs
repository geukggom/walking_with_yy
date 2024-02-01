using System;
using UnityEngine;

[Serializable]
public class GoogleSheetData
{
    [SerializeField] private string _sheetName;
    [SerializeField] private int _sheetId;

    public string SheetName => _sheetName;
    public int SheetId => _sheetId;
}

[CreateAssetMenu(menuName = "GoogleSheetsConfig")]
public class GoogleSheetsConfig : ScriptableObject
{
    [SerializeField] private string _googleSheetsUrl;
    [SerializeField] private GoogleSheetData[] _googleSheetsDataList;

    public const string FilePath = "Assets/Resources/GoogleSheetsConfig.asset";
    public GoogleSheetData[] GoogleSheetsDataList => _googleSheetsDataList;
    public string GetGoogleSheetsAddress(long sheetId) => $"{_googleSheetsUrl}/export?format=tsv&range=A1:Z&gid={sheetId}";
}