using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Metaseed.WebPageScreenSaver.Configuration.Model;
using Metaseed.WebPageScreenSaver.InputHook;

namespace Metaseed.WebPageScreenSaver
{
    static class Program
    {
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

            switch (args.Length)
            {
                case > 0 when args[0].ToLower().Contains("/p"):
                    return;
                case 0:
                case > 0 when args[0].ToLower().Contains("/c"):
                    ShowPreferences();
                    break;
                default:
                    ShowScreenSaverOnScreens();
                    break;
            }
        }

        private static void ShowPreferences()
        {
            Application.Run(new PreferencesForm());
        }

        private static void ShowScreenSaverOnScreens()
        {
            var forms = new List<Form>();

            foreach ((int _, ScreenInformation info) in Preferences.Screens)
            {
                var form = new ScreenSaverForm(info);
                forms.Add(form);
            }

            AddInputHook();

            Application.Run(new MultiFormAppContext(forms));
        }

        private static void AddInputHook()
        {
            //_globalKeyboardHook = new GlobalKeyboardHook(new Keys[] { Keys.A, Keys.B });
            // Hooks into all keys.
            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed += (sender, e) =>
            {
                if (e.KeyboardState != GlobalKeyboardHook.KeyboardState.KeyDown) return;

                switch (e.KeyboardData.Key)
                {
                    //Keys loggedKey = e.KeyboardData.Key;
                    //int loggedVkCode = e.KeyboardData.VirtualCode;
                    case Keys.Escape:
                        Application.Exit();
                        break;
                    case Keys.C:
                    {
                        if (_preferencesForm == null)
                        {
                            _preferencesForm = new PreferencesForm();
                            _preferencesForm.Closed += ((o, args) => _preferencesForm = null);
                            _preferencesForm.ShowDialog();
                        }

                        break;
                    }
                }
            };

            if (Preferences.CloseOnMouseMovement)
            {
                MouseHook.Start();
                MouseHook.MouseAction += (o, e) => { Application.Exit(); };
            }
        }

        private static PreferencesForm? _preferencesForm;
        private static GlobalKeyboardHook? _globalKeyboardHook;
    }
}
