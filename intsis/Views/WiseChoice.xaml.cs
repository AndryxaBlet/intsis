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
using Wpf.Ui.Appearance;
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

            if (intsis.Properties.Settings.Default.Theme == "Тёмная")
            {
                ApplicationThemeManager.Apply(ApplicationTheme.Dark);

            }
            else if (intsis.Properties.Settings.Default.Theme == "Светлая")
            {
                ApplicationThemeManager.Apply(ApplicationTheme.Light);
            }
            GlobalDATA.Accent = ApplicationAccentColorManager.PrimaryAccentBrush;
            Application.Current.Resources["AccentColor"]=GlobalDATA.Accent;

        }

        private void LogPG_Click(object sender, RoutedEventArgs e)
        {
            MainNavigation.Navigate(typeof(MainWindow));
        }

        private void MainNavigation_Loaded(object sender, RoutedEventArgs e)
        {
            MainNavigation.Navigate(typeof(Info));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainNavigation.Navigate(typeof(Settings));
        }

        private void FluentWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BrowserClick(object sender, RoutedEventArgs e)
        {
            MainNavigation.Navigate(typeof(WebBrowser));
        }
    }
}
