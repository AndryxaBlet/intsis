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
        public ObservableCollection<Questions> RuleOptions { get; set; }
        Wpf.Ui.Controls.NavigationView navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
        public ANS()
        {
            InitializeComponent();
            int ID = GlobalDATA.SelectRULEID;
            int idsis = GlobalDATA.IdSisForCREATE;
            binddatagrid(ID);
            id = Convert.ToInt32(ID);
            RuleOptions = new ObservableCollection<Questions>(ExpertSystemV2Entities.GetContext().Questions.Where(x=>x.ExpSysID==idsis));
            RuleTextBlock.Text="Настройка вопроса: "+ ExpertSystemV2Entities.GetContext().Questions.Where(x => x.ExpSysID == ID).FirstOrDefault()?.Text;
            // Устанавливаем DataContext для MainWindow, чтобы RuleOptions был доступен
            DataContext = this;

        }
       
        public void binddatagrid(int ID)
        {
            try 
            {
                

                // Получаем ответы по Id
                var answersFromDb = ExpertSystemV2Entities.GetContext().Answers
                    .Where(a => a.QuestionID == ID)
                    .ToList();
                Dg.ItemsSource = answersFromDb; // Привязываем коллекцию к DataGrid
            }
            catch (Exception r)
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Ошибка", Content = r.Message };
                messagebox.ShowDialogAsync();

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем измененные записи из DataGrid
                var answers = Dg.ItemsSource as List<Answers>;
                foreach (var answer in answers)
                    {
                        answer.QuestionID = id;

                        // Проверяем, существует ли запись в базе данных
                        var existingAnswer = ExpertSystemV2Entities.GetContext().Answers
                            .FirstOrDefault(a => a.AnswerID == answer.AnswerID);

                        if (existingAnswer != null)
                        {
                        // Обновляем запись

                        existingAnswer.Text = answer.Text;
                        existingAnswer.NextQuestion = answer.NextQuestion;
                        existingAnswer.Recommendation = answer.Recommendation;
                       
                        }
                        else
                        {
                        // Добавляем новую запись
                        ExpertSystemV2Entities.GetContext().Answers.Add(answer);
                        }
                    }


                // Сохраняем изменения в базе данных
                     ExpertSystemV2Entities.GetContext().SaveChanges();

                    // Повторно привязываем обновленные данные к DataGrid
                    binddatagrid(id);

                    var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок",Content = "Обновление прошло успешно." };
                    messagebox.ShowDialogAsync();
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
                var itemToDelete = ExpertSystemV2Entities.GetContext().Questions.FirstOrDefault(x => x.QuestionID == del);

                if (itemToDelete != null)
                {
                    var result = new Wpf.Ui.Controls.MessageBox { Content = "Удалить вопрос?", Title = "Подтверждение", PrimaryButtonText = "Дa", CloseButtonText = "Нет" }.ShowDialogAsync().Result;

                    if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
                    {
                        result = new Wpf.Ui.Controls.MessageBox { Content = "Вы уверены, что хотите продолжить?", Title = "Подтверждение", PrimaryButtonText = "Дa", CloseButtonText = "Нет" }.ShowDialogAsync().Result;

                        if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
                        {
                            // Удалить объект из контекста
                            ExpertSystemV2Entities.GetContext().Questions.Remove(itemToDelete);

                            // Сохранить изменения в базе данных
                            ExpertSystemV2Entities.GetContext().SaveChanges();
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
                int del = (Dg.ItemsSource as List<Answers>)[Dg.SelectedIndex].AnswerID;
                var itemToDelete = ExpertSystemV2Entities.GetContext().Answers.FirstOrDefault(x => x.AnswerID == del);

                if (itemToDelete != null)
                {
                    var result = new Wpf.Ui.Controls.MessageBox { Content = "Удалить ответ на вопрос?", Title = "Подтверждение", PrimaryButtonText = "Дa", CloseButtonText = "Нет" }.ShowDialogAsync().Result;

                    if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
                    {
                        result = new Wpf.Ui.Controls.MessageBox { Content = "Вы уверены, что хотите продолжить?", Title = "Подтверждение", PrimaryButtonText = "Дa", CloseButtonText = "Нет" }.ShowDialogAsync().Result;

                        if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
                        {

                            // Удалить объект из контекста
                            ExpertSystemV2Entities.GetContext().Answers.Remove(itemToDelete);

                            // Сохранить изменения в базе данных
                            ExpertSystemV2Entities.GetContext().SaveChanges();
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
                if (dataGridRow?.Item is Answers answer)
                {
                    // Обновляем значение NextR для текущего элемента
                    answer.NextQuestion = Convert.ToInt32(comboBox.SelectedValue);

                        // Сохраняем изменения для текущего объекта в базе данных
                        var context = ExpertSystemV2Entities.GetContext();
                        var existingAnswer = context.Answers.FirstOrDefault(a => a.AnswerID== answer.AnswerID);
                        if (existingAnswer != null)
                        {

                            existingAnswer.NextQuestion = answer.NextQuestion;

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
                    if (dataGridRow?.Item is Answers deleted)
                    {
                        // Находим объект для удаления в контексте
                        var itemToDelete = ExpertSystemV2Entities.GetContext().Answers
                            .FirstOrDefault(x => x.AnswerID== deleted.AnswerID);

                        if (itemToDelete != null)
                        {
                            // Удаляем объект
                            ExpertSystemV2Entities.GetContext().Answers.Remove(itemToDelete);

                            // Сохраняем изменения в базе данных
                            ExpertSystemV2Entities.GetContext().SaveChanges();

                            // Обновляем DataGrid
                            binddatagrid(id); // Используйте корректный идентификатор системы
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок",Content = "Сохраните изменения, прежде чем удалять." };
                messagebox.ShowDialogAsync();
            }
            catch (Exception ex)
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Ошибка", Content = ex.Message };
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