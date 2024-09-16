using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceView.Model
{
    /// <summary>
    /// Класс для логирования вызовов методов
    /// </summary>
    public static class MethodCallLogger
    {
        private static readonly string LogFilePath = "method_calls.log";
        private static readonly Dictionary<string, int> CallCounts = new Dictionary<string, int>();

        static MethodCallLogger()
        {
            LoadCallCounts();
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }

        public static void LogMethodCall(string methodName)
        {
            if (CallCounts.ContainsKey(methodName))
            {
                CallCounts[methodName]++;
            }
            else
            {
                CallCounts[methodName] = 1;
            }
        }

        private static void LoadCallCounts()
        {
            if (File.Exists(LogFilePath))
            {
                string[] lines = File.ReadAllLines(LogFilePath);
                foreach (var line in lines)
                {
                    var parts = line.Split('=');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int count))
                    {
                        CallCounts[parts[0]] = count;
                    }
                }
            }
        }

        private static void SaveCallCounts()
        {
            using (StreamWriter writer = new StreamWriter(LogFilePath, false))
            {
                foreach (var entry in CallCounts)
                {
                    writer.WriteLine($"{entry.Key}={entry.Value}");
                }
            }
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            SaveCallCounts();
        }
    }
}