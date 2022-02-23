using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppMeteoWpf
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {
        public string baseApi = "http://goweather.herokuapp.com/weather/";
        public string city = "Paris";
        public string jsonString;

        public string newsApi = "https://newsapi.org/v2/everything?apiKey=e39fc23b1aeb497594c4be6d1fc766cc&language=fr&pageSize=10&q=";
        public string newsJsonString;
        public string[] newsUrl = new string [10];
        public MainWindow()
        {
            InitializeComponent();
            SetDay();
            GetApiResponse();
            SetUiInfos();
            DisplayNews();
        }


        // Fonction API / JSON
        public void GetApiResponse()
        {
            using (WebClient webClient = new WebClient())
            {
                // Téléchargement du JSON depuis l'API
                jsonString = webClient.DownloadString(baseApi + city);
                // Encodage UTF8 de la réponse
                byte[] bytes = Encoding.Default.GetBytes(jsonString);
                jsonString = Encoding.UTF8.GetString(bytes);
                SetUiInfos();
            }

        }

        public void SetUiInfos()
        {
            JObject jo =  JObject.Parse(jsonString);
            //MessageBox.Show(jsonString);
            weatherDesc.Content = jo["description"];
            tempText.Content = jo["temperature"].ToString().Replace(" ", "");
            windText.Content = jo["wind"];
            // Vérifier la météo
            CheckWeatherDesc(jo);

            tempDay1.Content = jo["forecast"][0]["temperature"].ToString().Replace(" ", "");
            tempDay2.Content = jo["forecast"][1]["temperature"].ToString().Replace(" ", "");
            tempDay3.Content = jo["forecast"][2]["temperature"].ToString().Replace(" ", "");
        }

        public void CheckWeatherDesc(JObject jo)
        {
            if (jo["description"].ToString().Contains("cloud"))
            {
                //MessageBox.Show("cloud");
            }
            if (jo["description"].ToString().Contains("sun"))
            {
                //MessageBox.Show("sun");
            }
            if (jo["description"].ToString().Contains("rain"))
            {
                SetHeaderImg();
            }
        }

        // Fil d'actualités
        public void DisplayNews()
        {

            using (WebClient webClient = new WebClient())
            {
                newsJsonString = webClient.DownloadString(newsApi + city);
                byte[] bytes = Encoding.Default.GetBytes(newsJsonString);
                newsJsonString = Encoding.UTF8.GetString(bytes);
            }

            JObject jo = JObject.Parse(newsJsonString);
            List<Article> articles = new List<Article>();

            for (int i =0; i < jo["articles"].Count(); i++)
            {
                string articleTitle = jo["articles"][i]["title"].ToString();
                articles.Add(new Article() { title = articleTitle });
                newsUrl[i] = jo["articles"][i]["url"].ToString();
            }
            
            articlesList.ItemsSource = articles;

        }
        public void SetDay()
        {    
            dateText.Content= CapitalizedStr(DateTime.Now.ToString("dddd, dd MMMM yyyy"));

            forecastDay1.Content = CapitalizedStr(DateTime.Now.AddDays(1).ToString("ddd"));
            forecastDay2.Content = CapitalizedStr(DateTime.Now.AddDays(2).ToString("ddd"));
            forecastDay3.Content = CapitalizedStr(DateTime.Now.AddDays(3).ToString("ddd"));
        }


        public string CapitalizedStr(string str)
        {
            string strFormat = str;
            strFormat = char.ToUpper(str[0]) + str.Substring(1);
            return strFormat;
        }
        public void SetHeaderImg()
        {
            string weather = "rain";

            if (weather == "rain")
            {
                // Permet de modifier l'image du header
                headerImg.Source = new BitmapImage(new Uri(@"Img/rain.jpg", UriKind.Relative));
            }
            else
            {
                // Pas de modification
            }
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            city = cityText.Text;
            cityName.Content = city;
            GetApiResponse();
            DisplayNews();
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Développé par Stéphane Jaouen", "Crédits", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void articlesList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var index = articlesList.SelectedIndex;
            // MessageBox.Show("Index = " + index);
            System.Diagnostics.Process.Start(newsUrl[index]);
        }
    }

    public class Article
    {
        public string title { get; set; }

        public override string ToString()
        {
            return "- " + this.title;
        }
    }
}

