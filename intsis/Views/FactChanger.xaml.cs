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
using System.IdentityModel.Metadata;

namespace intsis
{
    /// <summary>
    /// Логика взаимодействия для ANS.xaml
    /// </summary>
    public partial class FactChanger : Page
    {
        private int id = 0;
        int idsis=0;
        Wpf.Ui.Controls.NavigationView navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
        public FactChanger()
        {
            InitializeComponent();
            int ID = GlobalDATA.SelectFACTID;
            idsis = GlobalDATA.IdSisForCREATE;
            binddatagrid(ID);
            id = Convert.ToInt32(ID);
            RuleTextBlock.Text = "Настройка факта: " + ExpertSystemV2Entities.GetContext().Facts.Where(x => x.FactID == ID).FirstOrDefault()?.Description;
            // Устанавливаем DataContext для MainWindow, чтобы RuleOptions был доступен

        }

        public void binddatagrid(int ID)
        {
            //try
            //{

                // Получаем ответы по Id
                var answersFromDb = ExpertSystemV2Entities.GetContext().Questions
                    .Where(a => a.FactID == ID)
                    .ToList();
                Dg.ItemsSource = answersFromDb; // Привязываем коллекцию к DataGrid
            //}
            //catch (Exception r)
            //{
            //    var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Ошибка", Content = r.Message};
            //    messagebox.ShowDialogAsync();
            //}

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                // Получаем измененные записи из DataGrid
                var answers = Dg.ItemsSource as List<Questions>;
                foreach (var answer in answers)
                {
                    answer.FactID = id;
                    answer.ExpSysID = idsis;

                    // Проверяем, существует ли запись в базе данных
                    var existingQuest = ExpertSystemV2Entities.GetContext().Questions
                        .FirstOrDefault(a => a.QuestionID == answer.QuestionID);

                     if (existingQuest != null)
                    {
                        // Обновляем запись

                        existingQuest.Text = answer.Text;
                        existingQuest.FactID = answer.FactID;

                    }
                    else
                    {
                        // Добавляем новую запись
                        ExpertSystemV2Entities.GetContext().Questions.Add(answer);
                    }
                }


                // Сохраняем изменения в базе данных
                ExpertSystemV2Entities.GetContext().SaveChanges();

                // Повторно привязываем обновленные данные к DataGrid
                binddatagrid(id);

                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок",Content = "Обновление прошло успешно." };
                messagebox.ShowDialogAsync();

            //}
            //catch (Exception r)
            //{
            //    var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Ошибка", Content = r.Message };
            //    messagebox.ShowDialogAsync();

            //}
        }

 


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                if (sender is Button button)
                {
                    // Получаем текущий объект строки, к которому относится Button
                    var dataGridRow = intsis.FUNC.FindParent<DataGridRow>(button);
                    if (dataGridRow?.Item is Questions deleted)
                    {
                        // Находим объект для удаления в контексте
                        var itemToDelete = ExpertSystemV2Entities.GetContext().Questions
                            .FirstOrDefault(x => x.QuestionID == deleted.QuestionID);

                        if (itemToDelete != null)
                        {
                            // Удаляем объект
                            ExpertSystemV2Entities.GetContext().Questions.Remove(itemToDelete);

                            // Сохраняем изменения в базе данных
                            ExpertSystemV2Entities.GetContext().SaveChanges();

                            // Обновляем DataGrid
                            binddatagrid(id); // Используйте корректный идентификатор системы
                        }
                    }
                }
            //}
            //catch (NullReferenceException)
            //{
            //    var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Предупреждение", Content = "Сохраните изменения, прежде чем удалять." };
            //    messagebox.ShowDialogAsync();
            //}

            //catch (Exception ex)
            //{
            //    var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Ошибка", Content = ex.Message };
            //    messagebox.ShowDialogAsync();
            //}

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
            //try        
            //{
                if (sender is Button button)
                {
                

                    var dataGridRow = intsis.FUNC.FindParent<DataGridRow>(button);
                    if (dataGridRow?.Item is Questions choosed)
                    {
                        var selected = ExpertSystemV2Entities.GetContext().Questions.FirstOrDefault(x => x.QuestionID == choosed.QuestionID);

                        // Открываем окно с вопросами для выбранного правила
                        GlobalDATA.SelectRULEID = selected.QuestionID;
                        navigateView.Navigate(typeof(WeightAnsChanger));
                    
                    }
                
                }
            //}

            //catch (System.NullReferenceException)
            //{
            //    var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Предупреждение", Content = "Сохраните данные." };
            //    messagebox.ShowDialogAsync();
                
            //}

            //catch (Exception r)
            //{
            //    var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Ошибка", Content = r.Message };
            //    messagebox.ShowDialogAsync();
            //}

        }
    }
}