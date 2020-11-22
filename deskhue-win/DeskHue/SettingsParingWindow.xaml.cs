using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DeskHue.Entities;
using RestSharp;

namespace DeskHue
{
    /// <summary>
    /// Interaction logic for SettingsParingWindow.xaml
    /// </summary>
    public partial class SettingsParingWindow : Window
    {
        private Action showMainWindow;
        public SettingsParingWindow(Action showMainWindow)
        {
            InitializeComponent();
            this.showMainWindow = showMainWindow;
        }

        private async void BtnPair_Click(object sender, RoutedEventArgs e)
        {
            HueIpAddress ip = null;
            
            bool autobound = false;
            if (optDiscoverAuto.IsChecked.HasValue && optDiscoverAuto.IsChecked.Value)
            {
                var ips = await HueDiscoveryService.discover();
                if (ips.Count == 1)
                {
                    ip = ips[0];
                    autobound = true;
                }
                else
                {
                    MessageBox.Show("More than one hue found. Manual discovery required.");
                    return;
                }
            }
            else if (optDiscoverBind.IsChecked.HasValue && optDiscoverBind.IsChecked.Value)
            {
                if (lstHueIps.SelectedItems.Count == 1)
                {
                    ip = (HueIpAddress) lstHueIps.SelectedItems[0];
                    autobound = false;
                }
                else
                {
                    MessageBox.Show("You must select an IP or discover automatically");
                    return;
                }
            }

            var client = new RestClient("http://" + ip);

            var request = new RestRequest("/api", Method.POST);
            var newUserRequest = new NewUserRequest();
            newUserRequest.devicetype = "DeskHue - Windows";
            request.AddJsonBody(newUserRequest);
            var response = await client.ExecuteAsync<List<NewUserResponse>>(request);

            List<NewUserResponse> items = response.Data;
            if (items.Count != 1)
            {
                MessageBox.Show("Incorrect or malformed response from hue bridge: " + response.Content);
                return;
            }

            NewUserResponse newUserResponse = items[0];
            if (newUserResponse.error != null)
            {
                MessageBox.Show(newUserResponse.error.description);
                return;
            }
            else if (newUserResponse.success != null)
            {
                HueConfig hueConfig = new HueConfig();
                hueConfig.ip = ip.internalipaddress;
                hueConfig.deviceId = ip.id;
                hueConfig.username = newUserResponse.success.username;
                hueConfig.bound = true;
                hueConfig.autobound = autobound;
                ConfigService.saveConfig(hueConfig);
                this.Close();
                showMainWindow();
            }
            else
            {
                MessageBox.Show("Incorrect or malformed response from hue bridge: " + response.Content);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!ConfigService.loadConfig().bound)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private async void OptDiscoverBind_Checked(object sender, RoutedEventArgs e)
        {
            List<HueIpAddress> r = await HueDiscoveryService.discover();

            lstHueIps.Items.Clear();
            foreach (HueIpAddress hue in r)
            {
                lstHueIps.Items.Add(hue);
            }
        }

        private void OptDiscoverAuto_Checked(object sender, RoutedEventArgs e)
        {
            lstHueIps?.Items.Clear();
        }
    }
}
