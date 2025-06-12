using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using Wpf.Ui.Controls;

namespace intsis.Views
{
    /// <summary>
    /// Логика взаимодействия для WebBrowser.xaml
    /// </summary>
    public partial class WebBrowser : Page
    {
        public WebBrowser()
        {
            InitializeComponent();
            this.Loaded += WebBrowser_Loaded;
        }

        private async void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string userDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "WiseChoice", "WebView2");

                // Убедимся, что папка существует
                Directory.CreateDirectory(userDataFolder);

                var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await WebView.EnsureCoreWebView2Async(env);

                WebView.Source = new Uri("https://wise-choice.ru");
            }
            catch (Exception ex)
            {
                var messagebox = new Wpf.Ui.Controls.MessageBox
                {
                    CloseButtonText = "Ок",
                    Title = "Ошибка",
                    Content = ex.Message
                };

                await messagebox.ShowDialogAsync();
            }
        }
    }
}
