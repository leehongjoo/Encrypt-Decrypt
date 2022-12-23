using SFB;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Text;

public class Encrypt_Decrypt : MonoBehaviour
{
    public TMP_Text EquipmentName;
    public TMP_Text StartTime;
    public TMP_Text EndTime;
    public TMP_Text ChannelNumber;
    public TMP_Text samplingRate;
    public TMP_Text TotalSample;
    public TMP_Text StatusText;

    private string EquipmentNameString;
    private string StartTimeString;
    private string EndTimeString;
    private string ChannelNumberString;
    private string samplingRateString;
    private string TotalSampleString;
    private string EEGDataString = "";
    private PED textData;

    private string plainString;

    private string bfnString = "";
    private string encryptString;

    public void TxtFileLoad()
    {
#if UNITY_STANDALONE_WIN
        var bp = new BrowserProperties();
        bp.filter = "txt files (*.txt)|*.txt";
        bp.filterIndex = 0;
        bp.initialDir = @"C:\cmd";
        EEGDataString = "";
        new fileBrowser().OpenFileBrowser(bp, result =>
        {
            FileInfo fileInfo = new FileInfo(result);
            string line = "";
            if (fileInfo.Exists)
            {
                StreamReader reader = new StreamReader(result);
                EquipmentNameString = reader.ReadLine();
                StartTimeString = reader.ReadLine();
                EndTimeString = reader.ReadLine();
                reader.ReadLine();
                ChannelNumberString = reader.ReadLine();
                samplingRateString = reader.ReadLine();
                TotalSampleString = reader.ReadLine();
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    EEGDataString += "\n";
                    EEGDataString += line;
                }
                reader.Close();
            }
            else
            {
                // 에러 코드 
                line = "파일이 없습니다";
            }
        });
        StatusText.text = " Load 완료";
#endif
    }
    public void EncryptPEDFile()
    {
        PED ped = new PED(EquipmentNameString, StartTimeString, EndTimeString, ChannelNumberString, samplingRateString, 
            TotalSampleString, EEGDataString);
        //string saveFileName = "PanaxtosTextData";
        //string path = "C:/Users/KimKyoungHoon/Desktop/이이사/absc.abcd";
        var extensionList = new[]
        {
            new ExtensionFilter("PanaxtosEEGData", "ped"),
        };
        StandaloneFileBrowser.SaveFilePanelAsync("Save File", "", "", extensionList, (string path) =>
        {
            if (path == "")
                return;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(path));
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            CryptManager.SavePEDFile(ped, path);
            StatusText.text = " 암호화 & PED 파일 저장 완료";
        });        
    }
    public void PEDFileDecrypt()
    {
#if UNITY_STANDALONE_WIN
        var bp = new BrowserProperties();
        bp.filter = "ped files (*.ped)|*.ped";
        bp.filterIndex = 0;
        bp.initialDir = @"C:\cmd";
        textData = null;
        new fileBrowser().OpenFileBrowser(bp, result =>
        {
            FileInfo fileInfo = new FileInfo(result);
            if (fileInfo.Exists)
            {
                textData = CryptManager.LoadPEDFile(result);
                EquipmentName.text = textData.EquipmentName;
                StartTime.text = textData.StartTime;
                EndTime.text = textData.EndTime;
                ChannelNumber.text = textData.ChannelNumber;
                samplingRate.text = textData.SamplingRate;
                TotalSample.text = textData.TotalSample;
                StatusText.text = " PED 파일 Load 완료";
            }
        });
#endif
    }
    public void DecryptDataMakeTxtFile()
    {
        var extensionList = new[]
        {
            new ExtensionFilter("", "txt"),
        };
        StandaloneFileBrowser.SaveFilePanelAsync("Save File", "", "", extensionList, (string path) =>
        {
            if (path == "")
                return;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(path));
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fileStream, System.Text.Encoding.Unicode);
            writer.WriteLine(textData.EquipmentName);
            writer.WriteLine(textData.StartTime);
            writer.WriteLine(textData.EndTime);
            writer.WriteLine();
            writer.WriteLine(textData.ChannelNumber);
            writer.WriteLine(textData.SamplingRate);
            writer.WriteLine(textData.TotalSample);
            writer.WriteLine();
            writer.WriteLine(textData.EEGData);
            writer.Close();
            StatusText.text = " Decrypt 완료 & Txt 파일 Save 완료";
        });
    }

    public void bfnFileLoad()
    {
#if UNITY_STANDALONE_WIN
        var bp = new BrowserProperties();
        bp.filter = "bfn files (*.bfn)|*.bfn";
        bp.filterIndex = 0;
        bp.initialDir = @"C:\cmd";
        bfnString = "";
        new fileBrowser().OpenFileBrowser(bp, result =>
        {
            FileInfo fileInfo = new FileInfo(result);
            string line = "";
            if (fileInfo.Exists)
            {
                StreamReader reader = new StreamReader(result);
                while ((line = reader.ReadLine()) != null)
                {
                    bfnString += line;
                    bfnString += "\n";
                }
                bfnString = bfnString.Substring(0, bfnString.Length - 1);
                reader.Close();
                Debug.Log(bfnString);
            }
            else
            {
                line = "파일이 없습니다";
            }
        });
#endif
    }
    public void EncryptbfnFile()
    {
        encryptString = AESCrypto.AESEncrypt128(bfnString);
        Debug.Log(encryptString);
    }
    public void bfnFileDecrypt()
    {
        string result = AESCrypto.AESDecrypt128(encryptString);
        Debug.Log(result);
    }
    public void DecryptDataMakeTxtFile2()
    {

    }
    public void bqiFileLoad()
    {
#if UNITY_STANDALONE_WIN
        var bp = new BrowserProperties();
        bp.filter = "bqi mqi files (*.bqi)|*.bqi|(*.mqi)|*.mqi";
        bp.filterIndex = 0;
        bp.initialDir = @"C:\cmd";
        EEGDataString = "";
        new fileBrowser().OpenFileBrowser(bp, result =>
        {
            FileInfo fileInfo = new FileInfo(result);
            string line = "";
            if (fileInfo.Exists)
            {
                StreamReader reader = new StreamReader(result);
                line = reader.ReadLine();
                plainString = line;
                StatusText.text = " Load 완료";
                reader.Close();
            }
            else
            {
                // 에러 코드 
                line = "파일이 없습니다";
            }
        });
#endif
    }
    public void LoadDataTotxtFile()
    {
        var extensionList = new[]
        {
            new ExtensionFilter("EEGData", "txt"),
        };
        StandaloneFileBrowser.SaveFilePanelAsync("Save File", "", "", extensionList, (string path) =>
        {
            if (path == "")
                return;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(path));
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8);
            string data = AESCrypto.AESDecrypt128(plainString);
            sw.Write(data);
            sw.Close();
            StatusText.text = " txt로 변경 저장 완료";
        });
    }
    public void bqfFileLoad()
    {
#if UNITY_STANDALONE_WIN
        var bp = new BrowserProperties();
        bp.filter = "bqf files (*.bqf)|*.bqf";
        bp.filterIndex = 0;
        bp.initialDir = @"C:\cmd";
        EEGDataString = "";
        new fileBrowser().OpenFileBrowser(bp, result =>
        {
            FileInfo fileInfo = new FileInfo(result);
            string line = "";
            if (fileInfo.Exists)
            {
                StreamReader reader = new StreamReader(result);
                line = reader.ReadLine();
                plainString = line;
                StatusText.text = "bqf Load 완료";
                reader.Close();
            }
            else
            {
                // 에러 코드 
                line = "파일이 없습니다";
            }
        });
#endif
    }
    public void bqfLoadDataTotxtFile()
    {
        var extensionList = new[]
        {
            new ExtensionFilter("EEGData", "txt"),
        };
        StandaloneFileBrowser.SaveFilePanelAsync("Save File", "", "", extensionList, (string path) =>
        {
            if (path == "")
                return;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(path));
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8);
            string data = AESCrypto.AESDecrypt128(plainString);
            sw.Write(data);
            sw.Close();
            StatusText.text = " txt로 변경 저장 완료";
        });
    }
}
