using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface iLog 
{
    
}


public static class iLogExtensions
{
    public static void Log(this iLog log, params string[] lines)
    {
        if (lines == null || lines.Length == 0) { App.LogWarning("Log failed. Cannot log blank entries."); return; }
        string[] logLines = new string[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            logLines[i] = lines[i].Trim();
        }

        App.Log(logLines);
    }
}
