using System;
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
        }
        string connect = Properties.Settings.Default.NotebookSQL2;

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                string username = LoginTextBox.Text;
                string password = PasswordBox.Password;

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
            //}
            //catch (Exception r)
            //{
            //    MessageBox.Show(r.Message);

            //}
        }


        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Registration registration = new Registration();
                registration.Show();
                this.Close();
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

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
                LoginButton.Focus(); // Передаем фокус на кнопку
                e.Handled = true; // Устанавливаем Handled, чтобы предотвратить дальнейшую обработку события
            }
        }
    }
}