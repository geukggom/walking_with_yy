using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public struct SheetInfo
{
    public string SheetName { get; internal set; }
    public string[] ColumnTypes { get; internal set; }
    public string[] ColumnNames { get; internal set; }
    public Dictionary<int, List<string>> EnumList { get; internal set; }
}

public static class DataCodeGenerator
{
    private static string GameDataFilePath => $"{Application.dataPath}/1_Scripts/GeneratedGameData/GeneratedGameData.cs";
    private static string GameDataGetterFilePath => $"{Application.dataPath}/1_Scripts/GeneratedGameData/GeneratedGameDataGetter.cs";
    public const string TypeEnum = "enum";
    private const string TypeInt = "int";
    private const string TypeString = "string";
    private const string TypeListIne = "list<int>";

    private static readonly Dictionary<string, string> StringToTypes = new()
    {
        { TypeEnum, "enum" },
        { TypeInt, "int" },
        { TypeString, "string" },
        { TypeListIne, "List<int>" },
    };

    private static readonly Dictionary<string, string> StringToParse = new()
    {
        { TypeEnum, "Enum.Parse<@>" },
        { TypeInt, "int.Parse" },
        { TypeString, "" },
        { TypeListIne, "List<int>" },
    };

    private static readonly List<string> NotUsingColumns = new()
    {
        "not_using",
    };

    public static async UniTask Generate(List<SheetInfo> loadedDataList)
    {
        var getterSb = MakeDataGetter(loadedDataList);
        var generatedSb = MakeDataClass(loadedDataList);

        await CodeGenerator.WriteGameDataToFileAsync(GameDataGetterFilePath, getterSb);
        await CodeGenerator.WriteGameDataToFileAsync(GameDataFilePath, generatedSb);
    }

    private static StringBuilder MakeDataClass(List<SheetInfo> loadedDataList)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"namespace {CodeGenerator.GeneratedGameDataNameSpace}");
        sb.AppendLine("{");
        foreach (var loadedData in loadedDataList)
        {
            sb.AppendIndentedLine($"public class {loadedData.SheetName}", 1);
            sb.AppendIndentedLine("{", 1);

            for (var i = 0; i < loadedData.ColumnNames.Length; i++)
            {
                if (NotUsingColumns.Contains(loadedData.ColumnTypes[i])) continue;

                var columnName = loadedData.ColumnNames[i];
                if (loadedData.ColumnTypes[i].ToLower() == TypeEnum)
                {
                    sb.AppendIndentedLine($"public {columnName} {columnName}" + " { get; private set; }", 2);
                }
                else
                {
                    var columnType = StringToTypes[loadedData.ColumnTypes[i]];
                    sb.AppendIndentedLine($"public {columnType} {columnName}" + " { get; private set; }", 2);
                }
            }

            sb.AppendLine();

            var paramList = MakeParameters(loadedData.ColumnTypes, loadedData.ColumnNames);
            sb.AppendIndentedLine($"public {loadedData.SheetName}({paramList})", 2);
            sb.AppendIndentedLine("{", 2);
            for (var i = 0; i < loadedData.ColumnNames.Length; i++)
            {
                if (NotUsingColumns.Contains(loadedData.ColumnTypes[i])) continue;

                var columnName = loadedData.ColumnNames[i];
                sb.AppendIndentedLine($"{columnName} = {columnName.FirstCharacterToLower()};", 3);
            }

            sb.AppendIndentedLine("}", 2);
            sb.AppendIndentedLine("}", 1);
        }

        sb.AppendLine("}");
        return sb;
    }

    private static StringBuilder MakeDataGetter(List<SheetInfo> loadedDataList)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using Generated;");
        sb.AppendLine("using System;");
        sb.AppendLine("using Cysharp.Threading.Tasks;");
        sb.AppendLine("using UnityEditor;");
        sb.AppendLine();
        sb.AppendLine("public partial class GameDataManager : Singleton<GameDataManager>");
        sb.AppendLine("{");
        sb.AppendIndentedLine("private void LoadGameData()", 1);
        sb.AppendIndentedLine("{", 1);
        foreach (var loadedData in loadedDataList)
        {
            sb.AppendIndentedLine($"Load{loadedData.SheetName}().Forget();", 2);
        }

        sb.AppendIndentedLine("}", 1);
        sb.AppendLine();
        foreach (var loadedData in loadedDataList)
        {
            sb.AppendIndentedLine($"public List<{loadedData.SheetName}> {loadedData.SheetName}List" + " { get; private set; }", 1);
            sb.AppendLine();
            sb.AppendIndentedLine($"private async UniTask Load{loadedData.SheetName}()", 1);
            sb.AppendIndentedLine("{", 1);
            sb.AppendIndentedLine($"var loadedData = await CodeGenerator.GetGoogleSheetData(\"{loadedData.SheetName}\");", 2);
            sb.AppendLine();
            sb.AppendIndentedLine($"{loadedData.SheetName}List = new List<{loadedData.SheetName}>();", 2);
            sb.AppendIndentedLine(@"var rows = loadedData.Split('\n', StringSplitOptions.RemoveEmptyEntries);", 2);
            sb.AppendIndentedLine("for (var i = 2; i < rows.Length; i++)", 2);
            sb.AppendIndentedLine("{", 2);
            sb.AppendIndentedLine(@"var columns = rows[i].TrimEnd('\r').Split('\t');", 3);

            var parsedParams = MakeParsedParameters(loadedData.ColumnTypes, loadedData.ColumnNames);
            sb.AppendIndentedLine($"{loadedData.SheetName}List.Add(new {loadedData.SheetName}({parsedParams}));", 3);
            sb.AppendIndentedLine("}", 2);
            sb.AppendIndentedLine("}", 1);
        }

        sb.AppendLine("}");
        return sb;
    }

    private static StringBuilder MakeParsedParameters(IReadOnlyList<string> types, IReadOnlyList<string> names)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < types.Count; i++)
        {
            if (NotUsingColumns.Contains(types[i])) continue;

            var columnType = StringToTypes[types[i]];
            if (columnType == TypeEnum)
            {
                var enumParse = StringToParse[columnType].Replace("@", names[i]);
                sb.Append($"{enumParse}(columns[{i}])");
            }
            else if (columnType == TypeInt)
            {
                sb.Append($"{StringToParse[columnType]}(columns[{i}])");
            }
            else
            {
                sb.Append($"columns[{i}]");
            }

            if (i != types.Count - 1)
            {
                sb.Append(", ");
            }
        }

        return sb;
    }

    private static StringBuilder MakeParameters(IReadOnlyList<string> types, IReadOnlyList<string> names)
    {
        var paramList = new StringBuilder();
        for (var i = 0; i < names.Count; i++)
        {
            if (NotUsingColumns.Contains(types[i])) continue;

            var columnName = names[i];
            if (types[i].ToLower() == TypeEnum)
            {
                paramList.Append($"{columnName} {columnName.FirstCharacterToLower()}");
            }
            else
            {
                var columnType = StringToTypes[types[i]];
                paramList.Append($"{columnType} {columnName.FirstCharacterToLower()}");
            }


            if (i != names.Count - 1)
            {
                paramList.Append(", ");
            }
        }

        return paramList;
    }
}