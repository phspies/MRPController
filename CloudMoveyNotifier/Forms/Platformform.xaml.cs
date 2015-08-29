using CloudMoveyNotifier.CloudMoveyWCF;
using CloudMoveyNotifier.Models;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace CloudMoveyNotifier.Forms
{
    /// <summary>
    /// Interaction logic for Platformform.xaml
    /// </summary>
    public partial class PlatformForm : MetroWindow
    {
        public Platform _record;
        int _action;
        public PlatformForm(Platform __record, int __action)
        {
            _record = __record;
            _action = __action;
            InitializeComponent();

        }
        private void platform_vendor_changed(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(platform_vendor.SelectedValue);
            toggle_platform();
        }
        private void platform_type_changed(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(platform_type.SelectedValue);
        }
        private void toggle_platform()
        {

            if (Convert.ToInt32(platform_vendor.SelectedIndex) != -1)
            {
                if (Int16.Parse(platform_vendor.SelectedValue.ToString()) == 0)
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
            else
            {
                platform_dimensiondata_url.Visibility = Visibility.Visible;
                platform_url.Visibility = Visibility.Collapsed;
            }
        }
        private void initialize_form(object sender, RoutedEventArgs e)
        {
            platform_vendor.ItemsSource = (new Vendors()).VendorList;
            platform_dimensiondata_url.DataContext = new DimensionDataLocationViewModel(); //(new DimensionDataLocations()).LocationList;
            platform_type.ItemsSource = (new Types()).TypeList;
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
            CloudMoveyServiceClient channel = new CloudMoveyServiceClient();
            Debug.WriteLine(platform_dimensiondata_url.Text);
            string url = "https://" + _record.url;
            DatacenterListType _datacenters = channel.ListDatacenters(url, platform_username.Text, platform_password.Text);
            List<Datacenter> _viewdatacenters = new List<Datacenter>();
            _datacenters.datacenterField.ForEach(x => _viewdatacenters.Add(new Datacenter() { datacenter = x.displayNameField, moid = x.idField }));
            platform_datacenter.ItemsSource = _viewdatacenters;
        }
    }
}
