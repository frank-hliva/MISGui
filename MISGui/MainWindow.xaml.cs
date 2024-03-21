using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Win32;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using MIS;
using System;
using MIS.Utils;
using System.Diagnostics.Tracing;
using static Microsoft.FSharp.Core.ByRefKinds;
using System.Text.RegularExpressions;
using MIS.Storages;

namespace MISGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MIS.App.Context appContext;
        private readonly IBasicStorage windowStorage;
        private readonly IBasicStorage locationsStorage;
        private readonly LocationBuilder locationBuilder;

        private static readonly ResourceDictionary resourceDictionary = new ResourceDictionary {
            Source = new Uri("Images/Resources.xaml", UriKind.Relative) 
        };
        private readonly BitmapImage START_ICON = resourceDictionary["StartIcon"] as BitmapImage;
        private readonly BitmapImage STOP_ICON = resourceDictionary["StopIcon"] as BitmapImage;

        static BitmapImage resourceToBitmap(string uri)
        {
            return new BitmapImage(new Uri(uri));
        }

        public MainWindow(
            MIS.App.Context appContext,
            WindowStorage windowStorage,
            LocationsStorage locationsStorage,
            LocationBuilder locationBuilder
        )
        {
            InitializeComponent();
            this.appContext = appContext;
            this.windowStorage = windowStorage;
            this.locationsStorage = locationsStorage;
            this.locationBuilder = locationBuilder;

            UrlTextBox.Text = locationsStorage.GetValue("mainUrl");
            var stayOnTop = windowStorage.GetValue("stayOnTop");
            StayOnTopCheckBox.IsChecked = Topmost = (stayOnTop == "" ? true : stayOnTop == "y");
            UpdateWSLCommandStatus();
            //StartLocalhostCommandButton.Content = START;
            try
            {
                Left = Double.Parse(windowStorage.GetValue("x"));
                Top = Double.Parse(windowStorage.GetValue("y"));
            }
            catch (Exception ex)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (localhostWSLRunningCommand != null)
            {
                StopWSLCommand();
            }
            try
            {
                locationsStorage.SetValue("mainUrl", UrlTextBox.Text);
                windowStorage.SetValue("x", this.Left.ToString());
                windowStorage.SetValue("y", this.Top.ToString());
                windowStorage.SetValue("stayOnTop", StayOnTopCheckBox.IsChecked.Value ? "y" : "n");
            }
            catch (Exception ex)
            {
            }
        }

        private void UrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            try
            {
                var misLocations = locationBuilder.GetAllLocationsFor(sourceUrl: textBox.Text);
                LocalhostTextBox.Text = misLocations.Localhost.ToString();
                SpaceTextBox.Text = misLocations.Space.ToString();
                LocalhostCommandTextBox.Text = misLocations.RunLocalhostCommand;
            } catch (Exception ex)
            {
                LocalhostTextBox.Text = "";
                SpaceTextBox.Text = "";
                LocalhostCommandTextBox.Text = "";
            }
        }

        private void StayOnTopCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var stayOnTopCheckBox = sender as CheckBox;
            Topmost = stayOnTopCheckBox.IsChecked.Value;
        }
        private void OpenInBrowser(string url)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
        }

        private void UrlOpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (UrlTextBox.Text != "")
            {
                OpenInBrowser(UrlTextBox.Text);
            }
        }

        private void LocalhostOpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (LocalhostTextBox.Text != "")
            {
                OpenInBrowser(LocalhostTextBox.Text);
            }
        }

        private void SpaceOpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (SpaceTextBox.Text != "")
            {
                OpenInBrowser(SpaceTextBox.Text);
            }
        }

        private void CopyLocalCommandButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(LocalhostCommandTextBox.Text);
        }

        const string START = "Start";
        const string STOP = "Stop";

        MIS.WSLRunningCommand localhostWSLRunningCommand = null;

        private void StopWSLCommand()
        {
            if (IsStartedWSLCommand)
            {
                localhostWSLRunningCommand.Stop();
                localhostWSLRunningCommand = null;
            }
        }

        void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            StopWSLCommand();
        }

        public bool IsStartedWSLCommand { get { return localhostWSLRunningCommand != null; } }

        private void Log(string output, Brush color = null, string heading = "MIS", Brush headingColor = null)
        {
            if (output != null && output != "")
            {
                new Inline[]
                {
                    new Run(DateTime.Now.ToString("HH:mm:ss")) { Foreground = Brushes.Gray },
                    new Run(" ["),
                    new Run(heading) { Foreground = headingColor ?? Brushes.LightGreen },
                    new Run("] "),
                    new Run(output) { Foreground = color ?? Brushes.LightGray },
                    new LineBreak()
                }.ForEach(ConsoleOutputTextBox.Inlines.Add);
            }
        }

        void UpdateWSLCommandStatus(bool localhostScriptRunning = false)
        {
            var isStartedWSL = IsStartedWSLCommand;
            StartLocalhostCommandIcon.Source = isStartedWSL ? STOP_ICON : START_ICON;
            StartLocalhostCommandButton.IsChecked = isStartedWSL;
            StartLocalhostCommandButton.IsEnabled = LocalhostCommandTextBox.Text != String.Empty;

            isStartedWSL = isStartedWSL && localhostScriptRunning;

            LocalhostOpenButton.IsEnabled = isStartedWSL;
            LocalhostOpenIcon.Opacity = isStartedWSL ? 1 : 0.5;

            SpaceOpenButton.IsEnabled = isStartedWSL;
            SpaceOpenIcon.Opacity = isStartedWSL ? 1 : 0.5;
        }

        private void StartLocalhostCommandButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsStartedWSLCommand)
            {
                Log("Stopping WSL...");
                StopWSLCommand();
                Log("WSL localhost Stopped.", color: Brushes.White);
            }
            else
            {
                var endOfOutputRegex = new Regex("Proxy to.*listening at");
                Log("Starting WSL...", color: Brushes.White);
                var wslCommand = new MIS.WSLCommand(LocalhostCommandTextBox.Text);
                wslCommand.OutputDataReceived.AddHandler((sender, eventArgs) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        var receivedData = eventArgs.Data;
                        Log(receivedData, heading: "WSL");
                        CommandLineScrollViewer.ScrollToBottom();
                        if (receivedData != null && endOfOutputRegex.Match(receivedData).Success)
                        {
                            UpdateWSLCommandStatus(localhostScriptRunning: true);
                            Log("WSL localhost Started.", color: Brushes.White);
                        }
                    });
                });
                localhostWSLRunningCommand = wslCommand.Start();
            }
            UpdateWSLCommandStatus();
        }

        private void LocalhostCommandTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWSLCommandStatus(LocalhostTextBox.IsEnabled);
        }
    }
}