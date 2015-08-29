using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using MahApps.Metro.Controls;
using System.Collections.Generic;
using CloudMoveyNotifier.CloudMoveyWCF;
using CloudMoveyNotifier.Forms;
using System.Windows.Controls;

namespace CloudMoveyNotifier
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        CloudMoveyServiceClient channel = new CloudMoveyServiceClient();
        private BackgroundWorker bw = new BackgroundWorker();
        private List<Platform> _platform_list = new List<Platform>();
        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        public MainWindow()
        {
            InitializeComponent();

            //Load lists from worker service
            refesh_platform_list();


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


            //attached 
            


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

 
        }
      
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WorkerState ws = e.UserState as WorkerState;
            //this.worker_guid.Text = ws.guid;
            //this.worker_version.Text = ws.version;
            //this.worker_queue_count.Text = ws.worker_queue_count.ToString();
        }

        private void add_platforms_button_clicked(object sender, RoutedEventArgs e)
        {
            PlatformForm _form = new PlatformForm(new Platform(), 0);
            if (_form.ShowDialog() == true)
            {
                channel.AddPlatform(_form._record);
            }
            refesh_platform_list();
        }

        private void refresh_platforms_button_clicked(object sender, RoutedEventArgs e)
        {
            refesh_platform_list();
        }
        private void update_platform_button(object sender, RoutedEventArgs e)
        {
            Platform _platform = (Platform)((Button)sender).DataContext;
            PlatformForm _form = new PlatformForm(_platform, 1);
            if (_form.ShowDialog() == true)
            {
                channel.UpdatePlatform(_form._record);
            }
            refesh_platform_list();
        }
        private void delete_platform_button(object sender, RoutedEventArgs e)
        {
            Platform _platform = (Platform)((Button)sender).DataContext;
            channel.DestroyPlatform(_platform);
            refesh_platform_list();

        }
        private void refresh_credentials_button_clicked(object sender, RoutedEventArgs e)
        {

        }

        private void add_credentials_button_clicked(object sender, RoutedEventArgs e)
        {

        }
        private void refesh_platform_list()
        {
            _platform_list = channel.ListPlatforms();
            lvPlatforms.ItemsSource = _platform_list;
        }
    }

    public partial class WorkerState
    {
        public string guid {get; set;}
        public string version {get; set;}
        public int worker_queue_count { get; set; }
    }
}
