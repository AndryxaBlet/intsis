using System;
using System.Linq;
using System.Windows;
using System.Data.Entity;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Data.Entity.Validation;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Web.Configuration;
using System.Data;
using intsis;
using System.Security.Cryptography;
using System.Configuration.Internal;
using Microsoft.Win32;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;
using CustomMessageBox = Wpf.Ui.Controls.MessageBox;


namespace intsis.Views
{
    /// <summary>
    /// Логика взаимодействия для Create_Window.xaml
    /// </summary>
    public partial class Create_Window : Page
    {
        private int id = 0;
        private int SelectedSys = -1;    
        SqlJSON sqlJSON = new SqlJSON();
        bool IsWeight = false;
        Wpf.Ui.Controls.NavigationView navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;

        public Create_Window()
        {   
            InitializeComponent();
            BindComboBox();
            DgLinear.Visibility = Visibility.Hidden;
            DgWeight.Visibility = Visibility.Hidden;
            int id = GlobalDATA.IdSisForCREATE;
            if (id != -1)
            {                
                NameI.SelectedValue = id;
                IsWeight=ExpertSystemEntities.GetContext().ExpSystem.Where(r => r.Id == id).First().Type;
                binddatagrid(Convert.ToInt32(NameI.SelectedValue));
                binddatagrid(id);
            }
        }

        void BindComboBox()
        {
            var Sis = ExpertSystemEntities.GetContext().ExpSystem.ToList();
            NameI.ItemsSource = Sis;
            NameI.DisplayMemberPath = "Name";
            NameI.SelectedValuePath = "Id";
            binddatagrid(Convert.ToInt32(NameI.SelectedValue));
        }

        public void binddatagrid(int systemId)
        {
            {
                SelectedSys = systemId;
                if (ExpertSystemEntities.GetContext().ExpSystem.Where(r => r.Id == systemId).FirstOrDefault() != null)
                {

                    if (!IsWeight)
                    {
                        if (systemId != 0)
                        {
                            id = systemId;

                            // Получаем правила, привязанные к системе
                            var rules = ExpertSystemEntities.GetContext().LinearSystem_Question
                                .Where(r => r.SystemId == systemId)
                                .ToList();

                            // Привязываем данные к DataGrid
                            DgLinear.ItemsSource = rules;
                            DgLinear.Visibility = Visibility.Visible;
                            DgWeight.Visibility = Visibility.Collapsed;
                            Ans.Visibility = Visibility.Visible;
                            ChangeFact.Visibility = Visibility.Collapsed;

                            // Устанавливаем значение по умолчанию для колонки IDSis
                            foreach (var rule in rules)
                            {
                                rule.SystemId = systemId;
                            }
                        }
                    }
                    else {

                        if (systemId != 0)
                        {
                            id = systemId;

                            // Получаем правила, привязанные к системе
                            var rules = ExpertSystemEntities.GetContext().WeightedSystem_Fact
                                .Where(r => r.SystemId == systemId)
                                .ToList();

                            // Привязываем данные к DataGrid
                            DgWeight.ItemsSource = rules;
                            DgLinear.Visibility = Visibility.Collapsed;
                            DgWeight.Visibility=Visibility.Visible;
                            Ans.Visibility = Visibility.Collapsed;
                            ChangeFact.Visibility = Visibility.Visible;

                            // Устанавливаем значение по умолчанию для колонки IDSis
                            foreach (var rule in rules)
                            {
                                rule.SystemId = systemId;
                            }
                        }
                    }
                }
            }
        }


        public void Create_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (DgLinear.SelectedIndex != -1 || DgWeight.SelectedIndex != -1)
            {
                if (SelectedSys != -1)
                { var nameSisId = SelectedSys;


                    if (!IsWeight)
                    {
                        // Получаем измененные данные из DataGrid
                        var rules = DgLinear.ItemsSource as List<LinearSystem_Question>;

                        if (rules != null)
                        {
                            // Получаем ID существующей ExpSystem
                            foreach (var rule in rules)
                            {
                                // Проверяем, существует ли запись в базе данных
                                var existingRule = ExpertSystemEntities.GetContext().LinearSystem_Question
                                    .FirstOrDefault(r => r.Id == rule.Id);

                                if (existingRule != null)
                                {
                                    // Обновляем существующую запись
                                    ExpertSystemEntities.GetContext().Entry(existingRule).CurrentValues.SetValues(rule);
                                }
                                else
                                {
                                    // Устанавливаем правильный ID для нового правила
                                    var newRule = new LinearSystem_Question
                                    {
                                        SystemId = nameSisId,
                                        Text = rule.Text
                                    };
                                    ExpertSystemEntities.GetContext().LinearSystem_Question.Add(newRule);
                                }
                            }

                            // Сохраняем изменения в базе данных
                            ExpertSystemEntities.GetContext().SaveChanges();
                            binddatagrid(SelectedSys);

                        }
                    }
                    else
                    {
                        // Получаем измененные данные из DataGrid
                        var facts = DgWeight.ItemsSource as List<WeightedSystem_Fact>;

                        if (facts != null)
                        {
                            // Получаем ID существующей ExpSystem
                            foreach (var fact in facts)
                            {
                                // Проверяем, существует ли запись в базе данных
                                var existingRule = ExpertSystemEntities.GetContext().WeightedSystem_Fact
                                    .FirstOrDefault(r => r.Id == fact.Id);

                                if (existingRule != null) 
                                {
                                    // Обновляем существующую запись
                                    ExpertSystemEntities.GetContext().Entry(existingRule).CurrentValues.SetValues(fact);
                                }
                                else
                                {
                                    // Устанавливаем правильный ID для нового правила
                                    var newRule = new WeightedSystem_Fact
                                    {
                                        SystemId = nameSisId,
                                        Name = fact.Name,
                                        Text = fact.Text
                                    };
                                    ExpertSystemEntities.GetContext().WeightedSystem_Fact.Add(newRule);
                                }
                            }

                            // Сохраняем изменения в базе данных
                            ExpertSystemEntities.GetContext().SaveChanges();
                            binddatagrid(SelectedSys);

                        }
                    }
                }
                else
                {
                    var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок",Content = "Не выбрана система." };
                    messagebox.ShowDialogAsync();
                }
            }
            else
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок",Content = "Выберите систему." };
                messagebox.ShowDialogAsync();
            }

        }


        private void C(object sender, RoutedEventArgs e)
        {
            if (DgLinear.SelectedIndex != -1)
            {
                var nameSisId = SelectedSys;
                if (ExpertSystemEntities.GetContext().ExpSystem.Where(r => r.Id == nameSisId).FirstOrDefault().Type == false)
                {
                    // Открываем окно с вопросами для выбранного правила
                    LinearSystem_Question selectedRuleId = DgLinear.SelectedValue as LinearSystem_Question;
                    GlobalDATA.SelectRULEID = selectedRuleId.Id;
                    navigateView.Navigate(typeof(ANS));
                }
            }
            else
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок",Content = "Выберите вопрос или систему." };
                messagebox.ShowDialogAsync();
            }
        }

        private void NameI_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Проверяем, существует ли система с таким именем

            var systemExists = ExpertSystemEntities.GetContext().ExpSystem.Any(ns => ns.Name == NameI.Text);

            if (systemExists)
            {
                int t = Convert.ToInt32(NameI.SelectedValue);
                IsWeight = ExpertSystemEntities.GetContext().ExpSystem.Where(r => r.Id == t).First().Type;
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Content = "Редактирование данных." }.ShowDialogAsync();
                binddatagrid(Convert.ToInt32(NameI.SelectedValue));
                SelectedSys = Convert.ToInt32(NameI.SelectedValue);
                GlobalDATA.IdSisForCREATE = SelectedSys;
            }
            BindComboBox();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (NameI.SelectedIndex != -1)
            {
                int del = Convert.ToInt32(NameI.SelectedValue);
                var itemToDelete = ExpertSystemEntities.GetContext().ExpSystem.FirstOrDefault(x => x.Id == del);

                if (itemToDelete != null)
                {     

                    var result =new Wpf.Ui.Controls.MessageBox {  Content = "Удалить систему?",Title= "Подтверждение", PrimaryButtonText = "Дa", CloseButtonText = "Нет" }.ShowDialogAsync().Result;

                    if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
                    { 
                        result =new Wpf.Ui.Controls.MessageBox { Content = "Вы уверены, что хотите продолжить?", Title = "Подтверждение", PrimaryButtonText = "Дa", CloseButtonText = "Нет" }.ShowDialogAsync().Result;

                        if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
                        {
                            // Удалить объект из контекста
                            ExpertSystemEntities.GetContext().ExpSystem.Remove(itemToDelete);
                            

                            // Сохранить изменения в базе данных
                            ExpertSystemEntities.GetContext().SaveChanges();
                            navigateView.GoBack();
                        }
                    }
                }
            }
            else
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок",Content = "Выберите систему." };
                messagebox.ShowDialogAsync();
            }
        }

        private void CreateSystem_Click(object sender, RoutedEventArgs e)
        {
            CreateSis createSis = new CreateSis(-1);
            
            bool? result = createSis.ShowDialog();

            if (result == true) // Проверяем, что окно закрыто с успешным результатом
            {
                int systemId = CreateSis.SystemId;
                BindComboBox();
                NameI.SelectedIndex = NameI.Items.Count - 1;
            }
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is System.Windows.Controls.Button button)
                {
                    // Получаем текущий объект строки, к которому относится ComboBox
                    var dataGridRow = intsis.FUNC.FindParent<DataGridRow>(button);

                    if (!IsWeight)
                    {
                        if (dataGridRow?.Item is LinearSystem_Question deleted)
                        {
                            var itemToDelete = ExpertSystemEntities.GetContext().LinearSystem_Question.FirstOrDefault(x => x.Id == deleted.Id);
                            ExpertSystemEntities.GetContext().LinearSystem_Question.Remove(itemToDelete);
                            ExpertSystemEntities.GetContext().SaveChanges();
                            binddatagrid(id);
                        }
                    }
                    else
                    {
                        if(dataGridRow?.Item is WeightedSystem_Fact deleted)
                        {
                            var itemToDelete = ExpertSystemEntities.GetContext().WeightedSystem_Fact.FirstOrDefault(x => x.Id == deleted.Id);
                            ExpertSystemEntities.GetContext().WeightedSystem_Fact.Remove(itemToDelete);
                            ExpertSystemEntities.GetContext().SaveChanges();
                            binddatagrid(id);
                        }
                    }
                }
            }
            catch(ArgumentNullException)
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Ошибка", Content = "Примените изменения перед удалением." };
                messagebox.ShowDialogAsync();
            }
        }

        private void UpdIS_Click(object sender, RoutedEventArgs e)
        {
            if (NameI.SelectedIndex != -1) { 
            CreateSis createSis = new CreateSis(Convert.ToInt32(NameI.SelectedValue));
            createSis.Title = "Редактировать систему";

            bool? result = createSis.ShowDialog();

            if (result == true) // Проверяем, что окно закрыто с успешным результатом
            {
                int systemId = CreateSis.SystemId;
                binddatagrid(systemId);
                BindComboBox();
                NameI.SelectedValue = systemId;
            }
            }
            else
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Ошибка", Content = "Выберите систему для редактирования" };
                messagebox.ShowDialogAsync();
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
             OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*", // Установите фильтр для файлов
                Title = "Выберите файл" // Заголовок диалогового окна
            };
            // Отображаем диалоговое окно и проверяем, была ли нажата кнопка "Открыть"
            if (openFileDialog.ShowDialog() == true)
            {
                // Получаем путь к выбранному файлу
                string filePath = openFileDialog.FileName;

                // Здесь можно добавить код для обработки файла
                MessageBox.Show($"Выбранный файл: {filePath}");
                sqlJSON.ImportData(filePath);
            }
            BindComboBox();
            NameI.SelectedIndex=NameI.Items.Count-1;

        }

        private void export_Click(object sender, RoutedEventArgs e)
        {
            if (NameI.SelectedIndex != -1)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*", // Установите фильтр для файлов
                    Title = "Выберите файл" // Заголовок диалогового окна
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    // Получаем путь к выбранному файлу
                    string filePath = saveFileDialog.FileName;

                    // Здесь можно добавить код для обработки файла
                    MessageBox.Show($"Выбранный файл: {filePath}");
                    sqlJSON.ExportData(Convert.ToInt32(NameI.SelectedValue), filePath);
                }
            }
            else
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Ошибка", Content = "Выберите систему для экспорта." };
                messagebox.ShowDialogAsync();
            }

        }

        private void ChangeFact_Click(object sender, RoutedEventArgs e)
        {
            if (DgWeight.SelectedIndex != -1)
            {
                var nameSisId = SelectedSys;
                if (ExpertSystemEntities.GetContext().ExpSystem.Where(r => r.Id == nameSisId).FirstOrDefault().Type == true)
                {
                    // Открываем окно с вопросами для выбранного правила
                    WeightedSystem_Fact selectedRuleId = DgWeight.SelectedValue as WeightedSystem_Fact;
                    GlobalDATA.SelectFACTID = selectedRuleId.Id;
                    navigateView.Navigate(typeof(FactChanger));
                }
            }
            else
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Ошибка", Content = "Выберите вопрос или систему." };
                messagebox.ShowDialogAsync();
            }
        }
    }
}