using System;
using System.IO;

namespace TestTaskParserWPF
{
    internal class Logger
    {
        /// <summary>
        /// Logs message to log file and log richtextbox on the mainwindow
        /// </summary>
        /// <param name="logMsg">Message needed to be written to log windows and txt file</param>
        internal static void LogMsg(string logMsg, string file = "log.txt")
        {
            logMsg = "\n" + DateTime.Now.ToString("G") + " " + logMsg;
            MainWindow.AppWindow.Dispatcher.Invoke(() =>
            {
                MainWindow.AppWindow.RichTextBoxLog.AppendText(logMsg);
                MainWindow.AppWindow.RichTextBoxLog.ScrollToEnd();
                File.AppendAllText(file, logMsg);
            });
        }
    }
}