using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Windows.Data;

namespace intsis
{
    /// <summary>
    /// Логика взаимодействия для ANS.xaml
    /// </summary>
    public partial class ANS : Window
    {
        private int id = 0;
        public ObservableCollection<Rules> RuleOptions { get; set; }

        public ANS(string ID, int idsis)
        {
            InitializeComponent();
            binddatagrid(ID);
            id = Convert.ToInt32(ID);
            RuleOptions = new ObservableCollection<Rules>(intsisEntities.GetContext().Rules.Where(x=>x.IDSis==idsis));

            // Устанавливаем DataContext для MainWindow, чтобы RuleOptions был доступен
            DataContext = this;

        }
       
        public void binddatagrid(string ID)
        {
            try 
            { 
          
                // Получаем ответы по IDRule
                var answers = intsisEntities.GetContext().Answer
                    .Where(a => a.IDRule.ToString() == ID)
                    .ToList();

                // Привязываем данные к DataGrid
                Dg.ItemsSource = answers;

                // Устанавливаем значение по умолчанию для колонки IDRule
                foreach (var answer in answers)
                {
                    answer.IDRule = int.Parse(ID);
                }
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем измененные записи из DataGrid
                var answers = Dg.ItemsSource as List<Answer>;

                if (answers != null)
                {
                    foreach (var answer in answers)
                    {
                        // Проверяем, существует ли соответствующее правило
                        answer.IDRule = id;

                        // Проверяем, существует ли запись в базе данных
                        var existingAnswer = intsisEntities.GetContext().Answer
                            .FirstOrDefault(a => a.ID == answer.ID);

                        if (existingAnswer != null)
                        {
                            // Обновляем запись
                            intsisEntities.GetContext().Entry(existingAnswer).CurrentValues.SetValues(answer);
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
                    binddatagrid(id.ToString());

                    MessageBox.Show("Обновление прошло успешно");
                }
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);

            }
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
    }
}