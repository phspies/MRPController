using System;
using System.Drawing;
using MRMPNotifier.MRMPWCFService;
using System.Windows.Forms;

namespace MRMPNotifier
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MRPWCFServiceClient channel = new MRPWCFServiceClient();       
        workerInformation _information = null;
        public MainWindow()
        {
            load_information();

            NotifyIcon m_notifyIcon = new NotifyIcon();
            m_notifyIcon.Text = "MMRP Manager GUID";
            m_notifyIcon.Icon = new Icon(System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/mrp.ico")).Stream);
            m_notifyIcon.Click += new EventHandler(copy_guid_button_clicked);
            m_notifyIcon.Visible = true;
        }


        private void ExitNotifier(object Sender, EventArgs e)
        {
            this.Close();
        }
        private void copy_guid_button_clicked(object sender, EventArgs e)
        {
            String GUID = _information.agentId;
            Clipboard.SetText(GUID, TextDataFormat.Text);
        }
    }
}
