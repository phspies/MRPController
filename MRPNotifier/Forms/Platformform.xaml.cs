using MRPNotifier.Models;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MRPNotifier.MRPWCFService;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MRPNotifier.Forms
{
    /// <summary>
    /// Interaction logic for Platformform.xaml
    /// </summary>
    public partial class PlatformForm : MetroWindow
    {
        public Platform _record;
        private int _action;
        public List<Credential> _credentials;
        public PlatformForm(Platform __record, int __action, List<Credential> __credentials)
        {
            _credentials = __credentials;
            _record = __record;
            InitializeComponent();
            initialize_form();
            _action = __action;
            switch (__action)
            {
                case 0:
                    this.Title = "Add Platform";
                    break;
                case 1:
                    this.Title = "Update Platform";
                    platform_vendor.IsEnabled = false;
                    break;
            }
            toggle_platform();
        }
        private void platform_credential_changed(object sender, SelectionChangedEventArgs e)
        {
            toggle_platform();
        }
        private void platform_vendor_changed(object sender, SelectionChangedEventArgs e)
        {
            toggle_platform();
        }
        private void toggle_platform()
        {
            if (_record.vendor == 0)
            {
                platform_dimensiondata_url.Visibility = Visibility.Visible;
                platform_url.Visibility = Visibility.Collapsed;
            }
            else
            {
                platform_dimensiondata_url.Visibility = Visibility.Collapsed;
                platform_url.Visibility = Visibility.Visible;
            }
        }
        private void initialize_form()
        {
            platform_credential.ItemsSource = _credentials.FindAll(x => x.credential_type == 0);
            platform_vendor.ItemsSource = (new Vendors()).VendorList;
            platform_dimensiondata_url.ItemsSource = (new DimensionDataLocations()).LocationList;
            this.DataContext = _record;
            toggle_platform();
        }


        private void accept_platform_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void cancel_platform_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void test_connection_button_Click(object sender, RoutedEventArgs e)
        {
            using (new WaitCursor())
            {
                try
                {
                    message.Text = "Testing Credentials";

                    MRPWCFServiceClient channel = new MRPWCFServiceClient();
                    Tuple<bool, string> _response = channel.Login(_record.url, _credentials.Find(x => x.id == _record.credential_id), _record.vendor);
                    if (_response.Item1)
                    {
                        List<Platform> _viewdatacenters = channel.ListDatacenters(_record.url, _credentials.Find(x => x.id == _record.credential_id), _record.vendor);
                        platform_datacenter.ItemsSource = _viewdatacenters;
                        _record.passwordok = 1;
                        message.Foreground = new SolidColorBrush(Colors.Green);
                        message.Text = String.Format("Found {0} Datacenters", _viewdatacenters.Count);

                    }
                    else
                    {
                        message.Text = _response.Item2;
                        message.Foreground = new SolidColorBrush(Colors.Red);
                        _record.passwordok = 0;
                    }

                    //show info
                }
                catch (Exception ex)
                {
                    message.Text = ex.Message;
                }
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
    }
}
