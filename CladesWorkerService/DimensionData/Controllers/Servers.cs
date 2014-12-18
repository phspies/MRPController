using cloudManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using cloudManage.restMethods;
using System.Windows;

namespace cloudManage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public void server_poweron(object sender, RoutedEventArgs e)
        {

            ServerDataModel _selectedServer = (ServerDataModel)serverList.SelectedItem;
            if (_selectedServer != null)
            {
                Servers poweron = new Servers();
                poweron.server_id = _selectedServer.id;
                if (poweron.serverPowerOn())
                {
                    MessageBox.Show("Sucessfully powered on " + _selectedServer.name);
                }
                else
                {
                    MessageBox.Show("Error powering on " + _selectedServer.name + ", reason: " + poweron.returnMessage);
                }
            }
        }
        public void server_poweroff(object sender, RoutedEventArgs e)
        {

            ServerDataModel _selectedServer = (ServerDataModel)serverList.SelectedItem;
            if (_selectedServer != null)
            {
                Servers poweroff = new Servers();
                poweroff.server_id = _selectedServer.id;
                if (poweroff.serverPowerOff())
                {
                    MessageBox.Show("Sucessfully powered off " + _selectedServer.name);
                }
                else
                {
                    MessageBox.Show("Error powering ff " + _selectedServer.name + ", reason: " + poweroff.returnMessage);
                }
            }
        }
        public void server_delete(object sender, RoutedEventArgs e)
        {

            ServerDataModel _selectedServer = (ServerDataModel)serverList.SelectedItem;
            if (_selectedServer != null)
            {
                Servers delete = new Servers();
                delete.server_id = _selectedServer.id;
                if (delete.serverDelete())
                {
                    MessageBox.Show("Sucessfully deleted " + _selectedServer.name);
                }
                else
                {
                    MessageBox.Show("Error deleting " + _selectedServer.name + ", reason: " + delete.returnMessage);
                }
            }
        }
    }
}