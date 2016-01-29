using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CloudMRPNotifier
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void ApplicationStart(object sender, StartupEventArgs e)
        {
            SplashWindow splash = new SplashWindow();
            splash.Show();
            MainWindow mainwindow = new MainWindow();
            this.Dispatcher.Invoke((Action)(() =>
            {
                mainwindow = new MainWindow();
                mainwindow.Show();
                mainwindow.Activate();
            }));
            while (true)
            {
                if (mainwindow.IsLoaded)
                {
                    splash.Close();
                    break;
                }
                Thread.Sleep(1000);
            }
        }
    }

}
