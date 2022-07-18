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
        private int _currentURLIndex;

        private Timer _timer;
        private readonly int _rotationInterval;
        private readonly Size _savedSize;
        private readonly Point _savedLocation;
        private readonly ScreenInformation _screen;

        public ScreenSaverForm(ScreenInformation screen)
        {
            _screen = screen;
            _currentURLIndex = 0;

            _rotationInterval = screen.RotationInterval;

            _savedSize = new Size(screen.Bounds.Width, screen.Bounds.Height);
            _savedLocation = new Point(screen.Bounds.Left, screen.Bounds.Top);

            //Cursor.Hide();
            InitializeComponent();

            // Manually change size and location, since the `InitializeComponent` code tends to get auto replaced by the Designer
            this.SuspendLayout();
            this._webBrowser.Size = _savedSize;
            this._webBrowser.Location = _savedLocation;
            this.ClientSize = _savedSize;
            this.Location = _savedLocation;
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

                _timer = new Timer();
                _timer.Interval = _rotationInterval * 1000;
                _timer.Tick += (s, ee) => RotateSite();
                _timer.Start();

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
            if (_currentURLIndex >= urls.Count)
            {
                _currentURLIndex = 0;
            }

            BrowseTo(urls[_currentURLIndex]);
            _currentURLIndex++;
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

