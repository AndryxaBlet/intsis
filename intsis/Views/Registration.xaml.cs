﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui.Appearance;
using static intsis.Views.MainWindow;

namespace intsis.Views
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Page
    {
        
        public Registration()
        {
            InitializeComponent();
            RegisterButton.Background = GlobalDATA.Accent;
            bool First = GlobalDATA.IsFirst;
            if (First)
            FirstAdmin();
        }
        bool isAdmin = false;
        private void FirstAdmin()
        {
                isAdmin = true;
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Предупреждение", Content = "Ваш аккаунт первый в системе, вы автоматически назначенны администратором, запомните ваши данные аккаунта." };
                messagebox.ShowDialogAsync();
                GlobalDATA.IsFirst =false;
               
        }
       


        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string username = LoginTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;


            if (string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Предупреждение", Content = "Все поля должны быть заполнены." };
                messagebox.ShowDialogAsync();
                return;
            }


            if (password != confirmPassword)
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Предупреждение", Content = "Пароли не совпадают!" };
                messagebox.ShowDialogAsync();
                return;
            }


            var existingUser = ExpertSystemV2Entities.GetContext().Users.FirstOrDefault(u => u.Login == username);

            if (existingUser != null)
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Предупреждение", Content = "Пользователь с таким логином или электронной почтой уже существует!" };
                messagebox.ShowDialogAsync();
                return;
            }
            Users newUser;
            if (isAdmin)
            {
                newUser = new Users
                {

                    Login = username,
                    Name = username,
                    Password = password,
                    Status = "Admin"

                };
            }
            else
            {
                 newUser = new Users
                {
 
                    Login = username,
                    Name = username,
                    Password = password,
                    Status = "User" // Set to false by default; adjust as needed

                };
            }

            ExpertSystemV2Entities.GetContext().Users.Add(newUser);
            ExpertSystemV2Entities.GetContext().SaveChanges();

            var messageboxx =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Content = "Регистрация успешна!" };
            messageboxx.ShowDialogAsync();

            // Optionally, open the main window after registration

            GlobalDATA.recvadmin = newUser.Status;
            var navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
            navigateView.Navigate(typeof(MainWindow));

           
        }

        private void BackToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            var navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
            navigateView.GoBack();
        }

        private void EmailTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // Проверяем, нажата ли клавиша Enter
            {
                LoginTextBox.Focus(); // Передаем фокус на кнопку
                e.Handled = true;
            }
        }

        private void LoginTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // Проверяем, нажата ли клавиша Enter
            {
                PasswordBox.Focus(); // Передаем фокус на кнопку
                e.Handled = true;
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // Проверяем, нажата ли клавиша Enter
            {
                ConfirmPasswordBox.Focus(); // Передаем фокус на кнопку
                e.Handled = true;
            }
        }

        private void ConfirmPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // Проверяем, нажата ли клавиша Enter
            {
                RegisterButton.Focus(); // Передаем фокус на кнопку
                e.Handled = true;
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
    }
}
