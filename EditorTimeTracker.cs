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
using System.Collections.Generic;

[InitializeOnLoad]
public class EditorTimeTracker : EditorWindow
{
    private static DateTime editorStartTime;
    private static TimeSpan currentSessionTime;
    private static TimeSpan totalEditorTime;
    private static bool isTracking = false;
    private static string logFilePath = "EditorTimeLog.json";
    private static string editorStartTimeKey = "EditorTimeTracker_StartTime";
    private static bool isInitialized = false;

    private static List<DailyRecord> dailyRecords = new List<DailyRecord>();

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
            EditorPrefs.SetString(editorStartTimeKey, editorStartTime.ToString("o"));
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
        GUILayout.Label("Current Session Time: " + FormatTimeSpan(DateTime.Now - editorStartTime));
        GUILayout.Label("Total Editor Time: " + FormatTimeSpan(totalEditorTime));
        GUILayout.Space(10);

        if (GUILayout.Button("Reload Timer"))
        {
            Repaint();
        }

        GUILayout.Space(10);

        GUILayout.Label("Daily Log:", EditorStyles.boldLabel);
        foreach (var record in dailyRecords)
        {
            GUILayout.Label(record.Date + ": " + FormatTimeSpan(TimeSpan.FromSeconds(record.SessionTime)));
        }
    }

    private static void OnEditorQuit()
    {
        currentSessionTime = DateTime.Now - editorStartTime;
        totalEditorTime += currentSessionTime;

        var today = DateTime.Today.Date.ToString("dd-MM-yy");
        var todayRecord = dailyRecords.Find(record => record.Date == today);

        if (todayRecord != null)
        {
            todayRecord.SessionTime += currentSessionTime.TotalSeconds;
        }
        else
        {
            dailyRecords.Add(new DailyRecord { Date = today, SessionTime = currentSessionTime.TotalSeconds });
        }

        SaveEditorTime();

        // Reset editorStartTime to current time
        editorStartTime = DateTime.Now;

        EditorPrefs.DeleteKey(editorStartTimeKey);
        Debug.Log("EditorTimeTracker session ended. Current session time: " + currentSessionTime);
    }

    private static void SaveEditorTime()
    {
        var timeData = new TimeData
        {
            TotalEditorTime = totalEditorTime.TotalSeconds,
            DailyRecords = dailyRecords
        };
        string json = JsonUtility.ToJson(timeData);
        File.WriteAllText(logFilePath, json);
    }

    private static void LoadEditorTime()
    {
        if (File.Exists(logFilePath))
        {
            string json = File.ReadAllText(logFilePath);
            TimeData data = JsonUtility.FromJson<TimeData>(json);
            totalEditorTime = TimeSpan.FromSeconds(data.TotalEditorTime);
            dailyRecords = data.DailyRecords ?? new List<DailyRecord>();
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
            EditorPrefs.SetString(editorStartTimeKey, editorStartTime.ToString("o"));
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
        public double TotalEditorTime;
        public List<DailyRecord> DailyRecords;
    }

    [Serializable]
    private class DailyRecord
    {
        public string Date;
        public double SessionTime;
    }
}
