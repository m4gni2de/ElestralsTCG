using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Logging
{
    #region LogType
    public enum LogType
    {

        Message = 0,
        Fatal = 1,
        Error = 2,
        Warning = 3,
    }
    #endregion

    #region LogEntry
    public class LogEntry
    {
        public DateTime When { get; set; }
        public LogType logType { get; set; }
        public string[] Lines { get; set; }


        LogEntry(LogType log, string msg)
        {
            When = DateTime.Now;
            logType = log;
            DoLines(msg);

        }

        LogEntry(LogType log, string[] msg)
        {
            When = DateTime.Now;
            logType = log;
            DoLines(msg);
        }

        protected void DoLines(string msg)
        {
            Lines = new string[3];
            Lines[0] = msg;
            Lines[1] = logType.ToString();
            Lines[2] = When.ToLongDateString();
        }

        protected void DoLines(string[] msg)
        {
            int lineCount = 2 + msg.Length;
            Lines = new string[lineCount];
            for (int i = 0; i < msg.Length; i++)
            {
                Lines[i] = msg[i];
            }
            Lines[lineCount - 2] = logType.ToString();
            Lines[lineCount - 1] = When.ToLongDateString();
        }

        public static LogEntry Log(string msg, LogType logType)
        {
            return new LogEntry(logType, msg);
        }

        public static LogEntry Message(string msg)
        {
            return new LogEntry(LogType.Message, msg);

        }
        public static LogEntry Warning(string msg)
        {
            return new LogEntry(LogType.Warning, msg);

        }
        public static LogEntry Fatal(string msg)
        {
            return new LogEntry(LogType.Fatal, msg);

        }
        public static LogEntry Error(string msg)
        {
            return new LogEntry(LogType.Error, msg);

        }
    }
    #endregion

    public class LogController
    {
        #region Properties
#if UNITY_EDITOR
        private static readonly string LogPath = "Assets/Logging";
#else
private static readonly string LogPath = $"{Application.persistentDataPath}/Logging";
#endif
        private static string FileName
        {
            get
            {

                string day = DateTime.Now.Day.ToString();
                string month = DateTime.Now.Month.ToString();
                string year = DateTime.Now.Year.ToString();
                string prefix = $"{day}_{month}_{year}";
                string path = prefix + "_log.txt";
                return path;
            }
        }

        private static string FilePath
        {
            get { return LogPath + "/" + FileName; }

        }

#endregion

#region Writing
        public static void CheckFile()
        {

            if (!File.Exists(FilePath))
            {
                using (StreamWriter sw = File.CreateText(FilePath))
                {
                    sw.WriteLine(DateTime.Now + " - Log Created.");
                }

            }
        }

        private static void WriteLog(LogEntry obj)
        {
            CheckFile();
            using (StreamWriter sw = File.AppendText(FilePath))
            {

                foreach (string item in obj.Lines)
                {

                    sw.WriteLine(item);
                }

            }

        }
#endregion

        protected static LogEntry CreateLog(string msg, LogType logType)
        {
            return LogEntry.Log(msg, logType);
        }
        public static void Log(string msg)
        {
            WriteLog(CreateLog(msg, LogType.Message));
        }
        public static void Log(string[] msg)
        {
            for (int i = 0; i < msg.Length; i++)
            {
                Log(msg[i]);
            }
        }
        public static void Warning(string msg)
        {
            WriteLog(CreateLog(msg, LogType.Warning));
        }
        public static void Error(string msg)
        {
            WriteLog(CreateLog(msg, LogType.Error));
        }
#region Fatal Error
        public static bool Fatal(string msg)
        {
            WriteLog(CreateLog(msg, LogType.Fatal));
            throw new Exception($"{FatalMessage} {msg}");
        }

        private static string FatalMessage
        {
            get
            {
                return "FATAL ERROR!";
            }
        }
#endregion
    }
}

