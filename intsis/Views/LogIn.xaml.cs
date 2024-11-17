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
using Wpf.Ui.Controls;
using static intsis.Views.MainWindow;
using System.Web.UI.WebControls;
using System.Configuration;
using Wpf.Ui.Appearance;

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
        }
       

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                string username = LoginTextBox.Text;
                string password = PasswordBox.Password;
            if (username != "" && password != "")
            {

                var login = ExpertSystemEntities.GetContext().User.FirstOrDefault(l => l.Login == username && l.Password == password);

                if (login != null)
                {
                 
                    GlobalDATA.recvadmin= Convert.ToBoolean(login.IsAdmin); 
                    var navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
                    navigateView.Navigate(typeof(MainWindow));
                   
                    
                    

                }
                else
                {
                    var messagebox = new Wpf.Ui.Controls.MessageBox {
                        Title = "Ошибка",
                        Content= "Неверно введены данные аккаунта!",
                        PrimaryButtonText = "OK",
                    };
                    messagebox.ShowDialogAsync();
                }
            }
            else
            {
                var messagebox = new Wpf.Ui.Controls.MessageBox { Title = "Ошибка", Content = "Оба поля авторизации должны быть заполнены!" };
                messagebox.ShowDialogAsync();
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
            GlobalDATA.IsFirst = false;
            var navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
            navigateView.Navigate(typeof(Registration));
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