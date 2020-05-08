using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftLiu.Analytics
{
    public class AnalyticsLogger
    {
        static bool Enabled = true;

        enum LogType
        {
            Default,
            Warning,
            Error,
        };

        public static void Log(string log)
        {
            LogInternal(log, LogType.Default);
        }

        public static void LogWarning(string log)
        {
            LogInternal(log, LogType.Warning);
        }

        public static void LogError(string log)
        {
            LogInternal(log, LogType.Error);
        }

        private static string Bolder(string input)
        {
            return "<b>" + input + "</b>";
        }

        private static string Italic(string input)
        {
            return "<i>" + input + "</i>";
        }

        private static string Coloured(string input, string colour)
        {
            return "<color=" + colour + ">" + input + "</color>";
        }

        private static void LogInternal(string log, LogType type)
        {
            if (!Enabled)
            {
                return;
            }

            switch (type)
            {
                case LogType.Default:
                    Debug.Log(Italic(Coloured(log, "lightblue")));
                    break;
                case LogType.Warning:
                    Debug.LogWarning(Bolder(Italic(Coloured(log, "orange"))));
                    break;
                case LogType.Error:
                    Debug.LogError(Bolder(Coloured(log, "red")));
                    break;
                default:
                    Debug.Log(log);
                    break;
            };
        }
    }
}
