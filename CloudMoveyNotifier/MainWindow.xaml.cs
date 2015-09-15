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
using CloudMoveyNotifier.Models;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using WPF.JoshSmith.Controls.Utilities;

namespace CloudMoveyNotifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        CloudMoveyServiceClient channel = new CloudMoveyServiceClient();
        private BackgroundWorker platformloader = new BackgroundWorker();
        private List<Platform> _platform_list = new List<Platform>();
        private List<Credential> _credential_list = new List<Credential>();
        private List<Failovergroup> _failovergroup_list = new List<Failovergroup>();
        private ObservableCollection<Failovergroup_TreeModel> _failovergroup_tree = new ObservableCollection<Failovergroup_TreeModel>();
        workerInformation _information = null;

        public List<Credential> workload_credentials()
        {
            Debug.WriteLine("Passing _workload_credentials along");
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
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            load_failovergrouptree();

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
                GridView view = this.lvWorkloads.View as GridView;
                Decorator border = VisualTreeHelper.GetChild(this.lvWorkloads, 0) as Decorator;
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
        private void Failovergroup_add_click(object sender, RoutedEventArgs e)
        {

            MenuItem item = (MenuItem)e.OriginalSource;
            Failovergroup_TreeModel _parent_failovergroup = (Failovergroup_TreeModel)(Failovergroup_treeview.SelectedValue);
            if (_parent_failovergroup != null)
            {
                FailovergroupForm _form = new FailovergroupForm(new Failovergroup(), 0);
                if (_form.ShowDialog() == true)
                {
                    _form._record.parent_id = _parent_failovergroup.group_object.id;
                    channel.AddFailovergroup(_form._record);
                }
                load_failovergrouptree();
            }
        }
        private void Failovergroup_update_click(object sender, RoutedEventArgs e)
        {
            Failovergroup_TreeModel _failovergroup = (Failovergroup_TreeModel)(Failovergroup_treeview.SelectedValue);
            FailovergroupForm _form = new FailovergroupForm(_failovergroup.group_object, 1);
            if (_form.ShowDialog() == true)
            {
                channel.UpdateFailovergroup(_form._record);
            }
            load_failovergrouptree();
        }
        private void Failovergroup_destroy_click(object sender, RoutedEventArgs e)
        {
            Failovergroup_TreeModel item = ((sender as MenuItem).DataContext) as Failovergroup_TreeModel;

        }
        private void Failovergroup_moveup_click(object sender, RoutedEventArgs e)
        {
            Failovergroup_TreeModel item = ((sender as MenuItem).DataContext) as Failovergroup_TreeModel;

        }
        private void Failovergroup_movedown_click(object sender, RoutedEventArgs e)
        {
            Failovergroup_TreeModel item = ((sender as MenuItem).DataContext) as Failovergroup_TreeModel;

        }

        private void load_failovergrouptree(Failovergroup _failovergroup = null)
        {
            _failovergroup_list = channel.ListFailovergroups();
            if (_failovergroup == null && _failovergroup_list.Count == 0)
            {
                channel.AddFailovergroup(new Failovergroup() { group = "Root Group", group_type = 10 });
                _failovergroup_list = channel.ListFailovergroups();
            }
            _failovergroup_tree = FlatToHierarchy(_failovergroup_list);
            Failovergroup_treeview.ItemsSource = _failovergroup_tree;

        }
        public ObservableCollection<Failovergroup_TreeModel> FlatToHierarchy(IEnumerable<Failovergroup> list, string id = null)
        {
            ObservableCollection<Failovergroup_TreeModel> _children = new ObservableCollection<Failovergroup_TreeModel>();
            foreach (Failovergroup _group in list.Where(x => x.parent_id == id))
            {
                _children.Add(new Failovergroup_TreeModel() { group = _group.group, group_object = _group, id = _group.id, group_type = _group.group_type, children = FlatToHierarchy(list, _group.id) });
            }
            return _children;
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
        private void failovergroup_treeview_selectionchanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var model = e.NewValue as Failovergroup_TreeModel;

            if (model != null)
            {
                if (model.parent_id == null)
                {
                    lvWorkloads.ItemsSource = channel.ListWorkloads();
                }
                Debug.WriteLine(String.Format("{0} : {1}", model.id, model.group));
            }
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
        private void refresh_platform_button(object sender, RoutedEventArgs e)
        {
            Platform _platform = (Platform)((Button)sender).DataContext;
            channel.RefreshPlatform(_platform);
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
            Platform _platform = (Platform)((Button)sender).DataContext;
            channel.RefreshPlatform(_platform);
            refesh_platform_list();
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
                ListView listView = sender as ListView;
                ListViewItem listViewItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

                // Find the data behind the ListViewItem
                Workload _workload = (Workload)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);

                // Initialize the drag & drop operation
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


        // DROP
        private void Tree_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Workload)))
            {
                var workload = e.Data.GetData(typeof(Workload)) as Workload;
                var failovergroup = ((TextBlock)e.OriginalSource).DataContext as Failovergroup_TreeModel;

                Debug.WriteLine(String.Format("{0} {1}", workload.hostname, failovergroup.group));

            }
            Failovergroup_treeview.Items.Refresh();

        }

        private void Tree_DragOver(object sender, DragEventArgs e)
        {
            TreeViewItem treeViewItem = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
            if (treeViewItem != null)
            {
                BrushConverter conv = new BrushConverter();
                treeViewItem.Background = conv.ConvertFromString("#69BE28") as SolidColorBrush;
            }
        }
        private void Tree_DragLeave(object sender, DragEventArgs e)
        {
            TreeViewItem treeViewItem = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
            if (treeViewItem != null)
            {
                treeViewItem.Background = System.Windows.Media.Brushes.White;
            }
        }


    }

    public partial class WorkerState
    {
        public string message { get; set; }
    }

}
