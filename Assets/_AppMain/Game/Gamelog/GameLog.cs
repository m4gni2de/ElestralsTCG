using System.Collections;
using System.Collections.Generic;
using Gameplay.GameCommands;
using UnityEngine;
using Logging;
using System.IO;
using System;

namespace Gameplay
{
    [System.Serializable]
    public class GameLog
    {
        #region File Writing

#if UNITY_EDITOR
        private static readonly string LogPath = "Assets/Logging/GameLog";
#else
private static readonly string LogPath = $"{Application.persistentDataPath}/Logging";
#endif


        public void CheckFile()
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

        private void WriteToLog(LogEntry obj)
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
       

        private string _fileName = "";
        public string fileName
        {
            get
            {
                if (string.IsNullOrEmpty(_fileName))
                {
                    _fileName = LogController.FileName;
                }
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }

        private string FilePath
        {
            get { return LogPath + "/" + fileName + ".txt"; }

        }


        protected LogEntry CreateLog(string msg, LogSeverity logType)
        {
            return LogEntry.Log(msg, logType);
        }

        /// <summary>
        /// WriteLog adds the log to the file to be written to the system.
        /// </summary>
        /// <param name="msg"></param>
        public void WriteLog(string msg)
        {
            WriteToLog(CreateLog(msg, LogSeverity.Message));
        }
        #endregion
        public bool WritesFile = false;

        [SerializeField]
        private List<string> _logs = null;
        public List<string> logs
        {
            get
            {
                _logs ??= new List<string>();
                return _logs;
            }
        }

       
        GameLog(string firstLine)
        {
            AddLog(firstLine);
        }
        GameLog(string logTitle, bool isWriter)
        {
            WritesFile = isWriter;
            fileName = $"{logTitle}_{LogController.FileName}";
        }

        public static GameLog Create(string title, bool isWriter)
        {
            return new GameLog(title, isWriter);
        }

        /// <summary>
        /// AddLog adds the log to the log to be kept within the app
        /// </summary>
        /// <param name="msg"></param>
        public void AddLog(string msg)
        {
            logs.Add(msg);
            //CardActionData.FromData(msg);
            if (WritesFile)
            {
                WriteLog(msg);
            }
        }

        

        public void LogAction(CardAction ac)
        {
            CardActionData data = ac.ActionData;
            if (data != null)
            {
                AddLog(data.GetJson);
            }
        }
       
    }
}

