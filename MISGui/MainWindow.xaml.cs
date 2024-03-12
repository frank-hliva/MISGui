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
        private readonly App.Context appContext;
        private readonly IBasicStorage windowStorage;
        private readonly IBasicStorage locationsStorage;

        public MainWindow(App.Context appContext, IBasicStorage windowStorage, IBasicStorage locationsStorage)
        {
            InitializeComponent();
            this.appContext = appContext;
            this.windowStorage = windowStorage;
            this.locationsStorage = locationsStorage;

            UrlTextBox.Text = locationsStorage.Read("mainUrl");
            var stayOnTop = windowStorage.Read("stayOnTop");
            StayOnTopCheckBox.IsChecked = Topmost = (stayOnTop == "" ? true : stayOnTop == "1");
            try
            {
                Left = Double.Parse(windowStorage.Read("x"));
                Top = Double.Parse(windowStorage.Read("y"));
            }
            catch (Exception ex)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                locationsStorage.Write("mainUrl", UrlTextBox.Text);
                windowStorage.Write("x", this.Left.ToString());
                windowStorage.Write("y", this.Top.ToString());
                windowStorage.Write("stayOnTop", StayOnTopCheckBox.IsChecked.Value ? "1" : "0");
            }
            catch (Exception ex)
            {
            }
        }

        private string getLocalhost(Uri mainUrl)
        {
            return $"{appContext.LocalhostPath}{mainUrl.PathAndQuery}";
        }
        private string getSpace(Uri localhostUrl)
        {
            return $"{appContext.LocalhostPath}/ims/html2/admin/space.html";
        }
        private string getLH(Uri mainUrl)
        {
            return $"npm run local-dev -- --url {mainUrl.Scheme}://{mainUrl.Host}{mainUrl.LocalPath} --reload";
        }

        private void UrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            try
            {
                var mainUrl = new Uri(textBox.Text);
                LocalhostTextBox.Text = getLocalhost(mainUrl);
                SpaceTextBox.Text = getSpace(new Uri(LocalhostTextBox.Text));
                LHTextBox.Text = getLH(mainUrl);
            } catch (Exception ex)
            {
                LocalhostTextBox.Text = "";
                SpaceTextBox.Text = "";
                LHTextBox.Text = "";
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
            Clipboard.SetText(LHTextBox.Text);
        }
    }
}