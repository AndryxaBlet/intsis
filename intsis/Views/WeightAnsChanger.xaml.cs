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
    public partial class WeightAnsChanger : Page
    {
        private int id = 0;
        Wpf.Ui.Controls.NavigationView navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
        public WeightAnsChanger()
        {
            InitializeComponent();
            int ID = GlobalDATA.SelectRULEID;
            int idsis = GlobalDATA.IdSisForCREATE;
            binddatagrid(ID);
            id = Convert.ToInt32(ID);
            RuleTextBlock.Text = "Настройка вопроса: " + ExpertSystemEntities.GetContext().WeightedSystem_Question.Where(x => x.Id == ID).FirstOrDefault()?.Text;
            // Устанавливаем DataContext для MainWindow, чтобы RuleOptions был доступен

        }

        public void binddatagrid(int ID)
        {
            try
            {

                // Получаем ответы по Id
                var answersFromDb = ExpertSystemEntities.GetContext().WeightedSystem_Answer
                    .Where(a => a.QuestionId == ID)
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
                var answers = Dg.ItemsSource as List<WeightedSystem_Answer>;
                foreach (var answer in answers)
                {
                    answer.QuestionId = id;

                    // Проверяем, существует ли запись в базе данных
                    var existingQuest = ExpertSystemEntities.GetContext().WeightedSystem_Answer
                        .FirstOrDefault(a => a.Id == answer.Id);

                    if (existingQuest != null)
                    {
                        // Обновляем запись

                        existingQuest.Text = answer.Text;
                        existingQuest.QuestionId = answer.QuestionId;

                    }
                    else
                    {
                        // Добавляем новую запись
                        ExpertSystemEntities.GetContext().WeightedSystem_Answer.Add(answer);
                    }
                }


                // Сохраняем изменения в базе данных
                ExpertSystemEntities.GetContext().SaveChanges();

                // Повторно привязываем обновленные данные к DataGrid
                binddatagrid(id);

                MessageBox.Show("Обновление прошло успешно");

            }
            //catch (Exception r)
            //{
            //    MessageBox.Show(r.Message);

            //}
        }




        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button)
                {
                    // Получаем текущий объект строки, к которому относится Button
                    var dataGridRow = intsis.FUNC.FindParent<DataGridRow>(button);
                    if (dataGridRow?.Item is WeightedSystem_Answer deleted)
                    {
                        // Находим объект для удаления в контексте
                        var itemToDelete = ExpertSystemEntities.GetContext().WeightedSystem_Answer
                            .FirstOrDefault(x => x.Id == deleted.Id);

                        if (itemToDelete != null)
                        {
                            // Удаляем объект
                            ExpertSystemEntities.GetContext().WeightedSystem_Answer.Remove(itemToDelete);

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
                MessageBox.Show("Сохраните изменения, прежде чем удалять.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void ChangeAns_Click(object sender, RoutedEventArgs e)
        {
           if (sender is Button button)
            {

                var dataGridRow = intsis.FUNC.FindParent<DataGridRow>(button);
                if (dataGridRow?.Item is WeightedSystem_Answer choosed)
                {

                    // Открываем окно с вопросами для выбранного правила
                    GlobalDATA.SelectANSID = choosed.Id;
                        navigateView.Navigate(typeof(WFA));
                    
                }
            }
        }
    }
}