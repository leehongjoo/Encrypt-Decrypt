using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class CryptManager : MonoBehaviour
{
    private static readonly string privateKey = "1718hy9dsf0jsdlfjds0pa9ids78ahgf81h32re";
    public static void SavePEDFile(PED ptd, string path)
    {
        string jsonString = PEDToJson(ptd);
        string encryptString = EncryptData(jsonString);
        WritePEDFile(encryptString, path);
    }
    private static void WritePEDFile(string jsonData, string path)
    {
        if (path == "")
            return;
        DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(path));
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }
        FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(bytes, 0, bytes.Length);
        fileStream.Close();
    }
    public static PED LoadPEDFile(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }
        string encryptData = ReadPEDFile(path);
        string decryptData = DecryptData(encryptData);
        PED ped = JsonToPED(decryptData);
        return ped;
    }
    private static string ReadPEDFile(string path)
    {
        if (path == "")
            return null;
        DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(path));
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        byte[] bytes = new byte[(int)fileStream.Length];
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        string jsonString = System.Text.Encoding.UTF8.GetString(bytes);
        return jsonString;
    }
    private static string PEDToJson(PED ped)
    {
        string jsonData = JsonUtility.ToJson(ped);
        return jsonData;
    }
    private static PED JsonToPED(string jsonData)
    {
        PED sd = JsonUtility.FromJson<PED>(jsonData);
        return sd;
    }
    private static string EncryptData(string data)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
        RijndaelManaged rm = MakeRijndaelManaged();
        ICryptoTransform ct = rm.CreateEncryptor();
        byte[] results = ct.TransformFinalBlock(bytes, 0, bytes.Length);
        return System.Convert.ToBase64String(results, 0, results.Length);

    }
    private static string DecryptData(string data)
    {
        byte[] bytes = System.Convert.FromBase64String(data);
        RijndaelManaged rm = MakeRijndaelManaged();
        ICryptoTransform ct = rm.CreateDecryptor();
        byte[] resultArray = ct.TransformFinalBlock(bytes, 0, bytes.Length);
        return System.Text.Encoding.UTF8.GetString(resultArray);
    }
    private static RijndaelManaged MakeRijndaelManaged()
    {
        byte[] keyArray = System.Text.Encoding.UTF8.GetBytes(privateKey);
        RijndaelManaged result = new RijndaelManaged();

        byte[] newKeysArray = new byte[16];
        System.Array.Copy(keyArray, 0, newKeysArray, 0, 16);
        result.Key = newKeysArray;
        result.Mode = CipherMode.ECB;
        result.Padding = PaddingMode.PKCS7;
        return result;
    }
}
[System.Serializable]
public class PED
{
    //PED = Panaxtos EEG Data
    public string EquipmentName;
    public string StartTime;
    public string EndTime;
    public string ChannelNumber;
    public string SamplingRate;
    public string TotalSample;
    public string EEGData;
    public PED(string EN, string startTime, string endTime, string channelNumber, string samplingrate, string totalsample, string data)
    {
        EquipmentName = EN;
        StartTime = startTime;
        EndTime = endTime;
        ChannelNumber = channelNumber;
        SamplingRate = samplingrate;
        TotalSample = totalsample;
        EEGData = data;
    }
}
