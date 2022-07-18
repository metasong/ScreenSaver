namespace Metaseed.WebPageScreenSaver
{
    partial class ScreenSaverForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer _components = null;
        private Microsoft.Web.WebView2.WinForms.WebView2 _webBrowser;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._webBrowser = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.timerUrlSwitch = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this._webBrowser)).BeginInit();
            this.SuspendLayout();
            // 
            // _webBrowser
            // 
            this._webBrowser.AllowExternalDrop = true;
            this._webBrowser.CreationProperties = null;
            this._webBrowser.DefaultBackgroundColor = System.Drawing.Color.White;
            this._webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this._webBrowser.Location = new System.Drawing.Point(0, 0);
            this._webBrowser.Margin = new System.Windows.Forms.Padding(0);
            this._webBrowser.Name = "_webBrowser";
            this._webBrowser.Size = new System.Drawing.Size(278, 244);
            this._webBrowser.Source = new System.Uri("about:blank", System.UriKind.Absolute);
            this._webBrowser.TabIndex = 0;
            this._webBrowser.TabStop = false;
            this._webBrowser.ZoomFactor = 1D;
            // 
            // ScreenSaverForm
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Controls.Add(this._webBrowser);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScreenSaverForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Load += new System.EventHandler(this.ScreenSaverForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this._webBrowser)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerUrlSwitch;
        private System.ComponentModel.IContainer components;
    }
}
