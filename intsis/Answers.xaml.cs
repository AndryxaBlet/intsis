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
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace intsis
{
    /// <summary>
    /// Логика взаимодействия для Answers.xaml
    /// </summary>
    public partial class Answers : Window
    {
        public Answers(int ID)
        {
            InitializeComponent();
            next=FIRST(ID);
            log = ID;
        }
        int next = 0;
        string connect = "data source = 1-236-EMP-01; initial catalog = intsisIR311; persist security info=True;user id = sa; password=123;MultipleActiveResultSets=True;";
        int log = 0;
    

        public int FIRST(int id)
        {
            SqlConnection mycon = new SqlConnection(connect);
            mycon.Open();
            string query = "SELECT Text from Rules where IDSis=" + id.ToString();
            string qu = "SELECT IDRule from Rules where IDSis=" + id.ToString();
            SqlCommand command = new SqlCommand(query, mycon);
            SqlCommand com = new SqlCommand(qu, mycon);
            var i = command.ExecuteScalar();
            var IDRule = com.ExecuteScalar();
            VOP.Content = i.ToString();
            UpdateItems((int)IDRule);
            return (int)IDRule;
        }
        private void UpdateItems(int id)
        {
                using (SqlConnection con = new SqlConnection(connect))
                {
                    con.Open();
                    SqlDataAdapter sa = new SqlDataAdapter($"SELECT * FROM Answer where IDRule={id.ToString()}", con);
                    DataSet ds = new DataSet();
                    sa.Fill(ds, "t");
                    CB.ItemsSource = ds.Tables["t"].DefaultView;
                    CB.DisplayMemberPath = "Ans";
                    CB.SelectedValuePath = "ID";
            }
            
        }

        public void BNext()
        {
      
            string v = CB.SelectedValue.ToString();
            SqlConnection mycon = new SqlConnection(connect);
            mycon.Open();
            string query = $"SELECT NextR from Answer where ID={v}";
            SqlCommand command = new SqlCommand(query, mycon);
            var i = command.ExecuteScalar();
            if (int.TryParse(i.ToString(), out next))
            {
                string p = "SELECT Rec from Answer where ID=" + v;
                SqlCommand dop = new SqlCommand(p, mycon);
                var REC = dop.ExecuteScalar();
                if (REC.ToString() != "") { MessageBox.Show(REC.ToString()); }
                string qu = "SELECT Text from Rules where IDRule=" + next.ToString();
                SqlCommand co = new SqlCommand(qu, mycon);
                var n = co.ExecuteScalar();
                VOP.Content = n.ToString();
                UpdateItems(next);
            }
            else
            {

                VOP.Content = i.ToString();
                CB.Visibility = Visibility.Hidden;
                Deny.Visibility = Visibility.Hidden;
                Repeat.Visibility = Visibility.Visible;
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            next=FIRST(log);
            CB.Visibility = Visibility.Visible;
            Deny.Visibility = Visibility.Visible;
            Repeat.Visibility = Visibility.Hidden;
        }
        private void Deny_Click(object sender, RoutedEventArgs e)
        {
            BNext();
        }
    }
}
