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
                args.Length == 0 ||
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


            //_globalKeyboardHook = new GlobalKeyboardHook(new Keys[] { Keys.A, Keys.B });
            // Hooks into all keys.
            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed += (sender, e) =>
            {
                if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
                {
                    // Now you can access both, the key and virtual code
                    //Keys loggedKey = e.KeyboardData.Key;
                    //int loggedVkCode = e.KeyboardData.VirtualCode;
                    if (e.KeyboardData.Key == Keys.Escape)
                        Application.Exit();
                    else if (e.KeyboardData.Key == Keys.C)
                    {
                        if (_preferencesForm == null)
                        {
                            _preferencesForm = new PreferencesForm();
                            _preferencesForm.Closed += ((o, args) => _preferencesForm = null);
                            _preferencesForm.ShowDialog();
                        }
                    }
                }

            };

            if (Preferences.CloseOnMouseMovement)
            {
                MouseHook.Start();
                MouseHook.MouseAction += (o, e) => { Application.Exit(); };
            }

            Application.Run(new MultiFormContext(forms));
        }

        private static PreferencesForm? _preferencesForm;
        private static GlobalKeyboardHook _globalKeyboardHook;
    }
}
