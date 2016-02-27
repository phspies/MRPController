using MahApps.Metro.Controls;
using MRPNotifier.MRPWCFService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPNotifier
{
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        private List<Platform> _platform_list = new List<Platform>();
        BackgroundWorker platformloader = new BackgroundWorker();

        public void load_platformlist()
        {
            if (!platformloader.IsBusy)
            {
                progress_indicator.Visibility = System.Windows.Visibility.Visible;
                progress_message.Visibility = System.Windows.Visibility.Visible;
                progress_indicator.IsActive = true;
                progress_message.Content = "Refreshing platform list";
            
                platformloader.WorkerReportsProgress = true;
                platformloader.WorkerSupportsCancellation = true;
                platformloader.DoWork += new DoWorkEventHandler(load_platformlist_worker);
                platformloader.ProgressChanged += new ProgressChangedEventHandler(load_platformlist_worker_changed);
                platformloader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(load_platformlist_worker_complete);
            }
        }
        public void load_platformlist_worker(object sender, DoWorkEventArgs e)
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
            }
            e.Result = _platform_list;
        }
        private void load_platformlist_worker_complete(object sender, RunWorkerCompletedEventArgs e)
        {
            _platform_list = (List<Platform>)e.Result;
            lvPlatforms.ItemsSource = _platform_list;
            progress_indicator.Visibility = System.Windows.Visibility.Collapsed;
            progress_message.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void load_platformlist_worker_changed(object sender, ProgressChangedEventArgs e)
        {
            WorkerState ws = e.UserState as WorkerState;
            Debug.Print(ws.message);
            progress_message.Content = ws.message;
        }
    }
}
