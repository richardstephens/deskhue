using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DeskHue.Entities;
using DeskHue.Services;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serialization.Json;

namespace DeskHue
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HotkeyHandler hotkeyHandler;

        public MainWindow()
        {
            InitializeComponent();

            this.hotkeyHandler = new HotkeyHandler();
            this.hotkeyHandler.adjustBrightness = (i) =>  AdjustBrightness(i);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            hotkeyHandler.OnSourceInitialized(e, this);
        }

        protected override void OnClosed(EventArgs e)
        {
            hotkeyHandler.OnClosed(e, this);
            base.OnClosed(e);
        }


        private void AdjustBrightness(Int32 offset, string lightId = null)
        {
            HueConfig config = ConfigService.loadConfig();
            if (lightId == null)
            {
                lightId = config.lightId;
            }
            if (config.ip != null && lightId != null && config.username != null)
            {
                // first get current brightness
                var client = new RestClient("http://" + config.ip);
                var getRequest = new RestRequest("/api/" + config.username + "/lights/"+lightId);
                var r = client.Execute<LightEntity>(getRequest, Method.GET);

                //now issue update
                var newBrightnes = r.Data.state.bri + offset;
                var updateBody = new HueStateUpdate();
                updateBody.bri = newBrightnes;
                var updateRequest = new RestRequest("/api/" + config.username + "/lights/" + lightId + "/state");
                updateRequest.AddJsonBody(updateBody);
                client.Execute(updateRequest, Method.PUT);
            }
        }

        async void LoadLightList()
        {
            var lights = await new HueDeviceService(ConfigService.loadConfig()).ListLights();
            lstLights.SelectedItem = null;
            lstLights.Items.Clear();
            string selected = ConfigService.loadConfig().lightId;
            foreach (var light in lights)
            {
                lstLights.Items.Add(light);
                if (selected == light.id)
                {
                    lstLights.SelectedItem= light;
                }
            }
        }
        
        
        private void CmdReload_Click(object sender, RoutedEventArgs e)
        {
            LoadLightList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadLightList();
        }

        private static readonly Regex Numericregex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private void TxtOffset_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Numericregex.IsMatch(e.Text);
        }

        private void CmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (lstLights.SelectedItems.Count == 1)
            {
                HueConfig config = ConfigService.loadConfig();
                config.lightId = ((LightEntity)lstLights.SelectedItems[0]).id;
                config.offset = Int32.Parse(txtOffset.Text);
                ConfigService.saveConfig(config);
            }
        }

        private void CmdUp_Click(object sender, RoutedEventArgs e)
        {
            if (lstLights.SelectedItems.Count == 1)
            {
                LightEntity light = (LightEntity) lstLights.SelectedItems[0];
                AdjustBrightness(Int32.Parse(txtOffset.Text), light.id);
            }
        }

        private void CmdDown_Click(object sender, RoutedEventArgs e)
        {
            if (lstLights.SelectedItems.Count == 1)
            {
                LightEntity light = (LightEntity) lstLights.SelectedItems[0];
                AdjustBrightness(Int32.Parse(txtOffset.Text)*-1, light.id);
            }
        }
    }
}
