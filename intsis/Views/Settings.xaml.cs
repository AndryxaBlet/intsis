using ControlzEx.Theming;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Appearance;
using System.Data.SqlClient;
using System.Configuration;



namespace intsis.Views
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();
            SaveSettings.Background = GlobalDATA.Accent;
            ConnectionString.Text = intsis.Properties.Settings.Default.ChoosedServer;
            OutLog.Visibility = Visibility.Collapsed;

            if (intsis.Properties.Settings.Default.RemembeR == true)
            {
                OutLog.Visibility = Visibility.Visible;
            }
            if (intsis.Properties.Settings.Default.Theme == "Тёмная")
            {
                ThemeComboBox.SelectedIndex = 0;
            }
            else { ThemeComboBox.SelectedIndex = 1; }

            if (intsis.Properties.Settings.Default.IsLocalDB == true)
            {
                UseLocalDatabase.IsChecked =true;
            }

            if (UseLocalDatabase.IsChecked == true) { ConnectionString.IsEnabled = false; }

        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            switch (ThemeComboBox.SelectedIndex)
            {
                case 0: { ApplicationThemeManager.Apply(ApplicationTheme.Dark); intsis.Properties.Settings.Default.Theme = "Тёмная"; }; break;
                case 1: { ApplicationThemeManager.Apply(ApplicationTheme.Light); intsis.Properties.Settings.Default.Theme = "Светлая";}; break;
            }

            if (UseLocalDatabase.IsChecked == true)
            {
                intsis.Properties.Settings.Default.IsLocalDB = true;
            }
            else
            {
                intsis.Properties.Settings.Default.IsLocalDB = false;
                intsis.Properties.Settings.Default.ChoosedServer =ConnectionString.Text ;
            }

            intsis.Properties.Settings.Default.Save();


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("В РАЗРАБОТКЕ");
        }

        private void UseLocalDatabase_Click(object sender, RoutedEventArgs e)
        {
            if(UseLocalDatabase.IsChecked == true) {  ConnectionString.IsEnabled = false; }
            else { ConnectionString.IsEnabled = true; }
        }

        private void OutLog_Click(object sender, RoutedEventArgs e)
        {
            intsis.Properties.Settings.Default.RemembeR = false;
            intsis.Properties.Settings.Default.Save();
            OutLog.Visibility = Visibility.Collapsed;
        }
    }
}
