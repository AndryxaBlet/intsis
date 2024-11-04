﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
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
using System.Security.Cryptography;

namespace intsis
{
    /// <summary>
    /// Логика взаимодействия для LogIn.xaml
    /// </summary>
    public partial class LogIn : Window
    {

        public LogIn()
        {
            InitializeComponent();
            intsisEntities.GetContext().Database.Connection.ConnectionString = connect;
            LoginTextBox.Focus();
            var users = intsisEntities.GetContext().User.FirstOrDefault();
            if (users == null)
            {
                Registration reg = new Registration(true);
                this.Close();
                reg.Show();
                
            }
        }
        string connect = Properties.Settings.Default.NotebookSQL;

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                string username = LoginTextBox.Text;
                string password = PasswordBox.Password;
            if (username != "" && password != "")
            {

                var login = intsisEntities.GetContext().User.FirstOrDefault(l => l.Login == username && l.Password == password);

                if (login != null)
                {
                    MainWindow window = new MainWindow(login.IsAdmin);
                    window.Show();
                    this.Close();

                }
                else
                {
                    MessageBox.Show("Неверно введены данные аккаунта!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Оба поля авторизации должны быть заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //}
            //catch (Exception r)
            //{
            //    MessageBox.Show(r.Message);

            //}
        }


        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                Registration registration = new Registration(false);
                registration.Show();
                this.Close();
            //}
            //catch (Exception r)
            //{
            //    MessageBox.Show(r.Message);

            //}
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

        private void LoginButton_Click(object sender, KeyEventArgs e)
        {

        }
    }
}