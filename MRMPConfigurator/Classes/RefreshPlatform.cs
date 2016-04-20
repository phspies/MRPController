using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MRMPConfigurator.Classes;
using MRMPConfigurator.Classes.Common;
using MRMPConfigurator.MRMPWCFService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;

namespace MRMPConfigurator
{
    public partial class MainWindow : MetroWindow
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
            
            WCFResultType _result = new WCFResultType();
            try
            {
                Platform _platform = e.Argument as Platform;
                channel.RefreshPlatform(_platform);
                _result.status = true;
            }
            catch (TimeoutException ex)
            {
                _result.status = false;
                _result.message = String.Format("Service has timed out : {0}", ex.Message);
            }
            catch (FaultException<UnexpectedServiceFault> ex)
            {
                _result.message = String.Format("Service error occurred: {0} {1}", ex.Message, System.Environment.NewLine);
                _result.message = String.Format("service message: {0} {1}", ex.Detail.ErrorMessage, System.Environment.NewLine);
                _result.message = String.Format("source: {0} {1}", ex.Detail.Source, System.Environment.NewLine);
                _result.message = String.Format("target: {0} {1}", ex.Detail.Target, System.Environment.NewLine);
                _result.message = String.Format("stack trace: {0} {1}", ex.Detail.StackTrace, System.Environment.NewLine);
                _result.status = false;
            }
            catch (FaultException ex)
            {
                _result.status = false;
                _result.message = String.Format("Service error occurred: {0}", ex.Message);
            }

            catch (CommunicationException ex)
            {
                _result.status = false;
                _result.message = String.Format("Communications error occurred: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                _result.status = false;
                _result.message = String.Format("Error occurred: {0}", ex.Message);
            }
            e.Result = _result;
        }
        private async void refresh_platform_worker_complete(object sender, RunWorkerCompletedEventArgs e)
        {
            WCFResultType _result = (WCFResultType)e.Result;
            if (!_result.status)
            {
                await this.ShowMessageAsync("Error while contacting MRMP Service", _result.message);
            }
            platforms_progress_indicator.Visibility = System.Windows.Visibility.Collapsed;
            platforms_progress_message.Visibility = System.Windows.Visibility.Collapsed;

            //refest platform list after platform was refreshed
            load_platformlist();
        }
    }
}
