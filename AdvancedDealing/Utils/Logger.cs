using MelonLoader;
using System;

namespace AdvancedDealing.Utils
{
    public static class Logger
    {
        public static void Msg(string msg)
        {
            Msg(null, msg);
        }

        public static void Msg(string prefix, string msg)
        {
            if (prefix != null) 
            { 
                msg = $"[{prefix}] {msg}"; 
            }

            MelonLogger.Msg(msg);
        }

        public static void Error(string msg) => Error(null, msg, null);

        public static void Error(string prefix, string msg) => Error(prefix, msg, null);

        public static void Error(string msg, Exception ex) => Error(null, msg, ex);

        public static void Error(string prefix, string msg, Exception ex)
        {
            if (prefix != null)
            {
                msg = $"[{prefix}] {msg}";
            }

            if (ex != null)
            {
                MelonLogger.Error(msg, ex);
            }
            else
            {
                MelonLogger.Error(msg);
            }
        }

        public static void Debug(string msg) => Debug(null, msg);

        public static void Debug(string prefix, string msg)
        {
            if (!ModConfig.Debug) return;

            if (prefix != null)
            {
                msg = $"[{prefix}] {msg}";
            }

            MelonLogger.Msg(ConsoleColor.Cyan, msg);
        }

        public static void Warning(string msg) => Warning(null, msg);

        public static void Warning(string prefix, string msg)
        {
            if (prefix != null)
            {
                msg = $"[{prefix}] {msg}";
            }

            MelonLogger.Warning(msg);
        }
    }
}
