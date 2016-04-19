using MahApps.Metro.Controls;
using MRMPConfigurator.MRMPWCFService;
using System.Collections.Generic;
using System.ComponentModel;

namespace MRMPConfigurator
{
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        BackgroundWorker platformrefreshbgworker = new BackgroundWorker();

        public void refresh_platform(Platform _platform)
        {
            if (!platformbgworker.IsBusy)
            {
                platforms_progress_message.Content = string.Format("Refreshing platform {0}", _platform.description);
                platforms_progress_indicator.Visibility = System.Windows.Visibility.Visible;
                platforms_progress_message.Visibility = System.Windows.Visibility.Visible;
                platforms_progress_indicator.IsActive = true;

                platformrefreshbgworker.WorkerReportsProgress = true;
                platformrefreshbgworker.WorkerSupportsCancellation = true;
                platformrefreshbgworker.DoWork += new DoWorkEventHandler(refresh_platform_worker);
                platformrefreshbgworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(refresh_platform_worker_complete);
                platformrefreshbgworker.RunWorkerAsync(_platform);
            }
        }
        public void refresh_platform_worker(object sender, DoWorkEventArgs e)
        {
            Platform _platform = e.Argument as Platform;
            channel.RefreshPlatform(_platform);
        }
        private void refresh_platform_worker_complete(object sender, RunWorkerCompletedEventArgs e)
        {
            platforms_progress_indicator.Visibility = System.Windows.Visibility.Collapsed;
            platforms_progress_message.Visibility = System.Windows.Visibility.Collapsed;

            //refest platform list after platform was refreshed
            load_platformlist();
        }
    }
}
