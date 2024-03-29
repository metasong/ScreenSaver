﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Metaseed.WebPageScreenSaver.Configuration.Model
{
    internal static class Preferences
    {
        private const string CloseOnMouseMovementName = "CloseOnMouseMovement";
        private const string CloseOnEscKeyName = "CloseOnEscKey";
        private const string MultiScreenModeName = "MultiScreenMode";

        private struct SettingDefaultValue
        {
            public const int RotationInterval = 30;
            public const bool Shuffle = false;
        }

        private const string KeyWebPageScreenSaver = @"Software\Metaseed.WebPageScreenSaver";

        private static RegistryKey RootKey => Registry.CurrentUser.GetOrCreateSubKey(KeyWebPageScreenSaver);

        public static int ScreenCount => Screen.AllScreens.Length;

        public static MultiScreenMode MultiScreen
        {
            get => Enum.Parse<MultiScreenMode>(RootKey.GetOrCreateValue(MultiScreenModeName, MultiScreenMode.Mirror /* default */));
            set => RootKey.SetValue(MultiScreenModeName, value);
        }

        public static bool CloseOnMouseMovement
        {
            get => bool.Parse(RootKey.GetOrCreateValue(CloseOnMouseMovementName, true /* default */));
            set => RootKey.SetValue(CloseOnMouseMovementName, value);
        }

        public static bool CloseOnEscKey
        {
            get => bool.Parse(RootKey.GetOrCreateValue(CloseOnEscKeyName, true /* default */));
            set => RootKey.SetValue(CloseOnEscKeyName, value);
        }

        public static Dictionary<int, ScreenInformation> Screens
        {
            get
            {
                var screens = new Dictionary<int, ScreenInformation>();

                MultiScreenMode multiScreenMode = MultiScreen;

                for (int screenNumber = 0; screenNumber < ScreenCount; screenNumber++)
                {
                    int screen = screenNumber;
                    if (multiScreenMode != MultiScreenMode.Separate) screen = 0;

                    string screenKeyName = $"Display{screen + 1}"; // To match tab name
                    RegistryKey subKey = RootKey.GetOrCreateSubKey(screenKeyName);
                    var info = new ScreenInformation(subKey, screenNumber, multiScreenMode);
                    screens.Add(screenNumber, info);
                }

                return screens;
            }
        }
    }
}