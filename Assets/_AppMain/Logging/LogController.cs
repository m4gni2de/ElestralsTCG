using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Logging
{
    #region LogType
    public enum LogSeverity
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
        public LogSeverity logType { get; set; }
        public string[] Lines { get; set; }


        LogEntry(LogSeverity log, string msg)
        {
            When = DateTime.Now;
            logType = log;
            DoLines(msg);

        }

        LogEntry(LogSeverity log, string[] msg)
        {
            When = DateTime.Now;
            logType = log;
            DoLines(msg);
        }

        LogEntry(string msg)
        {
            When = DateTime.Now;
            Lines = new string[1];
            Lines[0] = msg;
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

        public static LogEntry Log(string msg, LogSeverity logType)
        {
            return new LogEntry(logType, msg);
        }

        public static LogEntry LogSimple(string msg)
        {
            return new LogEntry(msg);
        }

        public static LogEntry Message(string msg)
        {
            return new LogEntry(LogSeverity.Message, msg);

        }
        public static LogEntry Warning(string msg)
        {
            return new LogEntry(LogSeverity.Warning, msg);

        }
        public static LogEntry Fatal(string msg)
        {
            return new LogEntry(LogSeverity.Fatal, msg);

        }
        public static LogEntry Error(string msg)
        {
            return new LogEntry(LogSeverity.Error, msg);

        }
    }
    #endregion

    public class LogController
    {
        #region Properties
        private static string LogPath
        {
            get
            {
                return GetLogPath();
            }
        }
        private static string GetLogPath()
        {
#if UNITY_EDITOR
            return "Assets/Logging";
#else
            return $"{Application.persistentDataPath}/Logging";
#endif
        }
        
        public static string FileName
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
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
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

        protected static LogEntry CreateLog(string msg, LogSeverity logType)
        {
            return LogEntry.Log(msg, logType);
        }
        protected static LogEntry WriteLogSimple(string msg)
        {
            return LogEntry.LogSimple(msg);
        }
        public static void Log(string msg)
        {
            WriteLog(CreateLog(msg, LogSeverity.Message));
        }
        public static void LogSimple(string msg)
        {
            WriteLog(WriteLogSimple(msg));
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
            WriteLog(CreateLog(msg, LogSeverity.Warning));
        }
        public static void Error(string msg)
        {
            WriteLog(CreateLog(msg, LogSeverity.Error));
        }
#region Fatal Error
        public static bool Fatal(string msg)
        {
            WriteLog(CreateLog(msg, LogSeverity.Fatal));
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

