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
        }
        string connect = "data source=1-236-EMP-01;initial catalog=intsisIR311;persist security info=True;user id=sa;password=123;MultipleActiveResultSets=True;";
        private SqlDataAdapter da;
        private DataTable dt;
        public void binddatagrid()
        {
            SqlConnection mycon = new SqlConnection(connect);
            mycon.Open();
            string query = "SELECT * from NameSis";
            SqlCommand command = new SqlCommand(query, mycon);
            da = new SqlDataAdapter(command);
            dt = new DataTable("Workless");
            da.Fill(dt);

            Dg.ItemsSource = dt.DefaultView;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            Create_Window window=new Create_Window();
            window.ShowDialog();
            binddatagrid();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (Dg.SelectedIndex!=-1)
            {
                Answers answers = new Answers((int)dt.DefaultView[Dg.SelectedIndex]["ID"]);
                answers.ShowDialog();
            }
            else MessageBox.Show("Выберите систему");

        }
    }
}
