using Microsoft.WindowsAPICodePack.Dialogs;
using smoodi.mc.lib;
using smoodi.updaterLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace SmoodiGameClient
{
    /// <summary>
    /// Interaction logic for GameSelector.xaml
    /// </summary>
    public partial class GameSelector : Window
    {
        private bool isPatching = false;
        private bool shutdown = false;
        private bool unpacking = false;
        Process minecraft = null;
        smoodi.updaterLib.UpdaterLib projectslib = new smoodi.updaterLib.UpdaterLib();

        public GameSelector()
        {
            InitializeComponent();
        }

        private void Home_MouseUp(object sender, MouseButtonEventArgs e)
        {
            homeGrid.Visibility = Visibility.Visible;
            NuclearHazard.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Hidden;
            Voltz.Visibility = Visibility.Hidden;
            ProjectS.Visibility = Visibility.Hidden;
            if ((bool)Tg_Btn.IsChecked) Tg_Btn.IsChecked = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            projectslib.init("http://dl.smoodi.de/projects/patch/game/update.xml", Config.PROJECT_S_INSTALL_DIR, Config.PROJECT_S_LAST_UPDATE, Config.PROJECT_S_VERSION);

            this.javaHomeText.Text = Config.JAVA_HOME;
            this.maxRAMSlider.Value = Config.MAXIMUM_JAVA_RAM;
            this.projectSOptifineInstall.IsChecked = Config.PROJECT_S_OPTIFINE;
            this.javaDebugCheckBox.IsChecked = Config.JAVA_USE_DEBUG;

            if(Config.PROJECT_S_INSTALLED)
            {
                projectSInstall.Background = ((ImageBrush)this.FindResource("PlayBrush"));
                projectSInstall.Content = "Play";
                projectSRemove.Visibility = Visibility.Visible;
            }


        }

        internal async void projectSStart(string pn, string uid, string token)
        {
            isPatching = true;
            if (!Directory.Exists(Config.PROJECT_S_INSTALL_DIR + @"\assets"))
            {
                WebClient t = new WebClient();
                if (!Directory.Exists(Config.TEMPORARY_FOLDER)) Directory.CreateDirectory(Config.TEMPORARY_FOLDER);

                unpacking = true;
                t.DownloadFileCompleted += T_DownloadFileCompleted;
                t.DownloadFileAsync(new Uri("http://dl.smoodi.de/projects/patch/game/assets.zip"), Config.TEMPORARY_FOLDER + @"\s_assets.tmp");
            }

            progressBarSProject.Visibility = Visibility.Visible;
            progressBarSProject.Opacity = 1;

            Progress<float> validation = new Progress<float>();
            validation.ProgressChanged += Validation_ProgressChanged;
            if(!await projectslib.isUpToDate())
            {
                try
                {
                    Directory.Delete(System.AppDomain.CurrentDomain.BaseDirectory + @"\config", true);
                } catch { }
                try { Directory.Delete(Config.PROJECT_S_INSTALL_DIR + @"\config", true); } catch { }
            }

            if (Config.PROJECT_S_OPTIFINE) projectslib.markOptionalFileAsRequested(Config.PROJECT_S_INSTALL_DIR + @"\mods\Optifine.jar");
            else
            {
                if (File.Exists(Config.PROJECT_S_INSTALL_DIR + @"\mods\Optifine.jar")) try { File.Delete(Config.PROJECT_S_INSTALL_DIR + @"\mods\Optifine.jar"); } catch { }
            }

            Task<int> download = null;

            await projectslib.validateInstallation(validation);

            int pendingDownloads = projectslib.getPendingDownloads();
            if (pendingDownloads > 0)
            {

                Progress<int> files = new Progress<int>();
                progressBarSProject.Maximum = pendingDownloads;
                progressBarSProject.Visibility = Visibility.Visible;
                progressBarSProject.Opacity = 1f;
                files.ProgressChanged += Files_ProgressChanged;

                download = projectslib.downloadPendingFiles(files);

            }

            if (download != null) await download;

            if (pendingDownloads > 0 )
            {
                try
                {
                    if (Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\config")) Directory.Delete(System.AppDomain.CurrentDomain.BaseDirectory + @"\config", true);
                    Util.DirectoryCopy(Config.PROJECT_S_INSTALL_DIR + @"\config", (System.AppDomain.CurrentDomain.BaseDirectory + @"\config"), true);
                }
                catch { }
            }


            isPatching = false;
            if (shutdown && !unpacking) { Close(); return; }
            ((Storyboard)this.FindResource("FinishPatch")).Begin();

            Config.PROJECT_S_INSTALLED = true;
            Config.PROJECT_S_LAST_UPDATE = DateTime.Now;
            Config.PROJECT_S_VERSION = await projectslib.getNewPatchVersion();

            projectSInstall.Background = ((ImageBrush)this.FindResource("PlayBrush"));
            projectSInstall.Content = "Play";
            projectSRemove.Visibility = Visibility.Visible;

            projectslib = new smoodi.updaterLib.UpdaterLib();
            projectslib.init("http://dl.smoodi.de/projects/patch/game/update.xml", Config.PROJECT_S_INSTALL_DIR, Config.PROJECT_S_LAST_UPDATE, Config.PROJECT_S_VERSION);

            mclib mc = new mclib();
            string i = Config.PROJECT_S_INSTALL_DIR;
            if (!Directory.Exists(i)) Directory.CreateDirectory(i);

            string gd = i + @"\";
            string version = "1.12.2";
            string assetIndex = version;
            string assets = gd + @"assets";
            string natives = gd + "natives";
            string mainfile = version + ".jar";
            string userprop = "{}";
            string minRAM = Config.MINIMUM_JAVA_RAM.ToString() + "G";
            string maxRAM = Config.MAXIMUM_JAVA_RAM.ToString() + "G";
            string javapath = Config.JAVA_HOME;

            mc.setForgeModLoader(true);
            mc.setJavaPath(javapath);
            bool antiDebug = !Config.JAVA_USE_DEBUG;

            mc.setVisibleConsole(Config.JAVA_USE_DEBUG);

            projectSInstall.Content = "Running...";

            string cmd = mc.configMinecraftStart(gd, mclib.defaultFMLTweakClass, mclib.defaultFMLMainClass, token, natives, assets, version, pn, uid, assetIndex, mainfile, userprop, minRAM, maxRAM);
            mc.runMinecraft();
            Process mcp = mc.getMinecraftProcess();
            mcp.EnableRaisingEvents = true;
            minecraft = mcp;
            mcp.Exited += (sender, e) => { this.Dispatcher.Invoke(new projectSButtonUpdate_callback(projectSButtonUpdate), new object[] { true, true, "Play" }); minecraft = null; };
            /*
            if (File.Exists(Config.PROJECT_S_INSTALL_DIR + @"\start.cmd")) File.Delete(Config.PROJECT_S_INSTALL_DIR + @"\start.cmd");
            File.WriteAllText(Config.PROJECT_S_INSTALL_DIR + @"\start.cmd", "@echo off\njava "+cmd);
            Process.Start(Config.PROJECT_S_INSTALL_DIR + @"\start.cmd").WaitForExit();
            File.Delete(Config.PROJECT_S_INSTALL_DIR + @"\start.cmd");
            */
        }

        private delegate void projectSButtonUpdate_callback(bool isEnabled, bool isPlay, string txt);

        private void projectSButtonUpdate(bool isEnabled, bool isPlay, string txt)
        {
            projectSInstall.IsEnabled = isEnabled;
            projectSInstall.Background = (isPlay) ? ((ImageBrush)this.FindResource("PlayBrush")) : ((ImageBrush)this.FindResource("InstallBrush"));
            projectSInstall.Content = txt;
        }

        private void T_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(Config.TEMPORARY_FOLDER + @"\s_assets.tmp");
            zip.ExtractAll(Config.PROJECT_S_INSTALL_DIR);
            zip.Dispose();
            File.Delete(Config.TEMPORARY_FOLDER + @"\s_assets.tmp");
            unpacking = false;
            if (shutdown && !isPatching) Close();
        }

        internal delegate void updateHomeCallback(string header, string descr, bool button, string buttontxt = "");
        internal void UpdateHome(string header, string descr, bool button, string buttontxt = "")
        {
            HomeTitle.Content = header;
            HomeDescription.Text = descr;
            HomeButton.Visibility = button ? Visibility.Visible : Visibility.Hidden;
            HomeButton.Content = buttontxt;
        }

        internal delegate void updateProjectSCallback(string header, string descr);
        internal void UpdateProjectS(string header, string descr)
        {
            projectsTitle.Content = header;
            projectsDescription.Text = descr;
        }

        private void NuclearHazard_MouseUp(object sender, RoutedEventArgs e)
        {
            homeGrid.Visibility = Visibility.Hidden;
            NuclearHazard.Visibility = Visibility.Visible;
            Settings.Visibility = Visibility.Hidden;
            Voltz.Visibility = Visibility.Hidden;
            ProjectS.Visibility = Visibility.Hidden;
            if ((bool)Tg_Btn.IsChecked) Tg_Btn.IsChecked = false;
        }

        private void Voltz_MouseUp(object sender, RoutedEventArgs e)
        {
            homeGrid.Visibility = Visibility.Hidden;
            NuclearHazard.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Hidden;
            Voltz.Visibility = Visibility.Visible;
            ProjectS.Visibility = Visibility.Hidden;
            if ((bool)Tg_Btn.IsChecked) Tg_Btn.IsChecked = false;
        }
        private void Twitter_MouseUp(object sender, RoutedEventArgs e)
        {

            Process.Start("http://www.twitter.com/smoodifox");
            if ((bool)Tg_Btn.IsChecked) Tg_Btn.IsChecked = false;
        }

        private void Settings_MouseUp(object sender, RoutedEventArgs e)
        {
            homeGrid.Visibility = Visibility.Hidden;
            NuclearHazard.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Visible;
            Voltz.Visibility = Visibility.Hidden;
            ProjectS.Visibility = Visibility.Hidden;
            if ((bool)Tg_Btn.IsChecked) Tg_Btn.IsChecked = false;
        }

        private void ProjectS_MouseUp(object sender, RoutedEventArgs e)
        {
            homeGrid.Visibility = Visibility.Hidden;
            NuclearHazard.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Hidden;
            Voltz.Visibility = Visibility.Hidden;
            ProjectS.Visibility = Visibility.Visible;

            if ((bool)Tg_Btn.IsChecked) Tg_Btn.IsChecked = false;
        }


        private void Exit_MouseUp(object sender, RoutedEventArgs e)
        {
            if ((bool)Tg_Btn.IsChecked) Tg_Btn.IsChecked = false;
            if (!isPatching && !unpacking) Close();
            else { projectslib.requestShutdown();
                shutdown = true;
            }
        }

        private void ProjectSInstall_Click(object sender, RoutedEventArgs e)
        {
            projectSInstall.IsEnabled = false;
            ProjectSLaunchModule s = new ProjectSLaunchModule(this);
            s.ShowDialog();
        }

        private void Files_ProgressChanged(object sender, int e)
        {
            progressBarSProject.Value = e;
        }

        private void Validation_ProgressChanged(object sender, float e)
        {
            progressBarSProject.Value = (int)e;
        }

        internal void NotifyUpdateStatus(int v)
        {
            this.progressBarSProject.Value = v;
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            settingsBgMedia.Position = TimeSpan.Zero;
            settingsBgMedia.Volume = 0;
            settingsBgMedia.Play();
        }

        private void JavaHomeText_TextChanged(object sender, TextChangedEventArgs e)
        {
            Config.JAVA_HOME = javaHomeText.Text;
        }

        private void MaxRAMSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Config.MAXIMUM_JAVA_RAM = (int)maxRAMSlider.Value;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if(result == CommonFileDialogResult.Ok)
            {
                javaHomeText.Text = dialog.FileName;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (minecraft != null)
                    minecraft.Kill();
            }
            catch { }
            System.Windows.Application.Current.Shutdown();
        }

        private void JavaDebugCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Config.JAVA_USE_DEBUG = (bool)javaDebugCheckBox.IsChecked;
        }

        private void JavaDebugCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Config.JAVA_USE_DEBUG = (bool)javaDebugCheckBox.IsChecked;
        }

        private void ProjectSRemove_Click(object sender, RoutedEventArgs e)
        {
            if(minecraft != null)
            {
                MessageBox.Show("The game is currently running! Please shutdown the game first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(Config.PROJECT_S_INSTALLED)
            {
                if(Directory.Exists(Config.PROJECT_S_INSTALL_DIR))
                {
                    Directory.Delete(Config.PROJECT_S_INSTALL_DIR, true);
                }
            }
            Config.PROJECT_S_INSTALLED = false;
            projectSInstall.Background = ((ImageBrush)this.FindResource("InstallBrush"));
            projectSInstall.Content = "Install";
            projectSRemove.Visibility = Visibility.Hidden;
        }

        private void ProjectSOptifineInstall_Checked(object sender, RoutedEventArgs e)
        {
            Config.PROJECT_S_OPTIFINE = (bool)projectSOptifineInstall.IsChecked;
        }

        private void ProjectSOptifineInstall_Unchecked(object sender, RoutedEventArgs e)
        {
            Config.PROJECT_S_OPTIFINE = (bool)projectSOptifineInstall.IsChecked;
        }
    }
}
