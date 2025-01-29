using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Architecture;

namespace IMGUI
{
    public class IMGUI_Console : MonoBehaviour
    {
        const int MAX_LOG_COUNT = 100;
        const float MAX_MSG_SECONDS = 5f;
        const float MSG_FADE_OUT_TIME = 0.2f;
        const float WIDTH_PCT = 0.4f;
        const float HEIGHT_PCT = 0.48f;
        const float MARGIN = 0.02f;

        public static IMGUI_Console Instance { get; private set; }
        public static PersistentBool ShouldDisplay = new PersistentBool("IMGUI_Console_ShouldDisplay", false);
        public static EnumArray<LogType, PersistentBool> DisplayFilter = new EnumArray<LogType, PersistentBool>();

        GUIStyle labelStyle;
        private IMGUI_Style.TextDescription textDescription = new IMGUI_Style.TextDescription(TextAnchor.LowerLeft, FontStyle.Normal, Color.white, 14);
        private static volatile LinkedList<LogEntry> logEntries = null;
        private static volatile object logLock = new object();
        private static GUIContent guiContent = null;
        private static StringBuilder strBuilder = new StringBuilder();


        private void OnEnable()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (logEntries == null)
            {
                logEntries = new LinkedList<LogEntry>();
                Application.logMessageReceived -= AddLog;
                Application.logMessageReceived += AddLog;
            }

            foreach((int index, var logType, var filter) in DisplayFilter.EnumerateIndicesEnumsAndValues())
            {
                DisplayFilter[index] = new PersistentBool($"IMGUI_Console_Filter_{logType}", true);
            }
        }

        private void OnDisable()
        {
            if (Instance == this) Instance = null;
        }

        private static void AddLog(string condition, string stackTrace, LogType type)
        {
            string msg;
            msg = condition;
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            {
                msg = $"<b>{msg}</b>";
                msg += "\nStackTrace: ";
                msg += stackTrace;
                msg += '\n';
            }

            lock (logLock)
            {
                logEntries.AddLast(new LogEntry()
                {
                    msg = msg,
                    type = type,
                    date = DateTime.Now,
                });

                if(logEntries.Count > MAX_LOG_COUNT)
                {
                    logEntries.RemoveFirst();
                }
            }
        }

        

        private void OnGUI()
        {
            if (!ShouldDisplay.Value) return;
            if(labelStyle == null) labelStyle = IMGUI_Style.GetTextStyle(textDescription);
            if(guiContent == null) guiContent = new GUIContent();

            IMGUI_Layout.InsideRect(WIDTH_PCT, HEIGHT_PCT, IMGUI_Layout.Anchor.BottomLeft, DisplayLogs, MARGIN, -MARGIN);
        }

        private void DisplayLogs(Rect rect)
        {
            GUIStyle labelStyle = IMGUI_Style.GetTextStyle(textDescription);

            strBuilder.Clear();
            DateTime now = DateTime.Now;
            lock (logLock)
            {
                var node = logEntries.First;
                var lastNode = logEntries.Last;

                while (node != null)
                {
                    var nextNode = node.Next;
                    LogEntry entry = node.Value;
                    if ((now - node.Value.date).TotalSeconds > MAX_MSG_SECONDS)
                    {
                        logEntries.Remove(node);
                    }
                    else if (DisplayFilter[entry.type].Value)
                    {
                        if (strBuilder.Length > 0) strBuilder.Append("\n");
                        Color color;
                        switch (entry.type)
                        {
                            default:
                            case LogType.Log: color = Color.white; break;
                            case LogType.Warning: color = Color.yellow; break;
                            case LogType.Assert:
                            case LogType.Exception:
                            case LogType.Error: color = new Color(1f, 0.3f, 0f, 1f); break;
                        }

                        if (node != lastNode)
                            color.a = 0.7f;

                        // FADE OUT CAUSES SEVERE SLOWDOWN FOR WHATEVER REASON
                        float elapsed = (float)(now - entry.date).TotalSeconds;
                        float fadeOut = 1f - Mathf.Clamp01((elapsed - (MAX_MSG_SECONDS - MSG_FADE_OUT_TIME)) / MSG_FADE_OUT_TIME);
                        color.a *= fadeOut;


                        strBuilder.Append($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>");
                        strBuilder.Append(entry.msg);
                        strBuilder.Append("</color>");
                    }

                    node = nextNode;
                }
            }

            guiContent.text = strBuilder.ToString();

            //TOO EXPENSIVE
            //if(logEntries.Count > 1 && labelStyle.CalcHeight(guiContent, rect.width) > rect.height)
            //{
            //    logEntries.RemoveFirst();
            //}

            GUI.Label(rect, guiContent, labelStyle);
        }

        private class LogEntry
        {
            public string msg;
            public LogType type;
            public DateTime date;
        }
    }
}
