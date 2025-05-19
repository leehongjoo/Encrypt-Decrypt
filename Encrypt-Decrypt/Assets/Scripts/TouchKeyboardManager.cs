using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchKeyboardManager : MonoBehaviour
{
    [DllImport("user32")]
    static extern IntPtr FindWindow(String sClassName, String sAppName);

    [DllImport("user32")]
    static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

    private static Process _onScreenKeyboardProcess = null;

    public TMP_InputField inputfield1;
    public TMP_InputField inputfield2;
    public TMP_Text Text1;
    public TMP_Text Text2;

    void Start()
    {
        inputfield1.onValueChanged.AddListener(TextChange);
    } 
    void TextChange(string text)
    {
        Text1.text = inputfield1.text;
        Text2.text = text;
    }
    public void ShowTouchKeyboard()
    {
        if (_onScreenKeyboardProcess == null || _onScreenKeyboardProcess.HasExited)
            _onScreenKeyboardProcess = ExternalCall("OSK", null, false);
    }
    public void HideOnScreenKeyboard()
    {
        if (_onScreenKeyboardProcess != null && !_onScreenKeyboardProcess.HasExited)
            _onScreenKeyboardProcess.Kill();
    }
    private static Process ExternalCall(string filename, string arguments, bool hideWindow)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = filename;
        startInfo.Arguments = arguments;

        if(hideWindow)
        {
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
        }
        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();
        process.Refresh();

        return process;
    }

    
}
