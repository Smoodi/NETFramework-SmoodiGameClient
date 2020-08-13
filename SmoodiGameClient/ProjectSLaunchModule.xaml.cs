using Newtonsoft.Json.Linq;
using smoodi.mojang.util;
using System;
using System.Collections.Generic;
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

namespace SmoodiGameClient
{
    /// <summary>
    /// Interaction logic for ProjectSLaunchModule.xaml
    /// </summary>
    public partial class ProjectSLaunchModule : Window
    {
        mojangUtil util = new mojangUtil();
        private string accesstoken;
        private string playername;
        private string responseString;

        private GameSelector gameSelector;

        public ProjectSLaunchModule(GameSelector gameSelector)
        {
            this.gameSelector = gameSelector;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            getMinecraftAuthenticationString(usernameTxt.Text, passwordTxt.Password);

            bool success = checkAuthentification();

            string pn = getPlayername();
            string uid = util.getMinecraftUUID(pn);
            string token = getAccessToken();

            if(success)
            {
                if ((bool)cSaveBox.IsChecked) Config.PROJECT_S_USER = usernameTxt.Text;
                gameSelector.projectSStart(pn, uid, token);
                ((Storyboard)this.FindResource("LoginSuccessful")).Begin();
            }
            else ((Storyboard)this.FindResource("LoginFailed")).Begin();
        }

        #region Requirements
        public void getMinecraftAuthenticationString(string username, string password)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://authserver.mojang.com/authenticate");
            string postData = @"{ ""agent"": { ""name"": ""Minecraft"", ""version"": 1 }, ""username"":" + '"' + username + '"' + ", " + '"' + "password" + '"' + ": " + '"' + password + '"' + " }";
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch { }
        }
        public bool checkAuthentification()
        {
            try
            {
                var obj = JObject.Parse(responseString);
                accesstoken = (string)obj["accessToken"];
                playername = (string)obj["selectedProfile"]["name"];
                return true;
            }
            catch
            {
                return false;
            }
        }
        public string getAccessToken()
        {
            return accesstoken;
        }
        public string getPlayername()
        {
            return playername;
        }
        #endregion

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            Close();
        }

        private void ProjectSInstall_Click(object sender, RoutedEventArgs e)
        {
            gameSelector.projectSInstall.IsEnabled = true;
            Close();
        }

        private void MCwindow_Loaded(object sender, RoutedEventArgs e)
        {
            usernameTxt.Text = Config.PROJECT_S_USER;
            cSaveBox.IsChecked = Config.PROJECT_S_LOGIN_SAVE;
        }

        private void CSaveBox_Checked(object sender, RoutedEventArgs e)
        {
            Config.PROJECT_S_LOGIN_SAVE = (bool)cSaveBox.IsChecked;
        }

        private void CSaveBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Config.PROJECT_S_LOGIN_SAVE = (bool)cSaveBox.IsChecked;
        }
    }
}
