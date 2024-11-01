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

namespace intsis
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(bool admin)
        {
            InitializeComponent();
            binddatagrid();
            if (!admin) {
                Create.Visibility = Visibility.Hidden;
            }
            
        }
        
        string connect = Properties.Settings.Default.NotebookSQL2;
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
                    var selectedRuleId = (Dg.ItemsSource as List<NameSis>)[Dg.SelectedIndex].ID;
                    Answers answers = new Answers(selectedRuleId);
                    answers.ShowDialog();
                }
                else MessageBox.Show("Выберите систему");
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }
        }
    }
}
