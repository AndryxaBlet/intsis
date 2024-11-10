﻿using System;
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
    public partial class Answers : Page
    {
        Wpf.Ui.Controls.NavigationView navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
        public Answers()
        {
            int ID = GlobalDATA.IdSisForCREATE;
            InitializeComponent();
            next=FIRST(ID);
            log = ID;
        }
        int next = 0;
        int log = 0;
    

        public int FIRST(int id)
        {
                // Получаем текст правила по IDSis
                var ruleText = intsisEntities.GetContext().Rules
                    .Where(r => r.IDSis == id)
                    .Select(r => r.Text)
                    .FirstOrDefault();

                // Получаем IDRule по IDSis
                var ruleID = intsisEntities.GetContext().Rules
                    .Where(r => r.IDSis == id)
                    .Select(r => r.IDRule)
                    .FirstOrDefault();

                // Устанавливаем текст вопроса в интерфейсе
                VOP.Text = ruleText;

                // Обновляем элементы ComboBox
                UpdateItems(ruleID);

                return ruleID;

        }
        private void UpdateItems(int id)
        {
            try
            {
                // Получаем список ответов по IDRule
                var answers = intsisEntities.GetContext().Answer
                    .Where(a => a.IDRule == id)
                    .Select(a => new { a.ID, a.Ans })
                    .ToList();

                // Присваиваем данные источнику ComboBox
                CB.ItemsSource = answers;
                CB.DisplayMemberPath = "Ans";
                CB.SelectedValuePath = "ID";

            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }
        }

        public void BNext()
        {
            try
            {
                if (CB.SelectedIndex != -1)
                {
                    var selectedValue = CB.SelectedValue.ToString();
                    int.TryParse(selectedValue, out int sv);

                    if (selectedValue != null)
                    {

                        // Получаем значение поля NextR
                        var nextValue = intsisEntities.GetContext().Answer
                            .Where(a => a.ID == sv)
                            .FirstOrDefault();


                      
                        if (nextValue.Out == "" || nextValue.Out == null)
                        {
                            next = int.Parse(nextValue.NextR.ToString());
                            // Получаем текст ответа
                            var rec = intsisEntities.GetContext().Answer
                                .Where(a => a.ID == sv)
                                .Select(a => a.Rec)
                                .FirstOrDefault();

                            if (!string.IsNullOrEmpty(rec))
                            {
                                MessageBox.Show(rec);
                            }

                            // Получаем текст следующего вопроса
                            var nextText = intsisEntities.GetContext().Rules
                                .Where(r => r.IDRule == next)
                                .Select(r => r.Text)
                                .FirstOrDefault();

                            VOP.Text = nextText;
                            UpdateItems(next);
                        }
                        else
                        {
                            // Если это не число, выводим строковое сообщение
                            VOP.Text = nextValue.Out.ToString();
                            CB.Visibility = Visibility.Hidden;
                            Deny.Visibility = Visibility.Hidden;
                            Repeat.Visibility = Visibility.Visible;
                        }

                    }
                }
                else
                { MessageBox.Show("Выберите вариант ответа.", "", MessageBoxButton.OK, MessageBoxImage.Warning); }
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            navigateView.GoBack();
        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                next = FIRST(log);
                CB.Visibility = Visibility.Visible;
                Deny.Visibility = Visibility.Visible;
                Repeat.Visibility = Visibility.Hidden;
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }
        }
        private void Deny_Click(object sender, RoutedEventArgs e)
        {
            try 
            { 
                BNext();
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }
        }
    }
}
