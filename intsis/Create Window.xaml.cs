﻿using System;
using System.Linq;
using System.Windows;
using System.Data.Entity;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;


namespace intsis
{
    /// <summary>
    /// Логика взаимодействия для Create_Window.xaml
    /// </summary>
    public partial class Create_Window : Window
    {
        private int id = 0;
        private int SelectedSys=-1;

        public Create_Window(int id)
        {
            InitializeComponent();
            BindComboBox();
            if (id != -1) 
            { 
                NameI.SelectedValue = id;
                binddatagrid(id);
            }
            if (NameI.SelectedIndex == -1)
                UpdateSis.IsEnabled = false;
            else
                UpdateSis.IsEnabled = true;
        }
        void BindComboBox()
        {
            var Sis = intsisEntities.GetContext().NameSis.ToList();
            NameI.ItemsSource = Sis;
            NameI.DisplayMemberPath = "Name";
            NameI.SelectedValuePath = "ID";
        }
        
        public void binddatagrid(int systemId)
        {
         
            {
                // Получаем ID системы по имени
                //var systemId = intsisEntities.GetContext().NameSis
                //    .Where(ns => ns.Name == name)
                //    .Select(ns => ns.ID)
                //    .FirstOrDefault();
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

        private void Create_Click(object sender, RoutedEventArgs e)
        {
          
                // Проверяем, существует ли система с таким именем
                var systemExists = intsisEntities.GetContext().NameSis
                    .Any(ns => ns.Name == NameI.Text);

                if (systemExists)
                {
                    MessageBox.Show("Редактирование данных");
                    binddatagrid(Convert.ToInt32(NameI.SelectedValue));
                    SelectedSys = Convert.ToInt32(NameI.SelectedValue);
                }
            BindComboBox();

        }

        public void Create_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSys != -1)
            {
                // Получаем измененные данные из DataGrid
                var rules = Dg.ItemsSource as List<Rules>;

                if (rules != null)
                {
                    // Получаем ID существующей NameSis
                    var nameSisId = SelectedSys;

                    // Проверяем, существует ли NameSis
                    if (nameSisId == 0)
                    {
                        MessageBox.Show("Система не найдена. Убедитесь, что система существует перед добавлением правил.");
                        return; // Прекращаем выполнение, если NameSis не найден
                    }

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

                    MessageBox.Show("Update successful");
                }
            } else {
                MessageBox.Show("Не выбранна система");
            }
            
        }


        private void C(object sender, RoutedEventArgs e)
        {
            if (Dg.SelectedIndex != -1)
            {
                // Открываем окно с вопросами для выбранного правила
                var selectedRuleId = (Dg.ItemsSource as List<Rules>)[Dg.SelectedIndex].IDRule;
                ANS ans = new ANS(selectedRuleId.ToString());
                ans.Show();
            }
            else
            {
                MessageBox.Show("Выберите систему");
            }
        }

        private void NameI_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (NameI.SelectedIndex == -1)
                UpdateSis.IsEnabled = false;
            else
                UpdateSis.IsEnabled = true;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
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
                    }
                }
            }
        }

        private void CreateSystem_Click(object sender, RoutedEventArgs e)
        {
            CreateSis createSis = new CreateSis();
            createSis.ShowDialog();
        }
    }
}