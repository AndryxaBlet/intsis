using System;
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
            bool First = GlobalDATA.IsFirst;
            InitializeComponent();
            if(First)
            FirstAdmin();
        }
        private void FirstAdmin()
        {
                isAdmin = true;
                MessageBox.Show("Ваш аккаунт первый в системе, вы автоматически назначенны администратором, запомните ваши данные аккаунта", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            
        }
        bool isAdmin = false;


        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string username = LoginTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;


            if (string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Все поля должны быть заполнены!");
                return;
            }


            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }


            var existingUser = intsisEntities.GetContext().User.FirstOrDefault(u => u.Login == username || u.Email == email);

            if (existingUser != null)
            {
                MessageBox.Show("Пользователь с таким логином или электронной почтой уже существует!");
                return;
            }
            User newUser;
            if (isAdmin)
            {
                newUser = new User
                {
                    Email = email,
                    Login = username,
                    Password = password,
                    IsAdmin = true

                };
            }
            else
            {
                 newUser = new User
                {
                    Email = email,
                    Login = username,
                    Password = password,
                    IsAdmin = false // Set to false by default; adjust as needed

                };
            }

            intsisEntities.GetContext().User.Add(newUser);
            intsisEntities.GetContext().SaveChanges();

            MessageBox.Show("Регистрация успешна!");

            // Optionally, open the main window after registration

            GlobalDATA.recvadmin = newUser.IsAdmin;
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
