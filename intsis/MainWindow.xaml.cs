﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;
using CustomMessageBox = Wpf.Ui.Controls.MessageBox;

namespace intsis
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow
    {
        public MainWindow(bool admin)
        {
            InitializeComponent();
            binddatagrid();
            if (!admin) {
                Create.Visibility = Visibility.Hidden;
            }
            
        }
        public void binddatagrid()
        {
            try
            {
                Dg.ItemsSource = intsisEntities.GetContext().NameSis.ToList();
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRuleId = -1;
                if (Dg.SelectedIndex != -1)
                {
                    selectedRuleId = (Dg.ItemsSource as List<NameSis>)[Dg.SelectedIndex].ID;
                }
                Create_Window window = new Create_Window(selectedRuleId);
                window.ShowDialog();
                binddatagrid();
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Dg.SelectedIndex != -1)
                {
                    NameSis selectedRuleId = Dg.SelectedValue as NameSis;
                    Answers answers = new Answers(selectedRuleId.ID);
                    answers.Title=selectedRuleId.Name;
                    answers.ShowDialog();
                }
                else MessageBox.Show("Выберите систему", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Нужно выбрать существующий набор вопросов", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }
            
        }

        private void Grid_Click(object sender, MouseEventArgs e)
        {
            if (Dg.IsFocused && !Dg.IsMouseOver)
            {
                Dg.SelectedIndex = -1;
            }
        }

        private void Dg_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
                Dg.SelectedIndex = -1;
            
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Dg.IsFocused && !Dg.IsMouseOver)
            {
                Dg.SelectedIndex = -1;
            }
        }

        private void Dg_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Dg.SelectedItem = null;
        }
    }
}
