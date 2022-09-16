using SFB;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class Encrypt_Decrypt : MonoBehaviour
{
    public TMP_Text EquipmentName;
    public TMP_Text StartTime;
    public TMP_Text EndTime;
    public TMP_Text ChannelNumber;
    public TMP_Text samplingRate;
    public TMP_Text TotalSample;
    public TMP_Text EEGData;

    private string EquipmentNameString;
    private string StartTimeString;
    private string EndTimeString;
    private string ChannelNumberString;
    private string samplingRateString;
    private string TotalSampleString;
    private string EEGDataString = "";
    private PED textData;
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
                Debug.Log(textData.EEGData[0]);
                Debug.Log(textData.EEGData[1]);
                Debug.Log(textData.EEGData[2]);
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
        });
    }
}
