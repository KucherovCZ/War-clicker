using Entities;
using System;
using UnityEngine;

public class Logger
{
    public static Database DB { get; set; }

    public static void Log(DbLog log)
    {
        switch (log.Level)
        {
            case LogLevel.INFO:
                Debug.Log(log.Message);
                break;
            case LogLevel.WARNING:
                Debug.LogWarning(log.Message);
                break;
            case LogLevel.ERROR:
                Debug.LogError($"{log.Message} \nException: {log.Exception}");
                break;
        }

        DB.SaveLog(log);
    }

    public static void Log(LogLevel level, string message, string exception)
    {
        Log(new DbLog(level, message, exception));
    }
}
