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
        public ObservableCollection<WeightedSystem_Fact> Facts { get; set; }
        Wpf.Ui.Controls.NavigationView navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
        public WFA()
        {
            InitializeComponent();
            int ID = GlobalDATA.SelectANSID;
            int idsis = GlobalDATA.IdSisForCREATE;
            binddatagrid(ID);
            id = Convert.ToInt32(ID);
            var first = ExpertSystemEntities.GetContext().WeightedSystem_Fact.Where(x => x.SystemId == idsis).FirstOrDefault();
            Facts = new ObservableCollection<WeightedSystem_Fact>(ExpertSystemEntities.GetContext().WeightedSystem_Fact.Where(x => x.SystemId == idsis && x.Id!=first.Id));
            RuleTextBlock.Text = "Настройка ответа: " + ExpertSystemEntities.GetContext().WeightedSystem_Answer.Where(x => x.Id == ID).FirstOrDefault()?.Text;
            // Устанавливаем DataContext для MainWindow, чтобы RuleOptions был доступен
            DataContext = this;

        }

        public void binddatagrid(int ID)
        {
            try
            {

                // Получаем ответы по Id
                var answersFromDb = ExpertSystemEntities.GetContext().WeightFactAnswer
                    .Where(a => a.IdAnswer == ID)
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
                var answers = Dg.ItemsSource as List<WeightFactAnswer>;
                foreach (var answer in answers)
                {
                    answer.IdAnswer = id;

                    // Проверяем, существует ли запись в базе данных
                    var existingAnswer = ExpertSystemEntities.GetContext().WeightFactAnswer
                        .FirstOrDefault(a => a.Id == answer.Id);

                    if (existingAnswer != null)
                    {
                        // Обновляем запись

                        existingAnswer.PlusOrMinus = answer.PlusOrMinus;
                        existingAnswer.Weight = answer.Weight;

                    }
                    else
                    {
                        // Добавляем новую запись
                        ExpertSystemEntities.GetContext().WeightFactAnswer.Add(answer);
                    }
                }


                // Сохраняем изменения в базе данных
                ExpertSystemEntities.GetContext().SaveChanges();

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

                if (dataGridRow?.Item is WeightFactAnswer answer)
                {
                    // Обновляем значение NextR для текущего элемента
                    answer.IdFact = Convert.ToInt32(comboBox.SelectedValue);
                   

                    // Сохраняем изменения для текущего объекта в базе данных
                    var context = ExpertSystemEntities.GetContext();
                    var existingAnswer = context.WeightFactAnswer.FirstOrDefault(a => a.Id == answer.Id);
                    if (existingAnswer != null)
                    {

                        existingAnswer.IdFact = answer.IdFact;

                    }
                }
                else
                {
                    WeightFactAnswer answerr = new WeightFactAnswer();
                    answerr.IdFact = Convert.ToInt32(comboBox.SelectedValue);
                    // Сохраняем изменения для текущего объекта в базе данных
                    var context = ExpertSystemEntities.GetContext();
                    answerr.IdAnswer = id;
                    context.WeightFactAnswer.Add(answerr);
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
                    if (dataGridRow?.Item is WeightFactAnswer deleted)
                    {
                        // Находим объект для удаления в контексте
                        var itemToDelete = ExpertSystemEntities.GetContext().WeightFactAnswer
                            .FirstOrDefault(x => x.Id == deleted.Id);

                        if (itemToDelete != null)
                        {
                            // Удаляем объект
                            ExpertSystemEntities.GetContext().WeightFactAnswer.Remove(itemToDelete);

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
                if (dataGridRow?.Item is WeightFactAnswer answer)
                {
                    var context = ExpertSystemEntities.GetContext();
                    var existingAnswer = context.WeightFactAnswer.FirstOrDefault(a => a.Id == answer.Id);
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
                if (dataGridRow?.Item is WeightFactAnswer answer)
                {
                    // Обновляем значение NextR для текущего элемента
                    answer.Weight =Convert.ToDecimal(nb.Value);

                    // Сохраняем изменения для текущего объекта в базе данных
                    var context = ExpertSystemEntities.GetContext();
                    var existingAnswer = context.WeightFactAnswer.FirstOrDefault(a => a.Id == answer.Id);
                    if (existingAnswer != null)
                    {
                        existingAnswer.Weight = answer.Weight;
                    }
                }
            }

        }
    }
}