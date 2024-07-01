/*
 * This software is provided under the MIT License.
 * 
 * Original Author: Jost Schienke
 * 
 * Disclaimer: This file has been modified from its original version.
 * The original author is not responsible for any changes made to the code.
 */

using UnityEditor;
using UnityEngine;
using System;
using System.IO;

[InitializeOnLoad]
public class EditorTimeTracker : EditorWindow
{
    private static DateTime editorStartTime;
    private static TimeSpan totalEditorTime;
    private static bool isTracking = false;
    private static string logFilePath = "EditorTimeLog.json";
    private static string editorStartTimeKey = "EditorTimeTracker_StartTime";
    private static bool isInitialized = false;

    static EditorTimeTracker()
    {
        LoadEditorTime();
        EditorApplication.update += Initialize;
        EditorApplication.quitting += OnEditorQuit;

        Debug.Log("EditorTimeTracker initialized.");
    }

    private static void Initialize()
    {
        if (isInitialized) return;

        if (EditorPrefs.HasKey(editorStartTimeKey))
        {
            editorStartTime = DateTime.Parse(EditorPrefs.GetString(editorStartTimeKey));
        }
        else
        {
            editorStartTime = DateTime.Now;
            EditorPrefs.SetString(editorStartTimeKey, editorStartTime.ToString());
        }

        isTracking = true;
        isInitialized = true;

        Debug.Log("EditorTimeTracker session started at: " + editorStartTime);
    }

    [MenuItem("Window/Editor Time Tracker")]
    public static void ShowWindow()
    {
        GetWindow<EditorTimeTracker>("Editor Time Tracker");
    }

    private void OnEnable()
    {
        EditorApplication.quitting += OnEditorQuit;
        EditorApplication.update += UpdateSessionTime;
    }

    private void OnDisable()
    {
        EditorApplication.quitting -= OnEditorQuit;
        EditorApplication.update -= UpdateSessionTime;
    }

    private void OnGUI()
    {
        GUILayout.Label("Editor Time Tracker", EditorStyles.boldLabel);
        GUILayout.Space(5);
        GUILayout.Label("Total Time: " + FormatTimeSpan(totalEditorTime));
        GUILayout.Space(5);
        GUILayout.Label("Current Session Time: " + FormatTimeSpan(DateTime.Now - editorStartTime));

        GUILayout.Space(10);

        if (GUILayout.Button("Reload Timer"))
        {
            Repaint();
        }
    }

    private static void OnEditorQuit()
    {
        totalEditorTime += DateTime.Now - editorStartTime;

        SaveEditorTime();

        EditorPrefs.DeleteKey(editorStartTimeKey);
        Debug.Log("EditorTimeTracker session ended. Total session time: " + totalEditorTime);
    }

    private static void SaveEditorTime()
    {
        string json = JsonUtility.ToJson(new TimeData { TotalSessionTime = totalEditorTime.TotalSeconds });
        File.WriteAllText(logFilePath, json);
    }

    private static void LoadEditorTime()
    {
        if (File.Exists(logFilePath))
        {
            string json = File.ReadAllText(logFilePath);
            TimeData data = JsonUtility.FromJson<TimeData>(json);
            totalEditorTime = TimeSpan.FromSeconds(data.TotalSessionTime);
        }
        else
        {
            totalEditorTime = TimeSpan.Zero;
        }
    }

    private static void UpdateSessionTime()
    {
        if (isTracking)
        {
            EditorPrefs.SetString(editorStartTimeKey, editorStartTime.ToString());
        }
    }

    private static string FormatTimeSpan(TimeSpan timeSpan)
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2}",
                             (int)timeSpan.TotalHours,
                             timeSpan.Minutes,
                             timeSpan.Seconds);
    }

    [Serializable]
    private class TimeData
    {
        public double TotalSessionTime;
    }
}
