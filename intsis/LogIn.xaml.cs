using System;
using System.Net.Http;
using System.Text;
using System.Windows;
using Newtonsoft.Json;

namespace intsis
{
    /// <summary>
    /// Логика взаимодействия для LogIn.xaml
    /// </summary>
    public partial class LogIn : Window
    {
        private readonly HttpClient _httpClient;

        public LogIn()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginTextBox.Text;
            string password = PasswordBox.Password;

            var loginData = new
            {
                username = username,
                password = password
            };

            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{Properties.Settings.Default.RestAPI_URL}api/login/", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<UserResponse>(jsonResponse); // Пример класса UserResponse

                MessageBox.Show($"ID пользователя: {user.id}"); // Здесь вы получаете ID пользователя

                // Дальнейшая логика с ID пользователя
            }
            else
            {
                MessageBox.Show("Ошибка авторизации");
            }
        }

        // Создайте класс для десериализации ответа
        public class UserResponse
        {
            public int id { get; set; }
            public string username { get; set; }
            public string email { get; set; }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для регистрации
            MessageBox.Show("Открыть форму регистрации.");
        }
    }
}