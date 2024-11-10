using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace intsis.Views
{
    /// <summary>
    /// Логика взаимодействия для Info.xaml
    /// </summary>
    public partial class Info : Page
    {
        public Info()
        {
            InitializeComponent();
            if (intsis.Properties.Settings.Default.Theme == "Тёмная")
            {
                ApplicationThemeManager.Apply(ApplicationTheme.Dark);

            }
            else if (intsis.Properties.Settings.Default.Theme == "Светлая")
            {
                ApplicationThemeManager.Apply(ApplicationTheme.Light);
            }
        }

        Wpf.Ui.Controls.NavigationView navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
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
        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            string connects;
            if (intsis.Properties.Settings.Default.IsLocalDB == true)
            {
                connects = ConfigurationManager.ConnectionStrings["intsisEntitiesLDB"].ConnectionString;
            }
            else { connects = intsis.Properties.Settings.Default.ChoosedServer; }
            intsisEntities.GetContext().Database.Connection.ConnectionString = connects;

            var users = intsisEntities.GetContext().User.FirstOrDefault();
            if (users == null)
            {
                GlobalDATA.IsFirst = true;
                navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
                navigateView.Navigate(typeof(Registration));

            }
            else
            {
                navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
                navigateView.Navigate(typeof(LogIn));
            }
        }
    }
}
