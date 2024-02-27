using System;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

public enum LocalDataType
{
    SaveFlow,
}

public class LocalDataManager : Singleton<LocalDataManager>
{
    private const string PassW = "4100000000019148";

    public void SaveIntPlayerPrefs(LocalDataType key, int value)
    {
        Save(key.ToString(), value.ToString());
    }

    [CanBeNull]
    public int? GetIntPlayerPrefs(LocalDataType key)
    {
        var value = GetValue(key.ToString());
        if (string.IsNullOrEmpty(value)) return null;

        return int.Parse(value);
    }

    private static void Save(string key, string value)
    {
        PlayerPrefs.SetString(key, EncryptStringData(value));
    }

    private static string GetValue(string key)
    {
        return PlayerPrefs.HasKey(key) ? DecryptStringData(PlayerPrefs.GetString(key)) : string.Empty;
    }

    private static string EncryptStringData(string inputText) //암호화
    {
        var keyArray = Encoding.UTF8.GetBytes(PassW);
        var toEncryptArray = Encoding.UTF8.GetBytes(inputText);
        var rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        var cTransform = rDel.CreateEncryptor();
        var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    private static string DecryptStringData(string inputText) //복호화
    {
        var keyArray = Encoding.UTF8.GetBytes(PassW);
        var toDecryptArray = Convert.FromBase64String(inputText);
        var rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        var cTransform = rDel.CreateDecryptor();
        var resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);
        return Encoding.UTF8.GetString(resultArray);
    }
}