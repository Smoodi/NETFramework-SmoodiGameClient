using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace SmoodiGameClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int installedUpdater = 0;
            if(File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+@"\SmoodiGames\updater\updaterVersion.dat")) { try
                {
                    string txt = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SmoodiGames\updater\updaterVersion.dat");
                    installedUpdater = Int32.Parse(txt);
                } catch (Exception ex) {
                    MessageBox.Show("An error occured trying to fetch online information. Application will shutdown.\nError:\n\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                } }
            WebClient d = new WebClient();
            string v = d.DownloadString("http://dl.smoodi.de/smoodigames/launcher/updaterVersion.dat");
            int nu = Int32.Parse(v);
            if(nu > installedUpdater)
            {
                this.status.Content = "Downloading / Fixing updater data... ( 0% )";
                d.DownloadFileCompleted += D_DownloadFileCompleted;
                d.DownloadProgressChanged += D_DownloadProgressChanged;

                if(!Directory.Exists(@"C:\.tmp")) Directory.CreateDirectory(@"C:\.tmp");
                d.DownloadFileAsync(new Uri("http://dl.smoodi.de/smoodigames/launcher/Install_Data.zip"), @"C:\.tmp\.update.tmp");
            }
            else
            {
                continueLoad();
            }
        }

        private void D_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.status.Content = "Downloading / Fixing updater data... ( " + e.ProgressPercentage.ToString() + " )";
        }

        private void D_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

            try {
                this.status.Content = "Installing update / fix...";
                if( Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SmoodiGames\updater"))
                    Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SmoodiGames\updater", true);
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SmoodiGames\updater");
                ZipFile zips = new ZipFile(@"C:\.tmp\.update.tmp");
                zips.ExtractAll(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SmoodiGames\updater");
                zips.Dispose();
                Directory.Delete(@"C:\.tmp", true); } catch (Exception ex) { MessageBox.Show("ERROR " + ex.Message); }

            continueLoad();
        }

        private async void continueLoad()
        {
            this.status.Content = "Updating home screen...";
            if (!Directory.Exists(Config.INSTALL_DIR)) Directory.CreateDirectory(Config.INSTALL_DIR);

            GameSelector gameSelector = new GameSelector();

            WebClient w = new WebClient();
            string w0 = await w.DownloadStringTaskAsync("http://www.smoodi.de/smoodigames/launcher/home.xml");
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.LoadXml(w0);
                string header = xml.SelectSingleNode("/root/home/title").InnerText;
                string descr = xml.SelectSingleNode("/root/home/description").InnerText;
                bool button = XmlConvert.ToBoolean(xml.SelectSingleNode("/root/home/button").Attributes[0].Value);
                string buttontxt = xml.SelectSingleNode("/root/home/button").InnerText;
                gameSelector.HomeTitle.Dispatcher.Invoke(new GameSelector.updateHomeCallback(gameSelector.UpdateHome), new object[] { header, descr, button, buttontxt });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }

            w0 = await w.DownloadStringTaskAsync("http://www.smoodi.de/smoodigames/launcher/projects.xml");
            xml = new XmlDocument();
            try
            {
                xml.LoadXml(w0);
                string header = xml.SelectSingleNode("/root/home/title").InnerText;
                string descr = xml.SelectSingleNode("/root/home/description").InnerText;
                gameSelector.HomeTitle.Dispatcher.Invoke(new GameSelector.updateProjectSCallback(gameSelector.UpdateProjectS), new object[] { header, descr });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }

            Config.Load(this, gameSelector);
        }

        public delegate void startLauncher_callback(GameSelector gameSelector);
        public void startLauncher(GameSelector gameSelector)
        {
            gameSelector.Show();
            Close();
        }

    }
}
