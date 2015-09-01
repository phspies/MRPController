using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using MahApps.Metro.Controls;
using System.Collections.Generic;
using CloudMoveyNotifier.CloudMoveyWCF;
using CloudMoveyNotifier.Forms;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Globalization;
using System.Windows.Data;
using CloudMoveyNotifier.Models;
using System.Diagnostics;

namespace CloudMoveyNotifier
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        CloudMoveyServiceClient channel = new CloudMoveyServiceClient();
        private BackgroundWorker platformloader = new BackgroundWorker();
        private List<Platform> _platform_list = new List<Platform>();
        private List<Credential> _credential_list = new List<Credential>();

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

            platformloader.WorkerReportsProgress = true;
            platformloader.WorkerSupportsCancellation = true;
            platformloader.DoWork += new DoWorkEventHandler(load_platformlist);
            platformloader.ProgressChanged += new ProgressChangedEventHandler(load_platformlist_changed);
            platformloader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(load_platformlist_complete);

            //attached 



        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            refesh_platform_list();
            refesh_credential_list();

        }
        public object ResolveVendorString(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = (bool)value;
            return (isVisible ? Visibility.Visible : Visibility.Collapsed);
        }
        void ResolveVendorString()
        {

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

        private void load_platformlist(object sender, DoWorkEventArgs e)
        {
            WorkerState _state = new WorkerState();
            
            List<Platform> _platform_list = new List<Platform>();
            _platform_list = channel.ListPlatforms();
            int index = 1;
            foreach (var platform in _platform_list)
            {
                int percentage = (index / _platform_list.Count) * 100;
                _state.message = String.Format("Updating {0} [{1} complete]", platform.description, percentage);
                (sender as BackgroundWorker).ReportProgress(percentage, _state);
                var dummy = platform.platform_details;
            }
            e.Result = _platform_list;
        }
        private void load_platformlist_complete(object sender, RunWorkerCompletedEventArgs e)
        {
            _platform_list = (List<Platform>)e.Result;
            lvPlatforms.ItemsSource = _platform_list;
            progress_indicator.Visibility = Visibility.Collapsed;
            progress_message.Visibility = Visibility.Collapsed;
        }

        private void load_platformlist_changed(object sender, ProgressChangedEventArgs e)
        {
            WorkerState ws = e.UserState as WorkerState;
            Debug.Print(ws.message);
            progress_message.Content = ws.message;

        }

        private void add_platforms_button_clicked(object sender, RoutedEventArgs e)
        {
            PlatformForm _form = new PlatformForm(new Platform(), 0, _credential_list);
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
            PlatformForm _form = new PlatformForm(_platform, 1, _credential_list);
            if (_form.ShowDialog() == true)
            {
                channel.UpdatePlatform(_form._record);
            }
            refesh_platform_list();
        }
        private async void delete_platform_button(object sender, RoutedEventArgs e)
        {
            Platform _platform = (Platform)((Button)sender).DataContext;
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Delete",
                NegativeButtonText = "Abort",
                ColorScheme = MetroDialogColorScheme.Theme
            };
            MahApps.Metro.Controls.Dialogs.MessageDialogResult messageBoxResult = await this.ShowMessageAsync("Delete Platform", "Are you sure you want to delete this platform?",MessageDialogStyle.AffirmativeAndNegative, mySettings);
            
            if (messageBoxResult == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
            {
                channel.DestroyPlatform(_platform);
                refesh_platform_list();
            }

        }
        private void update_credential_button(object sender, RoutedEventArgs e)
        {
            Credential _credential = (Credential)((Button)sender).DataContext;
            CredentialForm _form = new CredentialForm(_credential, 1);
            if (_form.ShowDialog() == true)
            {
                channel.UpdateCredential(_form._record);
            }
            refesh_credential_list();
        }
        private async void delete_credential_button(object sender, RoutedEventArgs e)
        {
            Credential _credential = (Credential)((Button)sender).DataContext;
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Delete",
                NegativeButtonText = "Abort",
                ColorScheme = MetroDialogColorScheme.Theme
            };
            MahApps.Metro.Controls.Dialogs.MessageDialogResult messageBoxResult = await this.ShowMessageAsync("Delete Platform", "Are you sure you want to delete this credential?", MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (messageBoxResult == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
            {
                channel.DestroyCredential(_credential);
                refesh_credential_list();
            }

        }
        private void refresh_credentials_button_clicked(object sender, RoutedEventArgs e)
        {
            refesh_credential_list();
        }

        private void add_credentials_button_clicked(object sender, RoutedEventArgs e)
        {
            CredentialForm _form = new CredentialForm(new Credential(), 0);
            if (_form.ShowDialog() == true)
            {
                channel.AddCredential(_form._record);
            }
            refesh_credential_list();
        }
        private void refesh_credential_list()
        {
            _credential_list = channel.ListCredentials();
            lvCredentials.ItemsSource = _credential_list;
        }
        
        private void refesh_platform_list()
        {
            if (!platformloader.IsBusy)
            {
                progress_indicator.Visibility = Visibility.Visible;
                progress_message.Visibility = Visibility.Visible;
                progress_indicator.IsActive = true;
                progress_message.Content = "Refreshing platform list";
                platformloader.RunWorkerAsync();
            }
        }
    }

    public partial class WorkerState
    {
        public string message {get; set;}
    }
   
}
