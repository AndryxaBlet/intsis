using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace intsis.Views
{
    /// <summary>
    /// Логика взаимодействия для WiseChoice.xaml
    /// </summary>
    public partial class WiseChoice : FluentWindow
    {
        public WiseChoice()
        {
            InitializeComponent();
        }

        private void LogPG_Click(object sender, RoutedEventArgs e)
        {
            MainNavigation.Navigate(typeof(LogIn));
        }
    }
}
