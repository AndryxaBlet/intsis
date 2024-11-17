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
                sys=ExpertSystemEntities.GetContext().ExpSystem.Where(x => x.Id == id).FirstOrDefault();
                NameTextBox.Text=sys.Name;
                ScopeTextBox.Text = sys.ScopeOfApplication;
                CommentTextBox.Text = sys.Description;
                WeightSwitch.IsChecked = sys.Type;
                

            }
        }
        public static int SystemId { get; private set; }
        ExpSystem sys=null;

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sys!=null)
                {
                    sys.Name=NameTextBox.Text;
                    sys.ScopeOfApplication=ScopeTextBox.Text;
                    sys.Description = CommentTextBox.Text;
                    ExpertSystemEntities.GetContext().SaveChanges();
                }
                else
                {
                    insert(NameTextBox.Text, ScopeTextBox.Text, CommentTextBox.Text,Convert.ToBoolean(WeightSwitch.IsChecked));
                  
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
        public void insert(string name, string scope, string comment,bool type)
        {
            try
            {
                // Добавляем новую систему
                var newSystem = new ExpSystem
                {
                    Name = name,
                    ScopeOfApplication = scope,
                    Description = comment,
                    Type = type
                };

                ExpertSystemEntities.GetContext().ExpSystem.Add(newSystem);
                ExpertSystemEntities.GetContext().SaveChanges();
                var sisid=ExpertSystemEntities.GetContext().ExpSystem.Where(x=>x.Name == name && x.ScopeOfApplication == scope && x.Description == comment && x.Type==type).FirstOrDefault();
                SystemId = sisid.Id;
                if (WeightSwitch.IsChecked == true)
                {
                    WeightedSystem_Fact first = new WeightedSystem_Fact
                    {
                        SystemId = SystemId,
                        Name = "Fact",
                        Text="Системный факт по умолчанию, нужен для составления таблицы лидеров. Необходимо заполнить несколько вопросов для работы системы"
                        
                    };
                    ExpertSystemEntities.GetContext().WeightedSystem_Fact.Add(first);
                    ExpertSystemEntities.GetContext().SaveChanges();
                }
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
