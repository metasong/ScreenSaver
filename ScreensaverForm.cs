using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WebPageScreensaver
{
    internal partial class ScreensaverForm : Form
    {
        private int _currentURLIndex;

        private readonly Timer _timer;
        private readonly bool _closeOnMouseMovement;
        private readonly int _rotationInterval;
        private readonly bool _shuffle;
        private readonly List<string> _urls;
        private readonly Size _savedSize;
        private readonly Point _savedLocation;
        private GlobalUserEventHandler userEventHandler;

        public ScreensaverForm(ScreenInformation screen)
        {
            _currentURLIndex = 0;

            _closeOnMouseMovement = Preferences.CloseOnMouseMovement;
            _rotationInterval = screen.RotationInterval;
            _shuffle = screen.Shuffle;
            _urls = screen.URLs.ToList();

            _savedSize = new Size(screen.Bounds.Width, screen.Bounds.Height);
            _savedLocation = new Point(screen.Bounds.Left, screen.Bounds.Top);

            //Cursor.Hide();
            InitializeComponent();

            // Manually change size and location, since the `InitializeComponent` code tends to get autoreplaced by the Designer
            this.SuspendLayout();
            this._webBrowser.Size = _savedSize;
            this._webBrowser.Location = _savedLocation;
            this.ClientSize = _savedSize;
            this.Location = _savedLocation;
            this.ResumeLayout(false);

            _timer = new Timer();
            userEventHandler = new GlobalUserEventHandler();
            userEventHandler.Event += new GlobalUserEventHandler.UserEvent(HandleUserActivity);
            StartTime = DateTime.Now;
        }
        private DateTime StartTime;
        private void HandleUserActivity()
        {
            if (StartTime.AddMilliseconds(20) > DateTime.Now) return;
            if (_closeOnMouseMovement)
                Close();
            //if (prefsManager.CloseOnActivity)
            //{
            //    Close();
            //}
            //else
            //{
            //    closeButton.Visible = true;
            //    Cursor.Show();
            //    webView2.Enabled = true;
            //}
        }
        private Timer registerTimer;
        private async void ScreensaverForm_Load(object sender, EventArgs e)
        {
            if (_webBrowser == null)
            {
                throw new NullReferenceException("webBrowser should have been initialized by now.");
            }
            await _webBrowser.EnsureCoreWebView2Async();

            if (_urls.Any())
            {
                if (_shuffle)
                {
                    Random random = new Random();
                    int n = _urls.Count;
                    while (n > 1)
                    {
                        n--;
                        int k = random.Next(n + 1);
                        var value = _urls[k];
                        _urls[k] = _urls[n];
                        _urls[n] = value;
                    }
                }

                _timer.Interval = _rotationInterval * 1000;
                _timer.Tick += (s, ee) => RotateSite();
                _timer.Start();

                RotateSite(); // First call, second one will be done _rotationInterval seconds later by _timer
            }
            else
            {
                _webBrowser.Visible = false;
            }


            //void RegisterMouseMove(object? oo, EventArgs ee)
            //{
            //    registerTimer.Tick -= RegisterMouseMove;
            //    Point? lastMousePos = null;

            //    transparentPanel1.MouseMove += (o, e) =>
            //            {
            //                // filter out the initial moving
            //                // screensavers and especially multi-window apps can get spurrious WM_MOUSEMOVE events
            //                // that don't actually involve any movement (cursor chnages and some mouse driver software
            //                // can generate them, for example) - so we record the actual mouse position and compare against it for actual movement.
            //                if (lastMousePos == null)
            //                {
            //                    lastMousePos = Cursor.Position;
            //                }
            //                else if (lastMousePos != Cursor.Position && _closeOnMouseMovement)
            //                    Close();
            //            };
            //}
            //registerTimer = new Timer() { Interval = 1000 };
            //registerTimer.Tick += RegisterMouseMove;
            //registerTimer.Start();
        }

        private void RotateSite()
        {
            if (_currentURLIndex >= _urls.Count)
            {
                _currentURLIndex = 0;
            }
            BrowseTo(_urls[_currentURLIndex]);
            _currentURLIndex++;
        }

        private void BrowseTo(string url)
        {            // Disable the user event handler while navigating
            //Application.RemoveMessageFilter(userEventHandler);

            if (string.IsNullOrWhiteSpace(url))
            {
                _webBrowser.Visible = false;
            }
            else
            {
                _webBrowser.Visible = true;
                try
                {
                    Debug.WriteLine($"Navigating: {url}");
                    _webBrowser.CoreWebView2.Navigate(url);
                }
                catch
                {
                    // This can happen if IE pops up a window that isn't closed before the next call to Navigate()
                }
            }
            //Application.AddMessageFilter(userEventHandler);
        }

        /// <summary>
        /// Allows capturing the ESC key to close the form.
        /// </summary>
        //protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        //{
        //    //if (keyData == Keys.Escape)
        //    //{
        //    //    Close();
        //    //    return true;``
        //    //}
        //    //return base.ProcessCmdKey(ref msg, keyData);
        //}

    }

    public class GlobalUserEventHandler : IMessageFilter
    {
        public delegate void UserEvent();

        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_MBUTTONDBLCLK = 0x209;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;

        // screensavers and especially multi-window apps can get spurious WM_MOUSEMOVE events
        // that don't actually involve any movement (cursor changes and some mouse driver software
        // can generate them, for example) - so we record the actual mouse position and compare against it for actual movement.
        private Point? lastMousePos;

        public event UserEvent Event;

        public bool PreFilterMessage(ref Message m)
        {

            if (((m.Msg == WM_MOUSEMOVE) && (Cursor.Position != this.lastMousePos))
                || (m.Msg > WM_MOUSEMOVE && m.Msg <= WM_MBUTTONDBLCLK) || m.Msg == WM_KEYDOWN || m.Msg == WM_KEYUP)
            {

                if (Event != null)
                {
                    Event();
                }
            }
            // Always allow message to continue to the next filter control
            return false;
        }
        public GlobalUserEventHandler()
        {
            this.lastMousePos = Cursor.Position;
        }
    }
}

