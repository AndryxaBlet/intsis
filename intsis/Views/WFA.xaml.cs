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
using Button = System.Windows.Controls.Button;
using System.Data.Entity;
using System.Xml;


namespace intsis
{
    /// <summary>
    /// Логика взаимодействия для ANS.xaml
    /// </summary>
    public partial class WFA : Page
    {
        private int id = 0;
        public ObservableCollection<Facts> Facts { get; set; }
        Wpf.Ui.Controls.NavigationView navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
        public WFA()
        {
            InitializeComponent();
            int ID = GlobalDATA.SelectANSID;
            int idsis = GlobalDATA.IdSisForCREATE;
            binddatagrid(ID);
            id = Convert.ToInt32(ID);
            var first = ExpertSystemV2Entities.GetContext().Facts.Where(x => x.ExpSysID == idsis).FirstOrDefault();
            Facts = new ObservableCollection<Facts>(ExpertSystemV2Entities.GetContext().Facts.Where(x => x.ExpSysID == idsis && x.FactID!=first.FactID));
            RuleTextBlock.Text = "Настройка ответа: " + ExpertSystemV2Entities.GetContext().Answers.Where(x => x.AnswerID == ID).FirstOrDefault()?.Text;
            // Устанавливаем DataContext для MainWindow, чтобы RuleOptions был доступен
            DataContext = this;

        }

        public void binddatagrid(int ID)
        {
            try
            {

                // Получаем ответы по Id
                var answersFromDb = ExpertSystemV2Entities.GetContext().WeightAnswers
                    .Where(a => a.AnswerID == ID)
                    .ToList();
                Dg.ItemsSource = answersFromDb; // Привязываем коллекцию к DataGrid
            }
            catch (Exception r)
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Ошибка", Content = r.Message};
                messagebox.ShowDialogAsync();

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //try
            {
                // Получаем измененные записи из DataGrid
                var answers = Dg.ItemsSource as List<WeightAnswers>;
                foreach (var answer in answers)
                {
                    answer.AnswerID = id;

                    // Проверяем, существует ли запись в базе данных
                    var existingAnswer = ExpertSystemV2Entities.GetContext().WeightAnswers
                        .FirstOrDefault(a => a.WAID == answer.WAID);

                    if (existingAnswer != null)
                    {
                        // Обновляем запись

                        existingAnswer.PlusOrMinus = answer.PlusOrMinus;
                        existingAnswer.Value = answer.Value;

                    }
                    else
                    {
                        // Добавляем новую запись
                        ExpertSystemV2Entities.GetContext().WeightAnswers.Add(answer);
                    }
                }


                // Сохраняем изменения в базе данных
                ExpertSystemV2Entities.GetContext().SaveChanges();

                // Повторно привязываем обновленные данные к DataGrid
                binddatagrid(id);

                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок",Content = "Обновление прошло успешно!"};
                messagebox.ShowDialogAsync();

            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedValue != null)
            {
                // Получаем текущий объект строки, к которому относится ComboBox
                
                System.Windows.Controls.TextBlock textBlock = new System.Windows.Controls.TextBlock();
                var dataGridRow = intsis.FUNC.FindParent<DataGridRow>(comboBox);

                if (dataGridRow?.Item is WeightAnswers answer)
                {
                    // Обновляем значение NextR для текущего элемента
                    answer.FactID = Convert.ToInt32(comboBox.SelectedValue);
                   

                    // Сохраняем изменения для текущего объекта в базе данных
                    var context = ExpertSystemV2Entities.GetContext();
                    var existingAnswer = context.WeightAnswers.FirstOrDefault(a => a.WAID == answer.WAID);
                    if (existingAnswer != null)
                    {

                        existingAnswer.FactID = answer.FactID;

                    }
                }
                else
                {
                    WeightAnswers answerr = new WeightAnswers();
                    answerr.FactID = Convert.ToInt32(comboBox.SelectedValue);
                    // Сохраняем изменения для текущего объекта в базе данных
                    var context = ExpertSystemV2Entities.GetContext();
                    answerr.AnswerID = id;
                    context.WeightAnswers.Add(answerr);
                    context.SaveChanges();
                    binddatagrid(id);

                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button)
                {
                    // Получаем текущий объект строки, к которому относится Button
                    var dataGridRow = intsis.FUNC.FindParent<DataGridRow>(button);
                    if (dataGridRow?.Item is WeightAnswers deleted)
                    {
                        // Находим объект для удаления в контексте
                        var itemToDelete = ExpertSystemV2Entities.GetContext().WeightAnswers
                            .FirstOrDefault(x => x.WAID == deleted.WAID);

                        if (itemToDelete != null)
                        {
                            // Удаляем объект
                            ExpertSystemV2Entities.GetContext().WeightAnswers.Remove(itemToDelete);

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
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Предупреждение", Content = "Сохраните изменения, прежде чем удалять." };
                messagebox.ShowDialogAsync();
            }
            catch (Exception ex)
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Ошибка", Content = ex.Message};
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
        private void ToggleSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggle && toggle.IsChecked != null)
            {
                // Получаем текущий объект строки, к которому относится ComboBox
                var dataGridRow = intsis.FUNC.FindParent<DataGridRow>(toggle);
                if (dataGridRow?.Item is WeightAnswers answer)
                {
                    var context = ExpertSystemV2Entities.GetContext();
                    var existingAnswer = context.WeightAnswers.FirstOrDefault(a => a.WAID == answer.WAID);
                    if (existingAnswer != null)
                    {
                        if (toggle.IsChecked == true)
                        {
                            existingAnswer.PlusOrMinus = true;
                        }
                        else
                            existingAnswer.PlusOrMinus = false;

                    }
                }
            }
        }

        private void NumberBox_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (sender is NumberBox nb)
            {
                // Получаем текущий объект строки, к которому относится ComboBox
                var dataGridRow = intsis.FUNC.FindParent<DataGridRow>(nb);
                if (dataGridRow?.Item is WeightAnswers answer)
                {
                    // Обновляем значение NextR для текущего элемента
                    answer.Value =Convert.ToDecimal(nb.Value);

                    // Сохраняем изменения для текущего объекта в базе данных
                    var context = ExpertSystemV2Entities.GetContext();
                    var existingAnswer = context.WeightAnswers.FirstOrDefault(a => a.WAID == answer.WAID);
                    if (existingAnswer != null)
                    {
                        existingAnswer.Value = answer.Value;
                    }
                }
            }

        }
    }
}