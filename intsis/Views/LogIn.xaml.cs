﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;
using Wpf.Ui.Appearance;
using System.Net.Http;
using System.Text;
using System.Net.PeerToPeer;
using System.Collections.Generic;

namespace intsis.Views
{
    /// <summary>
    /// Логика взаимодействия для LogIn.xaml
    /// </summary>
    public partial class LogIn : Page
    {

        public LogIn()
        {
            InitializeComponent();
            LoginTextBox.Focus();
            LoginButton.Background = ApplicationAccentColorManager.PrimaryAccentBrush;
            RememberMe.IsChecked=Properties.Settings.Default.RemembeR;
            LoginTextBox.Text=Properties.Settings.Default.Login;
            PasswordBox.Password = Properties.Settings.Default.Password;

        }
        public static object  ExpSysForEx {  get; set; }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                string username = LoginTextBox.Text;
                string password = PasswordBox.Password;
                string url = "https://wise-choice.ru/api/import/";
            if (username != "" && password != "")
            {
                var content =new
                {
                    username = username,
                    password = password,
                    file = ExpSysForEx
                };
                var contentJSON = JsonConvert.SerializeObject(content,Formatting.Indented);
                var data = new StringContent(contentJSON, Encoding.UTF8, "application/json");
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var response = await httpClient.PostAsync(url, data);

                        // Чтение ответа как строки
                        var result = await response.Content.ReadAsStringAsync();

                        // Десериализация JSON-ответа
                        var responseObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

                        string message;
                        if (responseObject != null && responseObject.ContainsKey("message"))
                        {
                            message = responseObject["message"];
                        }
                        else
                        {
                            message = "Не удалось получить сообщение от сервера.";
                        }

                        // Показываем сообщение
                        var messagebox = new Wpf.Ui.Controls.MessageBox
                        {
                            CloseButtonText = "Ок",
                            Title = "Экспорт",
                            Content = message
                        };
                        await messagebox.ShowDialogAsync();
                       var navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
                        navigateView.GoBack();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка: " + ex.Message);
                    }
                }
                    if (RememberMe.IsChecked == true)
                    {
                        Properties.Settings.Default.RemembeR = Convert.ToBoolean(RememberMe.IsChecked);
                        Properties.Settings.Default.Login = username;
                        Properties.Settings.Default.Password = password;
                        Properties.Settings.Default.Save();

                    }
                    //var navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
                    //navigateView.Navigate(typeof(MainWindow));
                   
                    
                    

            }
            else
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Ошибка", Content = "Оба поля авторизации должны быть заполнены!" };
                messagebox.ShowDialogAsync();
            }
        }


        

        private void LoginTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) 
            {
                PasswordBox.Focus(); 
                e.Handled = true; 
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Вызываем обработчик Click кнопки
                LoginButton_Click(LoginButton, new RoutedEventArgs());
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Определяем максимальные и минимальные размеры
            double minWidth = 800;  // минимальная ширина для нормального вида
            double minHeight = 600; // минимальная высота для нормального вида
           

            // Рассчитываем коэффициент масштаба в зависимости от размера окна
            double scaleFactor = Math.Min(e.NewSize.Width / minWidth, e.NewSize.Height / minHeight);

            // Ограничиваем минимальный и максимальный коэффициент масштаба
            scaleFactor = Math.Max(0.5, scaleFactor);  // минимальный размер 50%
            scaleFactor = Math.Min(1.0, scaleFactor);  // максимальный размер 150%

            // Применяем масштаб к элементам
            scaleTransform.ScaleX = scaleFactor;
            scaleTransform.ScaleY = scaleFactor;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.RemembeR == true)
            {
                LoginTextBox.Text = Properties.Settings.Default.Login;
                PasswordBox.Password = Properties.Settings.Default.Password;
                RememberMe.IsChecked = Properties.Settings.Default.RemembeR;
                LoginButton_Click(LoginButton, new RoutedEventArgs());
            }
        }
    }
}