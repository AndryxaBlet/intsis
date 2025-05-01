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
using System.Xml.Linq;

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
            var types = ExpertSystemV2Entities.GetContext().TypeOfSys.ToList();
            TypeSwitch.ItemsSource = types;
            TypeSwitch.DisplayMemberPath = "Name";
            TypeSwitch.SelectedValuePath = "TypeID";
            if (id != -1)
            {
                sys= ExpertSystemV2Entities.GetContext().ExpSystems.Where(x => x.ExpSysID == id).FirstOrDefault();
                NameTextBox.Text=sys.NameSys;
                ScopeTextBox.Text = sys.ScopeOfApplication;
                CommentTextBox.Text = sys.Description;
                TypeSwitch.SelectedValue = sys.TypeID;      // ПОСТАВЬ ПОЛЕ

            }
           
        }

        public static int SystemId { get; private set; }
        ExpSystems sys=null;

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sys!=null)
                {
                    sys.NameSys=NameTextBox.Text;
                    sys.ScopeOfApplication=ScopeTextBox.Text;
                    sys.Description = CommentTextBox.Text;
                    var type = ExpertSystemV2Entities.GetContext().TypeOfSys.Where(x=>x.TypeID==(int)TypeSwitch.SelectedValue).First();
                    sys.TypeOfSys=type;
                    ExpertSystemV2Entities.GetContext().SaveChanges();
                }
                else
                {
                    insert(NameTextBox.Text, ScopeTextBox.Text, CommentTextBox.Text,(int)TypeSwitch.SelectedValue);//////////////////////////////////////////////// замени 0 на поле 
                  
                }
            }
            finally 
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок",Content = "Успешно сохранено." };
                messagebox.ShowDialogAsync();
                this.Close();
            }
        }
        public void insert(string name, string scope, string comment,int type)
        {
            //try
            //{
                // Добавляем новую систему
                var newSystem = new ExpSystems
                {
                    NameSys = name,
                    ScopeOfApplication = scope,
                    Description = comment,
                    TypeID = type
                };

                ExpertSystemV2Entities.GetContext().ExpSystems.Add(newSystem);
                ExpertSystemV2Entities.GetContext().SaveChanges();
               
                if (Convert.ToInt32(TypeSwitch.SelectedValue) == 1)
                {
                    Facts first = new Facts
                    {
                        ExpSysID = newSystem.ExpSysID,
                        Name = "Fact",
                        Description="Системный факт по умолчанию, нужен для составления таблицы лидеров. Необходимо заполнить несколько вопросов для работы системы"
                        
                    };
                    ExpertSystemV2Entities.GetContext().Facts.Add(first);
                    ExpertSystemV2Entities.GetContext().SaveChanges();
                }
                DialogResult = true;
                SystemId = newSystem.ExpSysID;
            //}
            //catch (Exception r)
            //{
            //    MessageBox.Show(r.Message);
            //}
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
