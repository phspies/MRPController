using System;
using System.Threading;
using System.Windows;

namespace MRMPNotifier
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void ApplicationStart(object sender, StartupEventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
        }
    }

}
