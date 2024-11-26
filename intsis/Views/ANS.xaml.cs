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
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;
using CustomMessageBox = Wpf.Ui.Controls.MessageBox;

namespace intsis
{
    /// <summary>
    /// Логика взаимодействия для ANS.xaml
    /// </summary>
    public partial class ANS : Page
    {
        
        private int id = 0;
        public ObservableCollection<LinearSystem_Question> RuleOptions { get; set; }
        Wpf.Ui.Controls.NavigationView navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
        public ANS()
        {
            InitializeComponent();
            int ID = GlobalDATA.SelectRULEID;
            int idsis = GlobalDATA.IdSisForCREATE;
            binddatagrid(ID);
            id = Convert.ToInt32(ID);
            RuleOptions = new ObservableCollection<LinearSystem_Question>(ExpertSystemEntities.GetContext().LinearSystem_Question.Where(x=>x.SystemId==idsis));
            RuleTextBlock.Text="Настройка вопроса: "+ExpertSystemEntities.GetContext().LinearSystem_Question.Where(x=>x.Id==ID).FirstOrDefault()?.Text;
            // Устанавливаем DataContext для MainWindow, чтобы RuleOptions был доступен
            DataContext = this;

        }
       
        public void binddatagrid(int ID)
        {
            try 
            {
                

                // Получаем ответы по Id
                var answersFromDb = ExpertSystemEntities.GetContext().LinearSystem_Answer
                    .Where(a => a.QuestionId == ID)
                    .ToList();
                Dg.ItemsSource = answersFromDb; // Привязываем коллекцию к DataGrid
            }
            catch (Exception r)
            {
                var messagebox = new Wpf.Ui.Controls.MessageBox { Title = "Ошибка", Content = r.Message };
                messagebox.ShowDialogAsync();

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //try
            {
                // Получаем измененные записи из DataGrid
                var answers = Dg.ItemsSource as List<LinearSystem_Answer>;
                foreach (var answer in answers)
                    {
                        answer.QuestionId = id;

                        // Проверяем, существует ли запись в базе данных
                        var existingAnswer = ExpertSystemEntities.GetContext().LinearSystem_Answer
                            .FirstOrDefault(a => a.Id == answer.Id);

                        if (existingAnswer != null)
                        {
                        // Обновляем запись

                        existingAnswer.Text = answer.Text;
                        existingAnswer.NextQuestionId = answer.NextQuestionId;
                        existingAnswer.Recomendation = answer.Recomendation;
                       
                        }
                        else
                        {
                        // Добавляем новую запись
                        ExpertSystemEntities.GetContext().LinearSystem_Answer.Add(answer);
                        }
                    }
                    

                    // Сохраняем изменения в базе данных
                    ExpertSystemEntities.GetContext().SaveChanges();

                    // Повторно привязываем обновленные данные к DataGrid
                    binddatagrid(id);

                    var messagebox = new Wpf.Ui.Controls.MessageBox {Content = "Обновление прошло успешно." };
                    messagebox.ShowDialogAsync();
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
                var itemToDelete = ExpertSystemEntities.GetContext().LinearSystem_Question.FirstOrDefault(x => x.Id == del);

                if (itemToDelete != null)
                {
                    MessageBoxResult result = MessageBox.Show("Удалить вопрос?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        result = MessageBox.Show("Вы уверены, что хотите продолжить? Восстановить вопрос невозможно", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            // Удалить объект из контекста
                            ExpertSystemEntities.GetContext().LinearSystem_Question.Remove(itemToDelete);

                            // Сохранить изменения в базе данных
                            ExpertSystemEntities.GetContext().SaveChanges();
                            navigateView.GoBack();
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
                int del = (Dg.ItemsSource as List<LinearSystem_Answer>)[Dg.SelectedIndex].Id;
                var itemToDelete = ExpertSystemEntities.GetContext().LinearSystem_Answer.FirstOrDefault(x => x.Id == del);

                if (itemToDelete != null)
                {
                    MessageBoxResult result = MessageBox.Show("Удалить ответ на вопрос?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        result = MessageBox.Show("Вы уверены, что хотите продолжить? Восстановить ответ невозможно", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            // Удалить объект из контекста
                            ExpertSystemEntities.GetContext().LinearSystem_Answer.Remove(itemToDelete);

                            // Сохранить изменения в базе данных
                            ExpertSystemEntities.GetContext().SaveChanges();
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
                if (dataGridRow?.Item is LinearSystem_Answer answer)
                {
                    // Обновляем значение NextR для текущего элемента
                    answer.NextQuestionId = Convert.ToInt32(comboBox.SelectedValue);

                        // Сохраняем изменения для текущего объекта в базе данных
                        var context = ExpertSystemEntities.GetContext();
                        var existingAnswer = context.LinearSystem_Answer.FirstOrDefault(a => a.Id == answer.Id);
                        if (existingAnswer != null)
                        {

                            existingAnswer.NextQuestionId = answer.NextQuestionId;

                        }
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is System.Windows.Controls.Button button)
                {
                    // Получаем текущий объект строки, к которому относится Button
                    var dataGridRow = intsis.FUNC.FindParent<DataGridRow>(button);
                    if (dataGridRow?.Item is LinearSystem_Answer deleted)
                    {
                        // Находим объект для удаления в контексте
                        var itemToDelete = ExpertSystemEntities.GetContext().LinearSystem_Answer
                            .FirstOrDefault(x => x.Id == deleted.Id);

                        if (itemToDelete != null)
                        {
                            // Удаляем объект
                            ExpertSystemEntities.GetContext().LinearSystem_Answer.Remove(itemToDelete);

                            // Сохраняем изменения в базе данных
                            ExpertSystemEntities.GetContext().SaveChanges();

                            // Обновляем DataGrid
                            binddatagrid(id); // Используйте корректный идентификатор системы
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
                var messagebox = new Wpf.Ui.Controls.MessageBox {Content = "Сохраните изменения, прежде чем удалять." };
                messagebox.ShowDialogAsync();
            }
            catch (Exception ex)
            {
                var messagebox = new Wpf.Ui.Controls.MessageBox { Title = "Ошибка", Content = ex.Message };
                messagebox.ShowDialogAsync();
            }

        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Определяем максимальные и минимальные размеры
            double minWidth = 800;  // минимальная ширина для нормального вида
            double minHeight = 600; // минимальная высота для нормального вида


            // Рассчитываем коэффициент масштаба в зависимости от размера окна
            double scaleFactor = Math.Min(e.NewSize.Width / minWidth, e.NewSize.Height / minHeight);

            // Ограничиваем минимальный и максимальный коэффициент масштаба
            scaleFactor = Math.Max(0.5, scaleFactor);  // минимальный размер 50%
            scaleFactor = Math.Min(1.0, scaleFactor);  // максимальный размер 150%

            // Применяем масштаб к элементам
            scaleTransform.ScaleX = scaleFactor;
            scaleTransform.ScaleY = scaleFactor;
        }
    }
}