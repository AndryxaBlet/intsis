using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;

namespace intsis
{
    /// <summary>
    /// Логика взаимодействия для ANS.xaml
    /// </summary>
    public partial class ANS : Window
    {
        private int id = 0;
        public ObservableCollection<Rules> RuleOptions { get; set; }

        public ANS(int ID, int idsis)
        {
            InitializeComponent();
            binddatagrid(ID);
            id = Convert.ToInt32(ID);
            RuleOptions = new ObservableCollection<Rules>(intsisEntities.GetContext().Rules.Where(x=>x.IDSis==idsis));

            // Устанавливаем DataContext для MainWindow, чтобы RuleOptions был доступен
            DataContext = this;

        }
       
        public void binddatagrid(int ID)
        {
            try 
            { 
          
                // Получаем ответы по IDRule
                var answersFromDb = intsisEntities.GetContext().Answer
                    .Where(a => a.IDRule == ID)
                    .ToList();
                Dg.ItemsSource = answersFromDb; // Привязываем коллекцию к DataGrid
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //try
            {
                // Получаем измененные записи из DataGrid
                var answers = Dg.ItemsSource as List<Answer>;
                foreach (var answer in answers)
                    {
                        answer.IDRule = id;

                        // Проверяем, существует ли запись в базе данных
                        var existingAnswer = intsisEntities.GetContext().Answer
                            .FirstOrDefault(a => a.ID == answer.ID);

                        if (existingAnswer != null)
                        {
                        // Обновляем запись

                        existingAnswer.IDRule = answer.IDRule;
                        existingAnswer.Rules = answer.Rules;
                        existingAnswer.Rec = answer.Rec;
                       
                        }
                        else
                        {
                        // Добавляем новую запись
                        intsisEntities.GetContext().Answer.Add(answer);
                        }
                    }
                    

                    // Сохраняем изменения в базе данных
                    intsisEntities.GetContext().SaveChanges();

                    // Повторно привязываем обновленные данные к DataGrid
                    binddatagrid(id);

                    MessageBox.Show("Обновление прошло успешно");
                
            }
            //catch (Exception r)
            //{
            //    MessageBox.Show(r.Message);

            //}
        }

        private void DelV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int del = id;
                var itemToDelete = intsisEntities.GetContext().Rules.FirstOrDefault(x => x.IDRule == del);

                if (itemToDelete != null)
                {
                    MessageBoxResult result = MessageBox.Show("Удалить вопрос?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        result = MessageBox.Show("Вы уверены, что хотите продолжить? Восстановить вопрос невозможно", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            // Удалить объект из контекста
                            intsisEntities.GetContext().Rules.Remove(itemToDelete);

                            // Сохранить изменения в базе данных
                            intsisEntities.GetContext().SaveChanges();
                            this.Close();
                        }
                    }
                }
            }
            
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }
        }

        private void DelO_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int del = (Dg.ItemsSource as List<Answer>)[Dg.SelectedIndex].ID;
                var itemToDelete = intsisEntities.GetContext().Answer.FirstOrDefault(x => x.ID == del);

                if (itemToDelete != null)
                {
                    MessageBoxResult result = MessageBox.Show("Удалить ответ на вопрос?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        result = MessageBox.Show("Вы уверены, что хотите продолжить? Восстановить ответ невозможно", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            // Удалить объект из контекста
                            intsisEntities.GetContext().Answer.Remove(itemToDelete);

                            // Сохранить изменения в базе данных
                            intsisEntities.GetContext().SaveChanges();
                        }
                    }
                }
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }
        }

      

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedValue != null)
            {
                // Получаем текущий объект строки, к которому относится ComboBox
                var dataGridRow = intsis.FUNC.FindParent<DataGridRow>(comboBox);
                if (dataGridRow?.Item is Answer answer)
                {
                    // Обновляем значение NextR для текущего элемента
                    answer.NextR = Convert.ToInt32(comboBox.SelectedValue);

                        // Сохраняем изменения для текущего объекта в базе данных
                        var context = intsisEntities.GetContext();
                        var existingAnswer = context.Answer.FirstOrDefault(a => a.ID == answer.ID);
                        if (existingAnswer != null)
                        {

                            existingAnswer.NextR = answer.NextR;

                        }
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                // Получаем текущий объект строки, к которому относится ComboBox
                var dataGridRow = intsis.FUNC.FindParent<DataGridRow>(button);
                if (dataGridRow?.Item is Answer deleted)
                {
                    var itemToDelete = intsisEntities.GetContext().Answer.FirstOrDefault(x => x.ID == deleted.ID);
                    itemToDelete.IDRule = id;
                    intsisEntities.GetContext().Answer.Remove(itemToDelete);
                    intsisEntities.GetContext().SaveChanges();
                    binddatagrid(id);
                }
            }
        }
    }
}