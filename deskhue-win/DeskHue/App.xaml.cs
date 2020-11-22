using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using DeskHue.Entities;

namespace DeskHue
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private bool _isExit;
        private SettingsParingWindow settingsParingWindow;
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow();
            MainWindow.Closing += MainWindow_Closing;
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.DoubleClick += (s, args) => ShowMainWindow();
            _notifyIcon.Icon = DeskHue.Properties.Resources.LightBulb;
            _notifyIcon.Visible = true;

            CreateContextMenu();
            MainWindow.WindowState = WindowState.Minimized;
            MainWindow.Show();

            HueConfig config = ConfigService.loadConfig();
            if (!config.bound)
            {
                settingsParingWindow = new SettingsParingWindow(ShowMainWindow);
                settingsParingWindow.Show();
            } else if (config.autobound)
            {
                var ips = await HueDiscoveryService.discover();
                HueIpAddress matchedIp = null;
                try
                {
                    matchedIp = ips.First(i => i.id == config.deviceId);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show("Paired bridge not found. Re-pairing");
                    ConfigService.saveConfig(new HueConfig());
                    settingsParingWindow = new SettingsParingWindow(ShowMainWindow);
                    settingsParingWindow.Show();
                }

                if (matchedIp != null)
                {
                    config.ip = matchedIp.internalipaddress;
                    ConfigService.saveConfig(config);
                }
            }
        }

        private void CreateContextMenu()
        {
            _notifyIcon.ContextMenuStrip =
                new System.Windows.Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("MainWindow...").Click += (s, e) => ShowMainWindow();
            _notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => ExitApplication();
        }

        

        public void ShowMainWindow()
        {
            if (MainWindow.IsVisible)
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                {
                    MainWindow.WindowState = WindowState.Normal;
                }
                MainWindow.Activate();
            }
            else
            {
                MainWindow.Show();
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                MainWindow.Hide(); // A hidden window can be shown again, a closed one not
            }
        }

        private void ExitApplication()
        {
            _isExit = true;
            MainWindow.Close();
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }
    }
}
