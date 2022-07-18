using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Metaseed.WebPageScreenSaver.Configuration.Model;

namespace Metaseed.WebPageScreenSaver
{
    internal partial class ScreenSaverForm : Form
    {
        private int _currentUrlIndex;

        private readonly ScreenInformation _screen;

        public ScreenSaverForm(ScreenInformation screen)
        {
            _screen = screen;

            InitializeComponent();

            // Manually set size and location on screen
            this.SuspendLayout();
            this.Location = screen.Bounds.Location;
            this.ClientSize = screen.Bounds.Size;
            this.ResumeLayout(false);

        }

        private async void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            await _webBrowser.EnsureCoreWebView2Async();

            var urls = _screen.URLs.ToList();
            if (urls.Any())
            {
                if (_screen.Shuffle)
                {
                    var random = new Random();
                    var n = urls.Count;
                    while (n-- > 1)
                    {
                        var k = random.Next(n);
                        (urls[k], urls[n]) = (urls[n], urls[k]);
                    }
                }

                timerUrlSwitch.Interval = _screen.RotationInterval * 1000;
                timerUrlSwitch.Tick += (s, ee) => RotateSite();
                timerUrlSwitch.Start();

                RotateSite();
            }
            else
            {
                _webBrowser.Visible = false;
            }

        }

        private void RotateSite()
        {
            var urls = _screen.URLs.ToList();
            if (_currentUrlIndex >= urls.Count)
            {
                _currentUrlIndex = 0;
            }

            BrowseTo(urls[_currentUrlIndex++]);
        }

        private void BrowseTo(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            try
            {
                Debug.WriteLine($"Navigating: {url}");
                _webBrowser.CoreWebView2.Navigate(url);
            }
            catch
            {
                // ignored
            }
        }

    }
}

