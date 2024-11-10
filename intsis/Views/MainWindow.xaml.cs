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
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;
using CustomMessageBox = Wpf.Ui.Controls.MessageBox;
using System.Web.UI.WebControls;

namespace intsis.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Page
    {
       
        public MainWindow()
        {
            InitializeComponent();
            binddatagrid();
            bool admin = GlobalDATA.recvadmin;
            if (!admin) {
                Create.Visibility = Visibility.Hidden;
            }
            
        }
        public void binddatagrid()
        {
            try
            {
                Dg.ItemsSource = intsisEntities.GetContext().NameSis.ToList();
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRuleId = -1;
                if (Dg.SelectedIndex != -1)
                {
                    selectedRuleId = (Dg.ItemsSource as List<NameSis>)[Dg.SelectedIndex].ID;
                }
                GlobalDATA.IdSisForCREATE = selectedRuleId;
                var navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
                navigateView.Navigate(typeof(Create_Window));

                binddatagrid();
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Dg.SelectedIndex != -1)
                {
                    NameSis selectedRuleId = Dg.SelectedValue as NameSis;
                    GlobalDATA.IdSisForCREATE = selectedRuleId.ID;
                    var navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
                    navigateView.Navigate(typeof(Answers));
                }
                else MessageBox.Show("Выберите систему", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Нужно выбрать существующий набор вопросов", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }
            
        }

        private void Grid_Click(object sender, MouseEventArgs e)
        {
            if (Dg.IsFocused && !Dg.IsMouseOver)
            {
                Dg.SelectedIndex = -1;
            }
        }

        private void Dg_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
                Dg.SelectedIndex = -1;
            
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Dg.IsFocused && !Dg.IsMouseOver)
            {
                Dg.SelectedIndex = -1;
            }
        }

        private void Dg_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Dg.SelectedItem = null;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

         
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            // Устанавливаем минимальные и максимальные размеры окна
            double minWidth = 800;  // минимальная ширина для нормального вида
            double minHeight = 600; // минимальная высота для нормального вида


            // Вычисляем коэффициент масштабирования на основе текущего размера окна
            double scaleFactorX = e.NewSize.Width / minWidth;
            double scaleFactorY = e.NewSize.Height / minHeight;

            // Ограничиваем коэффициент масштабирования для X и Y
            double scaleFactor = Math.Min(scaleFactorX, scaleFactorY);  // Масштабируем по диагонали

            // Ограничиваем минимальное и максимальное масштабирование
            scaleFactor = Math.Max(0.5, scaleFactor);  // Минимальный масштаб
            scaleFactor = Math.Min(1, scaleFactor);  // Максимальный масштаб

            // Применяем коэффициент масштабирования
            scaleTransform.ScaleX = scaleFactor;
            scaleTransform.ScaleY = scaleFactor;
        }

    
    }
    
}
