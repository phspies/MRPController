using System;
using System.Drawing;
using System.Windows;
using MahApps.Metro.Controls;
using System.Collections.Generic;
using MRMPConfigurator.Forms;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using MRMPConfigurator.Models;
using System.Windows.Media;
using System.Linq;
using System.Data;
using MRMPConfigurator.Extensions;
using MRMPConfigurator.MRMPWCFService;
using System.Windows.Input;

namespace MRMPConfigurator
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        MRPWCFServiceClient channel = new MRPWCFServiceClient();
        
        private WindowState m_storedWindowState = WindowState.Normal;
        workerInformation _information = null;

        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        public MainWindow()
        {
            InitializeComponent();

            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipText = "MRP Notifier has been minimised. Click the tray icon to show.";
            m_notifyIcon.BalloonTipTitle = "MRP Notifier";
            m_notifyIcon.Text = "MRP Notifier";
            m_notifyIcon.Icon = new Icon(Application.GetResourceStream(new Uri("pack://application:,,,/mrp.ico")).Stream);
            m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);

        }
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            //pupolate window elements frem service
            load_credentiallist();
            load_information();
            load_platformlist();
            load_workloadlist();
        }

        //pupulate credential combo wiht data
        public List<Credential> workload_credentials()
        {
            List<Credential> _private_credentials = new List<Credential>();
            try
            {
                _private_credentials = channel.ListCredentials();
            }
            catch (Exception ex)
            {
                            }
            return _private_credentials;
        }
        private void lvTasks_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                GridView view = this.lvTasks.View as GridView;
                Decorator border = VisualTreeHelper.GetChild(this.lvTasks, 0) as Decorator;
                if (border != null)
                {
                    ScrollViewer scroller = border.Child as ScrollViewer;
                    if (scroller != null)
                    {
                        ItemsPresenter presenter = scroller.Content as ItemsPresenter;
                        if (presenter != null)
                        {
                            view.Columns[0].Width = presenter.ActualWidth;
                            for (int i = 1; i < view.Columns.Count; i++)
                            {
                                view.Columns[0].Width -= view.Columns[i].ActualWidth;
                            }
                        }
                    }
                }
            }
        }
        private void lvPlatforms_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                GridView view = this.lvPlatforms.View as GridView;
                Decorator border = VisualTreeHelper.GetChild(this.lvTasks, 0) as Decorator;
                if (border != null)
                {
                    ScrollViewer scroller = border.Child as ScrollViewer;
                    if (scroller != null)
                    {
                        ItemsPresenter presenter = scroller.Content as ItemsPresenter;
                        if (presenter != null)
                        {
                            view.Columns[0].Width = presenter.ActualWidth;
                            for (int i = 1; i < view.Columns.Count; i++)
                            {
                                view.Columns[0].Width -= view.Columns[i].ActualWidth + 10;
                            }
                        }
                    }
                }
            }
        }
        private void lvCredentials_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                GridView view = this.lvCredentials.View as GridView;
                Decorator border = VisualTreeHelper.GetChild(this.lvTasks, 0) as Decorator;
                if (border != null)
                {
                    ScrollViewer scroller = border.Child as ScrollViewer;
                    if (scroller != null)
                    {
                        ItemsPresenter presenter = scroller.Content as ItemsPresenter;
                        if (presenter != null)
                        {
                            view.Columns[0].Width = presenter.ActualWidth;
                            for (int i = 1; i < view.Columns.Count; i++)
                            {
                                view.Columns[0].Width -= view.Columns[i].ActualWidth + 10;
                            }
                        }
                    }
                }
            }
        }
        private void lvWorkloads_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                GridView view = this.lvWorkloads.View as GridView;
                Decorator border = VisualTreeHelper.GetChild(this.lvTasks, 0) as Decorator;
                if (border != null)
                {
                    ScrollViewer scroller = border.Child as ScrollViewer;
                    if (scroller != null)
                    {
                        ItemsPresenter presenter = scroller.Content as ItemsPresenter;
                        if (presenter != null)
                        {
                            view.Columns[0].Width = presenter.ActualWidth;
                            for (int i = 1; i < view.Columns.Count; i++)
                            {
                                view.Columns[0].Width -= view.Columns[i].ActualWidth+10;
                            }
                        }
                    }
                }
            }
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            load_platformlist();
        }
        void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            ShowTrayIcon(true);
            m_notifyIcon.ShowBalloonTip(2000);
        }
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

        private void copy_guid_button_clicked(object sender, RoutedEventArgs e)
        {
            String GUID = _information.agentId;
            System.Windows.Forms.Clipboard.SetText(GUID, System.Windows.Forms.TextDataFormat.Text);
        }
        private void add_platforms_button_clicked(object sender, RoutedEventArgs e)
        {
            PlatformForm _form = new PlatformForm(new Platform(), 0, _credential_list);
            if (_form.ShowDialog() == true)
            {
                channel.AddPlatform(_form._record);
            }
            load_platformlist();
        }
        private void workload_credential_changed(object sender, RoutedEventArgs e)
        {

        }
        private void refresh_platforms_button_clicked(object sender, RoutedEventArgs e)
        {
            UiServices.SetBusyState();
            load_platformlist();
        }
        private void update_platform_button(object sender, RoutedEventArgs e)
        {
            Platform _platform = (Platform)((Button)sender).DataContext;
            PlatformForm _form = new PlatformForm(_platform, 1, _credential_list);
            if (_form.ShowDialog() == true)
            {
                channel.UpdatePlatform(_form._record);
            }
            load_platformlist();
        }
        private void refresh_platform_button(object sender, RoutedEventArgs e)
        {
            Platform _platform = (Platform)((Button)sender).DataContext;
            refresh_platform(_platform);
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
            MessageDialogResult messageBoxResult = await this.ShowMessageAsync("Delete Platform", "Are you sure you want to delete this platform?", MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (messageBoxResult == MessageDialogResult.Affirmative)
            {
                channel.DestroyPlatform(_platform);
                load_platformlist();
            }

        }
        private void refresh_workload_button(object sender, RoutedEventArgs e)
        {
            Workload _workload = (Workload)((Button)sender).DataContext;
            //channel.RefreshPlatform(_platform);
            //refesh_platform_list();
        }
        private async void delete_workload_button(object sender, RoutedEventArgs e)
        {
            Workload _workload = (Workload)((Button)sender).DataContext;
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Delete",
                NegativeButtonText = "Abort",
                ColorScheme = MetroDialogColorScheme.Theme
            };
            MessageDialogResult messageBoxResult = await this.ShowMessageAsync("Delete Workload", "Are you sure you want to delete this workload?", MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (messageBoxResult == MessageDialogResult.Affirmative)
            {
                channel.DestroyWorkload(_workload);
                load_workloadlist();
            }

        }
        private async void update_credential_button(object sender, RoutedEventArgs e)
        {
            Credential _credential = (Credential)((Button)sender).DataContext;
            CredentialForm _form = new CredentialForm(_credential, 1);
            if (_form.ShowDialog() == true)
            {
                try
                {
                    channel.UpdateCredential(_form._record);
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("Error while contacting MRMP Service", ex.Message);
                }
            }
            using (new WaitCursor())
            {
                load_credentiallist();
            }
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
            MessageDialogResult messageBoxResult = await this.ShowMessageAsync("Delete Credential", "Are you sure you want to delete this credential?", MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (messageBoxResult == MessageDialogResult.Affirmative)
            {

                try
                {
                    using (new WaitCursor())
                    {
                        channel.DestroyCredential(_credential);
                        load_credentiallist();
                    }
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("Error while contacting MRMP Service", ex.Message);
                }
            }

        }

        private void workload_status_toggle(object sender, EventArgs e)
        {
            Workload _workload = (sender as ToggleSwitch).DataContext as Workload;
            ToggleSwitch _switch = sender as ToggleSwitch;
            if (_workload.id != null)
            {
                _workload.enabled = (bool)_switch.IsChecked;
                using (new WaitCursor())
                {
                    channel.UpdateWorkload(_workload);
                }
            }

        }
        private void workload_credentials_changed(object sender, System.EventArgs e)
        {
            Workload _workload = (sender as ComboBox).DataContext as Workload;
            Credential _credential = (sender as ComboBox).SelectedItem as Credential;

            if (_workload != null && _credential != null)
            {
                _workload.credential_id = _credential.id;
                using (new WaitCursor())
                {
                    channel.UpdateWorkload(_workload);
                }
            }
        }
        private void refresh_credentials_button_clicked(object sender, RoutedEventArgs e)
        {
            using (new WaitCursor())
            {
                load_credentiallist();
            }
        }
        private void add_credentials_button_clicked(object sender, RoutedEventArgs e)
        {
            CredentialForm _form = new CredentialForm(new Credential(), 0);
            if (_form.ShowDialog() == true)
            {
                channel.AddCredential(_form._record);
            }
            using (new WaitCursor())
            {
                load_credentiallist();
            }
        }

        private void workload_search_filter(object sender, RoutedEventArgs e)
        {
            using (new WaitCursor())
            {
                lvWorkloads.Items.Filter = new Predicate<object>(workload_search_object);
            }

        }
        public bool workload_search_object(object item)
        {
            using (new WaitCursor())
            {
                return ((item as Workload).hostname.IndexOf(workload_search.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }
        public class WaitCursor : IDisposable
        {
            private Cursor _previousCursor;

            public WaitCursor()
            {
                _previousCursor = Mouse.OverrideCursor;

                Mouse.OverrideCursor = Cursors.Wait;
            }

            #region IDisposable Members

            public void Dispose()
            {
                Mouse.OverrideCursor = _previousCursor;
            }

            #endregion
        }



        private void refresh_workloads_button_clicked(object sender, RoutedEventArgs e)
        {
            using (new WaitCursor())
            {
                load_workloadlist();
            }
        }

        private void import_workloads_button_clicked(object sender, RoutedEventArgs e)
        {
            ImportWorkloadsForm _form = new ImportWorkloadsForm(_credential_list);
            if (_form.ShowDialog() == true)
            {
                if (_form.DialogResult == true)
                {
                    if (_form._imported_workloads.Count > 0)
                    {
                        foreach (Workload _workload in _form._imported_workloads)
                        {
                            _workload.osedition = "Windows";
                            _workload.vcore = 0;
                            _workload.vcpu = 0;
                            _workload.vmemory = 0;
                            _workload.cpu_coresPerSocket = 0;
                            _workload.storage_count = 0;
                            _workload.credential_ok = false;
                            channel.AddWorkload(_workload);
                        }
                        load_workloadlist();
                    }
                }
            }
        }
        private void add_workload_button_clicked(object sender, RoutedEventArgs e)
        {
            WorkloadForm _form = new WorkloadForm(new Workload(), 0, _credential_list);
            if (_form.ShowDialog() == true)
            {
                _form._record.osedition = "windows";
                _form._record.vcore = 0;
                _form._record.vcpu = 0;
                _form._record.vmemory = 0;
                _form._record.cpu_coresPerSocket = 0;
                _form._record.enabled = false;
                _form._record.storage_count = 0;
                _form._record.credential_ok = false;
                channel.AddWorkload(_form._record);
            }
            load_workloadlist();
        }
        private void update_workload_button(object sender, RoutedEventArgs e)
        {
            Workload _workload = (Workload)((Button)sender).DataContext;
            WorkloadForm _form = new WorkloadForm(_workload, 1, _credential_list);
            if (_form.ShowDialog() == true)
            {
                channel.UpdateWorkload(_form._record);
            }
            load_workloadlist();
        }
    }

    public partial class WorkerState
    {
        public string message { get; set; }
    }

}
