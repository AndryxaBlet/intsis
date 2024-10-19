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

namespace intsis
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            binddatagrid();
            intsisEntities.GetContext().Database.Connection.ConnectionString = connect;
        }
        
        string connect = Properties.Settings.Default.NotebookSQL;
        private DataTable dt;
        public void binddatagrid()
        {
            Dg.ItemsSource = intsisEntities.GetContext().NameSis.ToList();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            var selectedRuleId = -1;
            if (Dg.SelectedIndex != -1)
            {
                 selectedRuleId = (Dg.ItemsSource as List<NameSis>)[Dg.SelectedIndex].ID;
            }
            Create_Window window=new Create_Window(selectedRuleId);
            window.ShowDialog();
            binddatagrid();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (Dg.SelectedIndex!=-1)
            {
                var selectedRuleId = (Dg.ItemsSource as List<NameSis>)[Dg.SelectedIndex].ID;
                Answers answers = new Answers(selectedRuleId);
                answers.ShowDialog();
            }
            else MessageBox.Show("Выберите систему");

        }
    }
}
