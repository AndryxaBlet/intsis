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

namespace intsis
{
    /// <summary>
    /// Логика взаимодействия для CreateSis.xaml
    /// </summary>
    public partial class CreateSis : Window
    {
        public CreateSis()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                insert(NameTextBox.Text, ScopeTextBox.Text, CommentTextBox.Text);
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }
            finally 
            {
                MessageBox.Show("Успешно сохранено");
                this.Close();
            }
        }
        public void insert(string name, string scope, string comment)
        {
            try
            {
                // Добавляем новую систему
                var newSystem = new NameSis
                {
                    Name = name,
                    ScopeOfApplication = scope,
                    Comment = comment
                };

                intsisEntities.GetContext().NameSis.Add(newSystem);
                intsisEntities.GetContext().SaveChanges();
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }
        }
    }
}
