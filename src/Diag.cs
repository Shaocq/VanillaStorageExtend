using System;
using System.IO;
using System.Reflection;

namespace VanillaStorageExtend
{
    /// <summary>
    /// 轻量级文件日志。
    ///
    /// 本 mod 的 Debug.Log 会被 Ducky.Logging 拦截，游戏自带的 Player.log /
    /// latest.log 里看不到，排查 UI 注入问题时直接写文件最可靠。
    /// 日志写在 mod DLL 同目录的 vse_debug.log，写失败时静默降级。
    /// </summary>
    internal static class Diag
    {
        private static readonly object Gate = new object();
        private static string logPath;
        private static bool disabled;

        internal static void Log(string message)
        {
            if (disabled)
            {
                return;
            }

            try
            {
                if (logPath == null)
                {
                    string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    if (string.IsNullOrEmpty(folder))
                    {
                        folder = Path.GetTempPath();
                    }
                    logPath = Path.Combine(folder, "vse_debug.log");
                }

                lock (Gate)
                {
                    File.AppendAllText(
                        logPath,
                        "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + message + Environment.NewLine);
                }
            }
            catch
            {
                // 日志失败绝不能影响游戏
                disabled = true;
            }
        }
    }
}
