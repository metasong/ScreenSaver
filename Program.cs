using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace WebPageScreensaver
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (Process.GetCurrentProcess().MainModule is not ProcessModule)
            {
                throw new NullReferenceException("Current process main module is null.");
            }

            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2); // Helps respect the specified window sizes
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true); // Prevents seeing tiny unexpected fonts

            if (args.Length > 0 && args[0].ToLower().Contains("/p"))
                return;

            if (
                //args.Length == 0 || 
                args.Length > 0 && args[0].ToLower().Contains("/c"))
            {
                ShowPreferences();
            }
            else
            {
                ShowScreenSaver();
            }
        }

        /// <summary>
        /// Show the screensaver preferences window.
        /// </summary>
        private static void ShowPreferences()
        {
            Application.Run(new PreferencesForm());
        }

        /// <summary>
        /// Shows the screensaver form in all the screens.
        /// </summary>
        private static void ShowScreenSaver()
        {
            var forms = new List<Form>();

            foreach ((int _, ScreenInformation info) in Preferences.Screens)
            {
                var form = new ScreensaverForm(info);
                forms.Add(form);
            }

            Application.Run(new MultiFormContext(forms));
        }
        //test
    }
}
