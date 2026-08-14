using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RobbyCopy
{
    public partial class App : Application
    {
        private static string _crashLogPath;
        private static bool _silent;

        protected override void OnStartup(StartupEventArgs e)
        {
            _silent = HasCommandLineFlag("run")
                   || HasCommandLineFlag("silent")
                   || HasCommandLineFlag("exit");

            InitCrashLog();

            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            base.OnStartup(e);
        }

        private static void InitCrashLog()
        {
            try
            {
                string folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "RobbyCopy");

                Directory.CreateDirectory(folder);
                _crashLogPath = Path.Combine(folder, "crash.log");
            }
            catch
            {
                _crashLogPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "RobbyCopy_crash.log");
            }
        }

        private static void App_DispatcherUnhandledException(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception);

            e.Handled = true;

            // Wenn das Hauptfenster nicht erfolgreich geladen wurde, beenden.
            if (Application.Current.MainWindow == null ||
                !Application.Current.MainWindow.IsLoaded)
            {
                Application.Current.Shutdown(1);
            }
        }

        private static void CurrentDomain_UnhandledException(
            object sender,
            UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception);
        }

        private static void TaskScheduler_UnobservedTaskException(
            object sender,
            UnobservedTaskExceptionEventArgs e)
        {
            HandleException(e.Exception);
            e.SetObserved();
        }

        private static void HandleException(Exception exception)
        {
            try
            {
                string text =
                    "====================" + Environment.NewLine +
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine +
                    exception?.ToString() + Environment.NewLine;

                File.AppendAllText(_crashLogPath, text);
            }
            catch
            {
            }

            if (!_silent)
            {
                try
                {
                    MessageBox.Show(
                        "RobbyCopy ist auf einen Fehler gestoßen." +
                        Environment.NewLine +
                        Environment.NewLine +
                        exception?.Message +
                        Environment.NewLine +
                        Environment.NewLine +
                        "Details wurden hier gespeichert:" +
                        Environment.NewLine +
                        (_crashLogPath ?? "%APPDATA%\\RobbyCopy\\crash.log"),
                        "RobbyCopy Fehler",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                catch
                {
                }
            }
        }

        private static bool HasCommandLineFlag(string name)
        {
            string[] args = Environment.GetCommandLineArgs();

            foreach (string arg in args)
            {
                if (arg.Equals("/" + name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}