using MahApps.Metro.Controls;
using MRPNotifier.MRPWCFService;
using System.Collections.Generic;
using System.ComponentModel;

namespace MRPNotifier
{
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        private List<Platform> _platform_list = new List<Platform>();
        BackgroundWorker platformbgworker = new BackgroundWorker();

        public void load_platformlist()
        {
            if (!platformbgworker.IsBusy)
            {
                platforms_progress_indicator.Visibility = System.Windows.Visibility.Visible;
                platforms_progress_message.Visibility = System.Windows.Visibility.Visible;
                platforms_progress_indicator.IsActive = true;
                platforms_progress_message.Content = "Refreshing platform list";
            
                platformbgworker.WorkerReportsProgress = true;
                platformbgworker.WorkerSupportsCancellation = true;
                platformbgworker.DoWork += new DoWorkEventHandler(load_platformlist_worker);
                platformbgworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(load_platformlist_worker_complete);
                platformbgworker.RunWorkerAsync();
            }
        }
        public void load_platformlist_worker(object sender, DoWorkEventArgs e)
        {
            List<Platform> _platform_list = new List<Platform>();
            e.Result = channel.ListPlatforms();
        }
        private void load_platformlist_worker_complete(object sender, RunWorkerCompletedEventArgs e)
        {
            _platform_list = (List<Platform>)e.Result;
            lvPlatforms.ItemsSource = _platform_list;
            platforms_progress_indicator.Visibility = System.Windows.Visibility.Collapsed;
            platforms_progress_message.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
