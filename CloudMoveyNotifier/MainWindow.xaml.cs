using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using Microsoft.Win32;

namespace CloudMoveyNotifier
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker bw = new BackgroundWorker();

        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        public MainWindow()
        {
            InitializeComponent();

            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipText = "CloudMovey Notifier has been minimised. Click the tray icon to show.";
            m_notifyIcon.BalloonTipTitle = "CloudMovey Notifier";
            m_notifyIcon.Text = "CloudMovey Notifier";
            m_notifyIcon.Icon = new Icon(Application.GetResourceStream(new Uri("pack://application:,,,/cloudmovey.ico")).Stream);
            m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);

            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerAsync();

        }
        void OnClose(object sender, CancelEventArgs args)
        {
            m_notifyIcon.Dispose();
            m_notifyIcon = null;
        }

        private WindowState m_storedWindowState = WindowState.Normal;
        void OnStateChanged(object sender, EventArgs args)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                if (m_notifyIcon != null)
                    m_notifyIcon.ShowBalloonTip(2000);
            }
            else
                m_storedWindowState = WindowState;
        }
        void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            CheckTrayIcon();
        }

        void m_notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = m_storedWindowState;
        }
        void CheckTrayIcon()
        {
            ShowTrayIcon(!IsVisible);
        }

        void ShowTrayIcon(bool show)
        {
            if (m_notifyIcon != null)
                m_notifyIcon.Visible = show;
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int progress = 0;
            while (true)
            {
                progress += 1;
                try
                {
                    String _registry = @"SOFTWARE\CloudMovey Worker Service";
                    RegistryKey rkSubKey = Registry.LocalMachine.OpenSubKey(_registry, true);
                    WorkerState _state = new WorkerState() { guid = rkSubKey.GetValue("agentId", false) as String, version = rkSubKey.GetValue("agentVersion", false) as String };
                    worker.ReportProgress(progress, _state);
                }
                catch(Exception error)
                {
                    worker.ReportProgress(progress, new WorkerState() { guid = error.Message, version = "error" });
                }
                System.Threading.Thread.Sleep(10000);

            }
        }
      
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WorkerState ws = e.UserState as WorkerState;
            this.worker_guid.Text = ws.guid;
            this.worker_version.Text = ws.version;
        }

    }

    public partial class WorkerState
    {
        public string guid {get; set;}
        public string version {get; set;}
    }
}
