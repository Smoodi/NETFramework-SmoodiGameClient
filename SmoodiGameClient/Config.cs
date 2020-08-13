using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace SmoodiGameClient
{
    public static class Config
    {
        private static MainWindow referenceWindow;
        private static GameSelector game;

        public static string PROJECT_S_USER { get; set; } = "";
        public static bool PROJECT_S_OPTIFINE { get; set; } = false;
        public static bool PROJECT_S_LOGIN_SAVE { get; set; } = false;
        public static string JAVA_HOME { get; set; } = "C:\\Program Files\\Java\\openjdk-1.8.0.232\\bin";
        public static int MINIMUM_JAVA_RAM { get; set; } = 4;
        public static int MAXIMUM_JAVA_RAM { get; set; } = 7;
        public static string INSTALL_DIR { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SmoodiGames";
        public static string PROJECT_S_INSTALL_DIR { get; set; } = INSTALL_DIR + @"\games\minecraft\modpacks\projects\game\master\java";

        public static bool JAVA_USE_DEBUG { get; set; } = false;
        public static bool PROJECT_S_INSTALLED { get; set; } = false;
        public static DateTime PROJECT_S_LAST_UPDATE { get; internal set; }
        public static float PROJECT_S_VERSION { get; internal set; } = 0f;
        public static string TEMPORARY_FOLDER { get; internal set; } = INSTALL_DIR + @"\.temp";

        internal static void Load(MainWindow mainWindow, GameSelector g)
        {
            referenceWindow = mainWindow;
            game = g;
            if (!File.Exists(INSTALL_DIR + @"\config.xml"))
            {
                updateJavaHome();
                return;
            }
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(File.ReadAllText(INSTALL_DIR + @"\config.xml"));
            PROJECT_S_USER = xml.SelectSingleNode("/config/java/projects/user").InnerText;
            PROJECT_S_OPTIFINE = XmlConvert.ToBoolean(xml.SelectSingleNode("/config/java/projects/optifine").InnerText);
            PROJECT_S_LOGIN_SAVE = XmlConvert.ToBoolean(xml.SelectSingleNode("/config/java/projects/loginSave").InnerText);
            JAVA_HOME = xml.SelectSingleNode("/config/java/home").InnerText;
            MINIMUM_JAVA_RAM = XmlConvert.ToInt32(xml.SelectSingleNode("/config/java/minRAM").InnerText);
            MAXIMUM_JAVA_RAM = XmlConvert.ToInt32(xml.SelectSingleNode("/config/java/maxRAM").InnerText);
            JAVA_USE_DEBUG = XmlConvert.ToBoolean(xml.SelectSingleNode("/config/java/debug").InnerText);
            PROJECT_S_INSTALLED = XmlConvert.ToBoolean(xml.SelectSingleNode("/config/java/projects/isInstalled").InnerText);
            PROJECT_S_LAST_UPDATE = XmlConvert.ToDateTime(xml.SelectSingleNode("/config/java/projects/lastUpdate").InnerText, XmlDateTimeSerializationMode.Unspecified);
            PROJECT_S_VERSION = XmlConvert.ToSingle(xml.SelectSingleNode("/config/java/projects/installedVersion").InnerText);

            referenceWindow.Dispatcher.Invoke(new MainWindow.startLauncher_callback(referenceWindow.startLauncher), new object[] { game });
        }

        internal static void updateJavaHome()
        {
            try
            {
                Process p = new Process();
                // set start info
                ProcessStartInfo si = new ProcessStartInfo();
                si.FileName = "cmd";
                si.UseShellExecute = false;
                si.RedirectStandardInput = true;
                si.RedirectStandardOutput = true;
                si.WorkingDirectory = @"c:\";
                si.Arguments = "/c where java";

                p.OutputDataReceived += p_OutputDataReceived;
                p.ErrorDataReceived += p_ErrorDataReceived;

                p.StartInfo = si;

                // start process
                p.Start();
                // send command to its input
                p.BeginOutputReadLine();
            }
            catch (Exception e)
            {
                MessageBox.Show("Java home fetching failed. " + e.Message);
            }
        }

        private static void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Process p = sender as Process;
            if (p == null) return;
            MessageBox.Show("Error occured redirecting commandline data.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);

            referenceWindow.Dispatcher.Invoke(new MainWindow.startLauncher_callback(referenceWindow.startLauncher), new object[] { game });
        }

        private static void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Process p = sender as Process;
            if (p == null) return;
            if (e.Data != null && e.Data != "")
                try { JAVA_HOME = e.Data.Substring(0, e.Data.IndexOf("java.exe")-1); }
                catch (Exception ex){ MessageBox.Show("Error occured parsing java directory. " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error); }
            else
            {
                JAVA_HOME = "UNSPECIFIED";
                MessageBox.Show("An error occured trying to locate a Java installation path. Please make sure to set it manually in the settings.");
            }

            p.CancelOutputRead();

            referenceWindow.Dispatcher.Invoke(new MainWindow.startLauncher_callback(referenceWindow.startLauncher), new object[] { game });
        }

        internal static void Save()
        {
            XmlDocument xml = new XmlDocument();
            var config = xml.CreateElement("config");
            var suser = xml.CreateElement("user");
            var optifine = xml.CreateElement("optifine");
            var loginSave = xml.CreateElement("loginSave");
            var jhome = xml.CreateElement("home");
            var minr = xml.CreateElement("minRAM");
            var maxr = xml.CreateElement("maxRAM");
            var deb = xml.CreateElement("debug");
            var sinstalled = xml.CreateElement("isInstalled");
            var lastUpdate = xml.CreateElement("lastUpdate");
            var iVersion = xml.CreateElement("installedVersion");

            suser.InnerText = PROJECT_S_USER;
            optifine.InnerText = XmlConvert.ToString(PROJECT_S_OPTIFINE);
            loginSave.InnerText = XmlConvert.ToString(PROJECT_S_LOGIN_SAVE);
            jhome.InnerText = JAVA_HOME;
            minr.InnerText = XmlConvert.ToString(MINIMUM_JAVA_RAM);
            maxr.InnerText = XmlConvert.ToString(MAXIMUM_JAVA_RAM);
            deb.InnerText = XmlConvert.ToString(JAVA_USE_DEBUG);
            sinstalled.InnerText = XmlConvert.ToString(PROJECT_S_INSTALLED);
            lastUpdate.InnerText = XmlConvert.ToString(PROJECT_S_LAST_UPDATE, XmlDateTimeSerializationMode.Unspecified);
            iVersion.InnerText = XmlConvert.ToString(PROJECT_S_VERSION);

            var java = xml.CreateElement("java");
            var projects = xml.CreateElement("projects");

            xml.AppendChild(config);
            config.AppendChild(java);
            java.AppendChild(projects);
            java.AppendChild(minr);
            java.AppendChild(maxr);
            java.AppendChild(jhome);
            java.AppendChild(deb);
            projects.AppendChild(suser);
            projects.AppendChild(optifine);
            projects.AppendChild(loginSave);
            projects.AppendChild(sinstalled);
            projects.AppendChild(lastUpdate);
            projects.AppendChild(iVersion);

            xml.Save(INSTALL_DIR + @"\config.xml");
        }
    }
}
