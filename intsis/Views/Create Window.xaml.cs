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
using System.Web.UI.WebControls;
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
        int SystemType;
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
                SystemType = ExpertSystemV2Entities.GetContext().ExpSystems.Where(r => r.ExpSysID == id).First().TypeID;
                binddatagrid(Convert.ToInt32(NameI.SelectedValue));
                binddatagrid(id);
            }
        }

        void BindComboBox()
        {
            var Sis = ExpertSystemV2Entities.GetContext().ExpSystems.ToList();
            NameI.ItemsSource = Sis;
            NameI.DisplayMemberPath = "NameSys";
            NameI.SelectedValuePath = "ExpSysID";
            binddatagrid(Convert.ToInt32(NameI.SelectedValue));
        }

        public void binddatagrid(int systemId)
        {
            {
                SelectedSys = systemId;
                if (ExpertSystemV2Entities.GetContext().ExpSystems.Where(r => r.ExpSysID == systemId).FirstOrDefault() != null)
                {

                    if (SystemType!=1)
                    {
                      
                        {
                            id = systemId;

                            // Получаем правила, привязанные к системе
                            var rules = ExpertSystemV2Entities.GetContext().Questions
                                .Where(r => r.ExpSysID == systemId)
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
                                rule.ExpSysID = systemId;
                            }
                        }
                    }
                    else {

                  
                        {
                            id = systemId;

                            // Получаем правила, привязанные к системе
                            var rules = ExpertSystemV2Entities.GetContext().Facts
                                .Where(r => r.ExpSysID == systemId)
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
                                rule.ExpSysID = systemId;
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


                    if (SystemType == 0) // linear sys
                    {
                        // Получаем измененные данные из DataGrid
                        var rules = DgLinear.ItemsSource as List<Questions>;

                        if (rules != null)
                        {
                            // Получаем ID существующей ExpSystem
                            foreach (var rule in rules)
                            {
                                // Проверяем, существует ли запись в базе данных
                                var existingRule = ExpertSystemV2Entities.GetContext().Questions
                                    .FirstOrDefault(r => r.ExpSysID == rule.ExpSysID);

                                if (existingRule != null)
                                {
                                    // Обновляем существующую запись
                                    ExpertSystemV2Entities.GetContext().Entry(existingRule).CurrentValues.SetValues(rule);
                                }
                                else
                                {
                                    // Устанавливаем правильный ID для нового правила
                                    var newRule = new Questions
                                    {
                                        ExpSysID = nameSisId,
                                        Text = rule.Text
                                    };
                                    ExpertSystemV2Entities.GetContext().Questions.Add(newRule);
                                }
                            }

                            // Сохраняем изменения в базе данных
                            ExpertSystemV2Entities.GetContext().SaveChanges();
                            binddatagrid(SelectedSys);

                        }
                    }
                    else
                    {
                        // Получаем измененные данные из DataGrid
                        var facts = DgWeight.ItemsSource as List<Facts>;

                        if (facts != null)
                        {
                            // Получаем ID существующей ExpSystem
                            foreach (var fact in facts)
                            {
                                // Проверяем, существует ли запись в базе данных
                                var existingRule = ExpertSystemV2Entities.GetContext().Facts
                                    .FirstOrDefault(r => r.ExpSysID == fact.ExpSysID);

                                if (existingRule != null) 
                                {
                                    // Обновляем существующую запись
                                    ExpertSystemV2Entities.GetContext().Entry(existingRule).CurrentValues.SetValues(fact);
                                }
                                else
                                {
                                    // Устанавливаем правильный ID для нового правила
                                    var newRule = new Facts
                                    {
                                        ExpSysID = nameSisId,
                                        Name = fact.Name,
                                        Description = fact.Description
                                    };
                                    ExpertSystemV2Entities.GetContext().Facts.Add(newRule);
                                }
                            }

                            // Сохраняем изменения в базе данных
                            ExpertSystemV2Entities.GetContext().SaveChanges();
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
                if (ExpertSystemV2Entities.GetContext().ExpSystems.Where(r => r.ExpSysID== nameSisId).FirstOrDefault().TypeID == 0)
                {
                    // Открываем окно с вопросами для выбранного правила
                    Questions selectedRuleId = DgLinear.SelectedValue as Questions;
                    GlobalDATA.SelectRULEID = selectedRuleId.QuestionID;
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
            var temp = NameI.SelectedItem as ExpSystems;
            var systemExists = ExpertSystemV2Entities.GetContext().ExpSystems.Any(ns => ns.NameSys == temp.NameSys);

            if (systemExists)
            {
                int t = Convert.ToInt32(NameI.SelectedValue);
                SystemType = ExpertSystemV2Entities.GetContext().ExpSystems.Where(r => r.ExpSysID == t).First().TypeID;
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
                var itemToDelete = ExpertSystemV2Entities.GetContext().ExpSystems.FirstOrDefault(x => x.ExpSysID == del);

                if (itemToDelete != null)
                {     

                    var result =new Wpf.Ui.Controls.MessageBox {  Content = "Удалить систему?",Title= "Подтверждение", PrimaryButtonText = "Дa", CloseButtonText = "Нет" }.ShowDialogAsync().Result;

                    if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
                    { 
                        result =new Wpf.Ui.Controls.MessageBox { Content = "Вы уверены, что хотите продолжить?", Title = "Подтверждение", PrimaryButtonText = "Дa", CloseButtonText = "Нет" }.ShowDialogAsync().Result;

                        if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
                        {
                            var factIds = itemToDelete.Facts.Select(f => f.FactID).ToList();
                            var relatedWeights = ExpertSystemV2Entities.GetContext().WeightAnswers.Where(wa => factIds.Contains(wa.FactID)).ToList();
                            ExpertSystemV2Entities.GetContext().WeightAnswers.RemoveRange(relatedWeights);
                            ExpertSystemV2Entities.GetContext().SaveChanges();
                            // Удалить объект из контекста
                            ExpertSystemV2Entities.GetContext().ExpSystems.Remove(itemToDelete);


                            // Сохранить изменения в базе данных
                            ExpertSystemV2Entities.GetContext().SaveChanges();
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
                SystemType = ExpertSystemV2Entities.GetContext().ExpSystems.Where(r => r.ExpSysID == systemId).First().TypeID;
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

                    if (SystemType!=1)
                    {
                        if (dataGridRow?.Item is Questions deleted)
                        {
                            var itemToDelete = ExpertSystemV2Entities.GetContext().Questions.FirstOrDefault(x => x.QuestionID == deleted.QuestionID);
                            ExpertSystemV2Entities.GetContext().Questions.Remove(itemToDelete);
                            ExpertSystemV2Entities.GetContext().SaveChanges();
                            binddatagrid(id);
                        }
                    }
                    else
                    {
                        if(dataGridRow?.Item is Facts deleted)
                        {
                            var itemToDelete = ExpertSystemV2Entities.GetContext().Facts.FirstOrDefault(x => x.FactID == deleted.FactID);
                            ExpertSystemV2Entities.GetContext().Facts.Remove(itemToDelete);
                            ExpertSystemV2Entities.GetContext().SaveChanges();
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

                var messagebox = new Wpf.Ui.Controls.MessageBox { CloseButtonText = "Ок", Content = $"Выбранный файл: {filePath}", Title = "Импорт" }.ShowDialogAsync();
                sqlJSON.ImportData(filePath);
               
            }
            SystemType = GlobalDATA.SystemType;
            NameI.SelectedIndex=NameI.Items.Count-1;
            BindComboBox();

        }

        private void export_Click(object sender, RoutedEventArgs e)
        {
            if (NameI.SelectedIndex != -1)
            {

                var resultm = new Wpf.Ui.Controls.MessageBox { Content = "Куда экспортировать?", Title = "Экспорт", PrimaryButtonText = "Сайт", SecondaryButtonText="ПК", CloseButtonText = "Отмена" }.ShowDialogAsync().Result;
                if (resultm == Wpf.Ui.Controls.MessageBoxResult.Secondary)
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
                else if(resultm == Wpf.Ui.Controls.MessageBoxResult.Primary)
                {
                   
                    sqlJSON.ExportData(Convert.ToInt32(NameI.SelectedValue), "wise-choice");
                    navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
                    navigateView.Navigate(typeof(LogIn));
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
                if (ExpertSystemV2Entities.GetContext().ExpSystems.Where(r => r.ExpSysID == nameSisId).FirstOrDefault().TypeID == 1)
                {
                    // Открываем окно с вопросами для выбранного правила
                    Facts selectedRuleId = DgWeight.SelectedValue as Facts;
                    GlobalDATA.SelectFACTID = selectedRuleId.FactID;
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