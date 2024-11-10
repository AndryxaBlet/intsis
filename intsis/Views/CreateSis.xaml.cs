using System;
using System.Collections.Generic;
using System.Deployment.Internal;
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
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;
using CustomMessageBox = Wpf.Ui.Controls.MessageBox;

namespace intsis.Views
{
    /// <summary>
    /// Логика взаимодействия для CreateSis.xaml
    /// </summary>
    public partial class CreateSis : FluentWindow
    {
        public CreateSis(int id)
        {
            InitializeComponent();
            if (id != -1)
            {
                sys=intsisEntities.GetContext().NameSis.Where(x => x.ID == id).FirstOrDefault();
                NameTextBox.Text=sys.Name;
                ScopeTextBox.Text = sys.ScopeOfApplication;
                CommentTextBox.Text = sys.Comment;

            }
        }
        public static int SystemId { get; private set; }
        NameSis sys=null;

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sys!=null)
                {
                    sys.Name=NameTextBox.Text;
                    sys.ScopeOfApplication=ScopeTextBox.Text;
                    sys.Comment = CommentTextBox.Text;
                    intsisEntities.GetContext().SaveChanges();
                }
                else
                {
                    insert(NameTextBox.Text, ScopeTextBox.Text, CommentTextBox.Text);
                }
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
                var sisid=intsisEntities.GetContext().NameSis.Where(x=>x.Name==name && x.ScopeOfApplication==scope && x.Comment==comment).FirstOrDefault();
                SystemId = sisid.ID;
                DialogResult = true;
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }
        }

        private void NameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ScopeTextBox.Focus();
                e.Handled = true;
            }
        }

        private void ScopeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommentTextBox.Focus();
                e.Handled = true;
            }
        }

        private void CommentTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Вызываем обработчик Click кнопки
                SaveButton_Click(SaveButton, new RoutedEventArgs());
            }
        }
    }
}
