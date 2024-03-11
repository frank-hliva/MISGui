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

namespace MISGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Url.Text = load("Locations", "mainUrl");
            var stayOnTop = load("Window", "stayOnTop");
            StayOnTopCheckBox.IsChecked = Topmost = (stayOnTop == "" ? true : stayOnTop == "1");
            try
            {
                Left = Double.Parse(load("Window", "x"));
                Top = Double.Parse(load("Window", "y"));
            }
            catch (Exception ex)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        private string appName = "MISGui";
        private string localPath = "http://localhost:3000";

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                var mainUrl = new Uri(Url.Text);
                save("Locations", "mainUrl", Url.Text);
                save("Window", "x", this.Left.ToString());
                save("Window", "y", this.Top.ToString());
                save("Window", "stayOnTop", StayOnTopCheckBox.IsChecked.Value ? "1" : "0");
            }
            catch (Exception ex)
            {
            }
        }

        private string getLocalhost(Uri mainUrl)
        {
            return $"{localPath}{mainUrl.PathAndQuery}";
        }
        private string getSpace(Uri localhostUrl)
        {
            return $"{localPath}/ims/html2/admin/space.html";
        }
        private string getLH(Uri mainUrl)
        {
            return $"npm run local-dev -- --url {mainUrl.Scheme}://{mainUrl.Host}{mainUrl.LocalPath} --reload";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            try
            {
                var mainUrl = new Uri(textBox.Text);
                Localhost.Text = getLocalhost(mainUrl);
                Space.Text = getSpace(new Uri(Localhost.Text));
                LH.Text = getLH(mainUrl);
            } catch (Exception ex)
            {
                Localhost.Text = "";
                Space.Text = "";
                LH.Text = "";
            }
        }

        public string load(string store, string key)
        {
            object value = "";
            var appKey = Registry.CurrentUser.OpenSubKey($"SOFTWARE\\{appName}");
            if (appKey != null)
            {
                var valueKey = appKey.OpenSubKey(store);
                if (valueKey != null)
                {
                    value = valueKey.GetValue(key);
                    valueKey.Close();
                }
                appKey.Close();
            }
            return (string)value;
        }

        public void save(string store, string key, string value)
        {
            var appKey = Registry.CurrentUser.CreateSubKey($"SOFTWARE\\{appName}");
            var valueKey = appKey.CreateSubKey(store);
            valueKey.SetValue(key, value);
            valueKey.Close();
            appKey.Close();
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
            if (Url.Text != "")
            {
                OpenInBrowser(Url.Text);
            }
        }

        private void LocalhostOpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (Localhost.Text != "")
            {
                OpenInBrowser(Localhost.Text);
            }
        }

        private void SpaceOpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (Space.Text != "")
            {
                OpenInBrowser(Space.Text);
            }
        }

        private void CopyLH_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(LH.Text);
        }
    }
}