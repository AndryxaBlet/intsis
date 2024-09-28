using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace intsis
{
    /// <summary>
    /// Логика взаимодействия для ANS.xaml
    /// </summary>
    public partial class ANS : Window
    {
        public ANS(string ID)
        {
            InitializeComponent();
            binddatagrid(ID);
            id = ID;

        }
        string connect = "data source=1-236-EMP-01;initial catalog=intsisIR311;persist security info=True;user id=sa;password=123;MultipleActiveResultSets=True;";
        private SqlDataAdapter da;
        private DataTable dt;
        string id = "";
        public DataSet dataSet { get; set; }
        public SqlCommandBuilder builder { get; set; }

        public void binddatagrid(string ID)
        {
            SqlConnection mycon = new SqlConnection(connect);
            mycon.Open();
            string query = "SELECT * from Answer where IDRule=" + ID;
            SqlCommand c = new SqlCommand(query, mycon);
            da = new SqlDataAdapter(c);
            dt = new DataTable();
            da.Fill(dt);
            Dg.ItemsSource = dt.DefaultView;
            dt.Columns[1].DefaultValue = ID;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connect))
            {
                SqlCommand com = new SqlCommand();
                conn.Open();
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(da);
                da.Update(dt);
                binddatagrid(id.ToString());
                MessageBox.Show("Update get succesful");
            }
        }
    }
}
