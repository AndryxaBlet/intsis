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
using System.Windows.Shapes;
using System.Reflection;

using static System.Net.Mime.MediaTypeNames;

namespace intsis
{
    /// <summary>
    /// Логика взаимодействия для Create_Window.xaml
    /// </summary>
    public partial class Create_Window : Window
    {
        public Create_Window()
        {
            InitializeComponent();
        }
        string connect = "data source=1-236-EMP-01;initial catalog=intsisIR311;persist security info=True;user id=sa;password=123;MultipleActiveResultSets=True;";
        private SqlDataAdapter da;
        private DataTable dt;
        public DataSet dataSet { get; set; }
        public SqlCommandBuilder builder { get; set; }
        int id = 0;

        public void binddatagrid(string name)
        {
            SqlConnection mycon = new SqlConnection(connect);
            mycon.Open();
            string qu = $"SELECT ID from NameSis where NameSis.Name='{name}'";
            SqlCommand command = new SqlCommand(qu, mycon);
            var i = command.ExecuteScalar();
            id = (int)i;
            string query = "SELECT * from Rules where IDSis="+id.ToString();
            SqlCommand c= new SqlCommand(query, mycon);
            da = new SqlDataAdapter(c);
            dt = new DataTable();
            da.Fill(dt);
            dt.Columns[1].DefaultValue = id;
            Dg.ItemsSource = dt.DefaultView;
        }
        public void insert(string name)
        {
            SqlConnection mycon = new SqlConnection(connect);
            mycon.Open();
            string query = $"Insert NameSis (Name) values ('{name}')";
            SqlCommand command = new SqlCommand(query, mycon);
            command.ExecuteNonQuery();
            binddatagrid(name);
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connect))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.CommandText = "select * from NameSis where Name=@name";
                command.Connection = connection;
                command.Parameters.AddWithValue("@name", NameI.Text);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        MessageBox.Show("Редактирование данных");
                        binddatagrid(NameI.Text);
                    }
                    else
                    {
                        MessageBox.Show("Создана новая система");
                        insert(NameI.Text);
                    }
                }
            }
        }

        private void Create_Copy_Click(object sender, RoutedEventArgs e)
        {
           
            using (SqlConnection conn = new SqlConnection(connect))
            {
                SqlCommand com = new SqlCommand();
                conn.Open();
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(da);
                da.Update(dt);
                binddatagrid(NameI.Text);
                MessageBox.Show("Update get succesful");
            }
        }

        private void C(object sender, RoutedEventArgs e)
        {
            if (Dg.SelectedIndex != -1)
            {
                ANS ans = new ANS(dt.DefaultView[Dg.SelectedIndex]["IDRule"].ToString());
                ans.Show();
            }
            else MessageBox.Show("Выберите систему");
            //test
        }
    }
}
