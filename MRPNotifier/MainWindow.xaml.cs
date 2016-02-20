using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using MahApps.Metro.Controls;
using System.Collections.Generic;
using MRPNotifier.Forms;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Globalization;
using MRPNotifier.Models;
using System.Diagnostics;
using System.Windows.Media;
using System.Linq;
using System.Windows.Input;
using System.Data;
using MRPNotifier.Extensions;
using System.Threading;
using MRPNotifier.MRPWCFService;

namespace MRPNotifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        MRPWCFServiceClient channel = new MRPWCFServiceClient();

        private BackgroundWorker platformloader = new BackgroundWorker();
        private List<Platform> _platform_list = new List<Platform>();
        private List<Credential> _credential_list = new List<Credential>();

        private List<Workload_ObjectDataModel> _workloads = new List<Workload_ObjectDataModel>();
        workerInformation _information = null;

        public List<Credential> workload_credentials()
        {
            return channel.ListCredentials().Where(x => x.credential_type == 1).ToList();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        public MainWindow()
        {
            InitializeComponent();
            _information = channel.CollectionInformation();
            //Assign global assignment for tree

            //apply item source and apply filter
            _workloads = new Workload_ListDataModel().list;
            lvWorkloads.ItemsSource = _workloads;
            lvWorkloads.Items.Filter = new Predicate<object>(workloads_enabled);


            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipText = "MRP Notifier has been minimised. Click the tray icon to show.";
            m_notifyIcon.BalloonTipTitle = "MRP Notifier";
            m_notifyIcon.Text = "MRP Notifier";
            m_notifyIcon.Icon = new Icon(Application.GetResourceStream(new Uri("pack://application:,,,/cloudmovey.ico")).Stream);
            m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);

            platformloader.WorkerReportsProgress = true;
            platformloader.WorkerSupportsCancellation = true;
            platformloader.DoWork += new DoWorkEventHandler(load_platformlist);
            platformloader.ProgressChanged += new ProgressChangedEventHandler(load_platformlist_changed);
            platformloader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(load_platformlist_complete);

        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {

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
                Decorator border = VisualTreeHelper.GetChild(this.lvPlatforms, 0) as Decorator;
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
        private void lvCredentials_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                GridView view = this.lvCredentials.View as GridView;
                Decorator border = VisualTreeHelper.GetChild(this.lvCredentials, 0) as Decorator;
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
        private void lvWorkloads_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (e.WidthChanged)
            {
                double remainingSpace = lvWorkloads.ActualWidth;

                if (remainingSpace > 0)
                {
                    for (int i = 0; i < (lvWorkloads.View as GridView).Columns.Count; i++)
                        if (i != 2)
                            remainingSpace -= (lvWorkloads.View as GridView).Columns[i].ActualWidth;

                    //Leave 15 px free for scrollbar
                    remainingSpace -= 15;
                    if (remainingSpace > 0)
                    {
                        (lvWorkloads.View as GridView).Columns.Last().Width = remainingSpace;
                    }
                }
            }
        }
        private static DependencyObject GetDependencyObjectFromVisualTree(DependencyObject startObject, Type type)
        {
            var parent = startObject;
            while (parent != null)
            {
                if (type.IsInstanceOfType(parent))
                    break;
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent;
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
        void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            ShowTrayIcon(true);
            m_notifyIcon.ShowBalloonTip(2000);
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
            refesh_platform_list();
        }
        private void workload_credential_changed(object sender, RoutedEventArgs e)
        {

        }
        private void refresh_platforms_button_clicked(object sender, RoutedEventArgs e)
        {
            UiServices.SetBusyState();
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
        private void refresh_platform_button(object sender, RoutedEventArgs e)
        {
            Platform _platform = (Platform)((Button)sender).DataContext;
            BackgroundWorker bgwanalysis = new BackgroundWorker();
            bgwanalysis.DoWork += delegate
            {
                channel.RefreshPlatform(_platform);
            };
            bgwanalysis.RunWorkerAsync();

            while (bgwanalysis.IsBusy)
            {
                Thread.Sleep(2000);
            }
            //reload workload list
            _workloads = new Workload_ListDataModel().list;


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
            MahApps.Metro.Controls.Dialogs.MessageDialogResult messageBoxResult = await this.ShowMessageAsync("Delete Platform", "Are you sure you want to delete this platform?", MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (messageBoxResult == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
            {
                channel.DestroyPlatform(_platform);
                refesh_platform_list();
            }

        }
        private void refresh_workload_button(object sender, RoutedEventArgs e)
        {
            Workload_ObjectDataModel _workload = (Workload_ObjectDataModel)((Button)sender).DataContext;
            //channel.RefreshPlatform(_platform);
            //refesh_platform_list();
        }
        private async void delete_workload_button(object sender, RoutedEventArgs e)
        {
            Platform _platform = (Platform)((Button)sender).DataContext;
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Delete",
                NegativeButtonText = "Abort",
                ColorScheme = MetroDialogColorScheme.Theme
            };
            MahApps.Metro.Controls.Dialogs.MessageDialogResult messageBoxResult = await this.ShowMessageAsync("Delete Platform", "Are you sure you want to delete this platform?", MessageDialogStyle.AffirmativeAndNegative, mySettings);

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        //Workload listview drag&drop
        System.Windows.Point startPoint;
        private void lvWorkloads_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            startPoint = e.GetPosition(null);
        }

        private void lvWorkloads_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            System.Windows.Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed && ((Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance) || (Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)))
            {
                // Get the dragged ListViewItem
                ListViewItem listViewItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

                // Find the data behind the ListViewItem
                Workload_ObjectDataModel _workload = (Workload_ObjectDataModel)listViewItem.DataContext;

                //// Initialize the drag & drop operation
                DataObject dragData = new DataObject(_workload);
                DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
            }
        }
        // Helper to search up the VisualTree
        private static T FindAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }



        private void workload_search_filter(object sender, RoutedEventArgs e)
        {
            UiServices.SetBusyState();

            lvWorkloads.Items.Filter = new Predicate<object>(workload_search_object);

        }
        private void workload_filter_toggle(object sender, RoutedEventArgs e)
        {
            UiServices.SetBusyState();

            if ((bool)workload_filter_toggleswitch.IsChecked)
            {
                lvWorkloads.Items.Filter = new Predicate<object>(workloads_disabled);
            }
            else
            {
                lvWorkloads.Items.Filter = new Predicate<object>(workloads_enabled);
            }
        }
        public bool workload_search_object(object item)
        {
            return ((item as Workload_ObjectDataModel).hostname.IndexOf(workload_search.Text, StringComparison.OrdinalIgnoreCase) >= 0 && (item as Workload_ObjectDataModel).enabled == ((bool)workload_filter_toggleswitch.IsChecked ? false : true));
        }
        public bool workloads_enabled(object de)
        {
            Workload_ObjectDataModel workload = de as Workload_ObjectDataModel;
            return ((workload.enabled == true) && workload.enabled == ((bool)workload_filter_toggleswitch.IsChecked ? false : true));
        }
        public bool workloads_disabled(object de)
        {
            Workload_ObjectDataModel workload = de as Workload_ObjectDataModel;
            return ((workload.enabled == false) && workload.enabled == ((bool)workload_filter_toggleswitch.IsChecked ? false : true));
        }
    }

    public partial class WorkerState
    {
        public string message { get; set; }
    }

}
