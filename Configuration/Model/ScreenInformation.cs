﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Metaseed.WebPageScreenSaver.Configuration.Model
{
    internal class ScreenInformation
    {
        private const string URLsName = "URLs";
        private const string RotationIntervalName = "IntervalRotation";
        private const string ShuffleName = "Shuffle";

        public ScreenInformation(RegistryKey rootKey, int screenNumber, MultiScreenMode multiScreenMode)
        {
            RootKey = rootKey;
            Bounds = multiScreenMode switch
            {
                MultiScreenMode.Span => FindEnclosingRectangle(),
                MultiScreenMode.Mirror or MultiScreenMode.Separate => Screen.AllScreens[screenNumber].Bounds,
                _ => throw new IndexOutOfRangeException("Unrecognized MultiScreenMode value.")
            };
        }

        public RegistryKey RootKey { get; private set; }

        public Rectangle Bounds { get; set; }

        public IEnumerable<(bool, string)> URLs
        {
            get => RootKey
                    .GetOrCreateValue(URLsName, "https://metaseed.github.io/pwsh/matrixRain<https://metaseed.github.io/pwsh/fireworks" /* default */)
                    .Split('<', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(u =>
                    {
                        var parts = u.Split('>');
                        var isChecked = true;
                        var url = u;
                        if (parts.Length == 2)
                        {
                            isChecked = bool.Parse(parts[0]);
                            url = parts[1];
                        }

                        return (isChecked, url);
                    });
            set
            {
                var v = value.Select((i) => $"{i.Item1}>{i.Item2}");
                RootKey.SetValue(URLsName, string.Join('<', v));
            }
        }

        public int RotationInterval
        {
            get => int.Parse(RootKey.GetOrCreateValue(RotationIntervalName, 10 /* default */));
            set => RootKey.SetValue(RotationIntervalName, value);
        }

        public bool Shuffle
        {
            get => bool.Parse(RootKey.GetOrCreateValue(ShuffleName, true /* default */));
            set => RootKey.SetValue(ShuffleName, value);
        }

        private static Rectangle FindEnclosingRectangle()
        {
            IEnumerable<Rectangle> bounds = Screen.AllScreens.Select(r => r.Bounds);
            return Rectangle.FromLTRB(
                bounds.Min(r => r.Left),
                bounds.Min(r => r.Top),
                bounds.Max(r => r.Right),
                bounds.Max(r => r.Bottom));
        }
    }
}
