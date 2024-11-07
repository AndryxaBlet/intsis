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


namespace intsis
{
    /// <summary>
    /// Логика взаимодействия для Create_Window.xaml
    /// </summary>
    public partial class Create_Window : FluentWindow
    {
        private int id = 0;
        private int SelectedSys = -1;

        public Create_Window(int id)
        {   
            InitializeComponent();
            BindComboBox();
            Dg.Visibility = Visibility.Hidden;
            if (id != -1)
            {                
                NameI.SelectedValue = id;
                binddatagrid(Convert.ToInt32(NameI.SelectedValue));
                binddatagrid(id);
                Dg.Visibility = Visibility.Visible;
            }
        }
        SqlJSON sqlJSON = new SqlJSON();

        void BindComboBox()
        {
            var Sis = intsisEntities.GetContext().NameSis.ToList();
            NameI.ItemsSource = Sis;
            NameI.DisplayMemberPath = "Name";
            NameI.SelectedValuePath = "ID";
            Dg.Visibility = Visibility.Visible;
            binddatagrid(Convert.ToInt32(NameI.SelectedValue));
        }

        public void binddatagrid(int systemId)
        {

            {
                SelectedSys = systemId;
                if (systemId != 0)
                {
                    id = systemId;

                    // Получаем правила, привязанные к системе
                    var rules = intsisEntities.GetContext().Rules
                        .Where(r => r.IDSis == systemId)
                        .ToList();

                    // Привязываем данные к DataGrid
                    Dg.ItemsSource = rules;

                    // Устанавливаем значение по умолчанию для колонки IDSis
                    foreach (var rule in rules)
                    {
                        rule.IDSis = systemId;
                    }
                }
            }
        }


        public void Create_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (Dg.SelectedIndex != -1)
            {
                if (SelectedSys != -1)
                {
                    // Получаем измененные данные из DataGrid
                    var rules = Dg.ItemsSource as List<Rules>;

                    if (rules != null)
                    {
                        // Получаем ID существующей NameSis
                        var nameSisId = SelectedSys;


                        foreach (var rule in rules)
                        {
                            // Проверяем, существует ли запись в базе данных
                            var existingRule = intsisEntities.GetContext().Rules
                                .FirstOrDefault(r => r.IDRule == rule.IDRule);

                            if (existingRule != null)
                            {
                                // Обновляем существующую запись
                                intsisEntities.GetContext().Entry(existingRule).CurrentValues.SetValues(rule);
                            }
                            else
                            {
                                // Устанавливаем правильный ID для нового правила
                                rule.IDSis = nameSisId; // Устанавливаем ID существующей NameSis
                                intsisEntities.GetContext().Rules.Add(rule);
                            }
                        }

                        // Сохраняем изменения в базе данных
                        intsisEntities.GetContext().SaveChanges();
                        binddatagrid(SelectedSys);
                    }
                }
                else
                {
                    MessageBox.Show("Не выбранна система");
                }
            }
            else
            {
                MessageBox.Show("Выберите систему", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }


        private void C(object sender, RoutedEventArgs e)
        {
            if (Dg.SelectedIndex != -1)
            {
                // Открываем окно с вопросами для выбранного правила
                Rules selectedRuleId = Dg.SelectedValue as Rules;
                int r = selectedRuleId.IDRule;
                int IDSYSTEM = id;
                ANS ans = new ANS(r,id);
                ans.Show();
                binddatagrid(Convert.ToInt32(NameI.SelectedValue));
            }
            else
            {
                MessageBox.Show("Выберите систему", "",MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void NameI_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Проверяем, существует ли система с таким именем
            var systemExists = intsisEntities.GetContext().NameSis.Any(ns => ns.Name == NameI.Text);

            if (systemExists)
            {
                MessageBox.Show("Редактирование данных");
                binddatagrid(Convert.ToInt32(NameI.SelectedValue));
                SelectedSys = Convert.ToInt32(NameI.SelectedValue);
            }
            BindComboBox();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (NameI.SelectedIndex != -1)
            {
                int del = Convert.ToInt32(NameI.SelectedValue);
                var itemToDelete = intsisEntities.GetContext().NameSis.FirstOrDefault(x => x.ID == del);

                if (itemToDelete != null)
                {
                    MessageBoxResult result = MessageBox.Show("Удалить систему?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        result = MessageBox.Show("Вы уверены, что хотите продолжить? Восстановить систему невозможно", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            // Удалить объект из контекста
                            intsisEntities.GetContext().NameSis.Remove(itemToDelete);

                            // Сохранить изменения в базе данных
                            intsisEntities.GetContext().SaveChanges();
                            this.Close();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите систему", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CreateSystem_Click(object sender, RoutedEventArgs e)
        {
            CreateSis createSis = new CreateSis(-1);
            
            bool? result = createSis.ShowDialog();

            if (result == true) // Проверяем, что окно закрыто с успешным результатом
            {
                int systemId = CreateSis.SystemId;
                binddatagrid(systemId);
                BindComboBox();
                NameI.SelectedValue = systemId;
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
                    if (dataGridRow?.Item is Rules deleted)
                    {
                        var itemToDelete = intsisEntities.GetContext().Rules.FirstOrDefault(x => x.IDRule == deleted.IDRule);
                        intsisEntities.GetContext().Rules.Remove(itemToDelete);
                        intsisEntities.GetContext().SaveChanges();
                        binddatagrid(id);
                    }
                }
            }
            catch(ArgumentNullException)
            {
                MessageBox.Show("Примените изменения, прежде чем удалять.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Выберите систему для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Выберите систему для экспорта","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            }

        }
    }
}