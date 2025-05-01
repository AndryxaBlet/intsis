using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Windows.Threading;
using System.Web.UI.WebControls;
using MaterialDesignThemes.Wpf;
using intsis.Views;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace intsis
{
    /// <summary>
    /// Логика взаимодействия для Answers.xaml
    /// </summary>
    public partial class AnswersWPF : Page
    {
        Wpf.Ui.Controls.NavigationView navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
        public AnswersWPF()
        {
            int ID = GlobalDATA.IdSisForCREATE;
            InitializeComponent();log = ID;
            if (ExpertSystemV2Entities.GetContext().ExpSystems.Where(r => r.ExpSysID == ID).First().TypeID == 1)
            {
                Deny.Click -= Deny_Click;
                StartButton_Click(Deny, new RoutedEventArgs());
            }
            else
            {
                next = FIRST(ID); 

            }
        }
        int next = 0;
        int log = 0;
      
   

        public int FIRST(int id)
        {
                // Получаем текст правила по IDSis
                var ruleText = ExpertSystemV2Entities.GetContext().Questions
                    .Where(r => r.ExpSysID == id)
                    .Select(r => r.Text)
                    .FirstOrDefault();

                // Получаем Id по IDSis
                var ruleID = ExpertSystemV2Entities.GetContext().Questions
                    .Where(r => r.ExpSysID == id)
                    .Select(r => r.QuestionID)
                    .FirstOrDefault();

                // Устанавливаем текст вопроса в интерфейсе
                VOP.Text = ruleText;

                // Обновляем элементы ComboBox
                UpdateItems(ruleID);

                return ruleID;

        }
        private void UpdateItems(int id)
        {
            //try
            {
                // Получаем список ответов по Id
                var answers = ExpertSystemV2Entities.GetContext().Answers
                    .Where(a => a.QuestionID == id)
                    .ToList();

                // Присваиваем данные источнику ComboBox
                CB.ItemsSource = answers;
                CB.DisplayMemberPath = "Text";
                CB.SelectedValuePath = "AnswerID";

            }
            //catch (Exception r)
            //{
            //    var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок",Title = "Ошибка", Content = r.Message };
            //    messagebox.ShowDialogAsync();

            //}
        }

        public void BNext()
        {
            //try
            //{
                if (CB.SelectedIndex != -1)
                {
                    var selectedValue = CB.SelectedValue.ToString();
                    int.TryParse(selectedValue, out int sv);

                    if (selectedValue != null)
                    {

                        // Получаем значение поля NextR
                        var nextValue = ExpertSystemV2Entities.GetContext().Answers
                            .Where(a => a.AnswerID == sv)
                            .FirstOrDefault();


                      
                        if (nextValue.NextQuestion != null)
                        {
                            next = int.Parse(nextValue.NextQuestion.ToString());
                            // Получаем текст ответа
                            var rec = ExpertSystemV2Entities.GetContext().Answers
                                .Where(a => a.AnswerID == sv)
                                .Select(a => a.Recommendation)
                                .FirstOrDefault();

                            if (!string.IsNullOrEmpty(rec))
                            {
                                new MessageBox {Content=rec}.ShowDialogAsync();
                            }

                            // Получаем текст следующего вопроса
                            var nextText = ExpertSystemV2Entities.GetContext().Questions
                                .Where(r => r.QuestionID == next)
                                .Select(r => r.Text)
                                .FirstOrDefault();

                            VOP.Text = nextText;
                            UpdateItems(next);
                        }
                        else
                        {
                            // Если это не число, выводим строковое сообщение
                            VOP.Text = nextValue.Recommendation.ToString(); //////////////////////////////////////////////////////////
                            CB.Visibility = Visibility.Hidden;
                            Deny.Visibility = Visibility.Hidden;
                            Repeat.Visibility = Visibility.Visible;
                        }

                    }
                }
                else
                {
                    var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Предупреждение", Content = "Выберите ваприант ответа." };
                    messagebox.ShowDialogAsync();
                }
            //}
            //catch (Exception r)
            //{
            //    new MessageBox { Content = r.Message }.ShowDialogAsync();

            //}
        }
        List<string> logs = new List<string>();
        private void LogToListBox(string message)
        {
            // Добавляем сообщение в ListBox
            logs.Add(message);
        }



        public static async Task RunSisAsync(
            ExpSystems system,
            Action< string> displayQuestion,
            Func<List<string>, Task<int>> getAnswer,
            Action<string> logToListBox, Action<string> exit) // Метод для добавления логов в ListBox
        {
            int count = 0;
            if (system != null)
            {
                ExpertSystemV2Entities context = ExpertSystemV2Entities.GetContext();
                Dictionary<int, decimal> LeaderBoard = new Dictionary<int, decimal>();

                logToListBox("Запуск системы."); // Логируем запуск системы

                // Шаг 1: Начинаем с факта 1 (стартовый факт)

                var startFact = context.Facts
                                        .Where(f => f.ExpSysID == system.ExpSysID)
                                        .FirstOrDefault();
                int startFactId = startFact.FactID;

                logToListBox($"Стартовый факт: {startFact.Name}");

                var startFactQuestions = context.Questions
                    .Where(q => q.FactID == startFactId)
                    .ToList();

                foreach (var factQuestion in startFactQuestions)
                {
                    displayQuestion(factQuestion.Text);
                    logToListBox($"Задан вопрос: {factQuestion.Text}");

                    var factAnswers = factQuestion.Answers
                        .Where(r => r.QuestionID == factQuestion.QuestionID)
                        .ToList();
                    var answerOptionsForFact = factAnswers.Select(a => $"{a.AnswerID}: {a.Text}").ToList();

                    int selectedAnswerId = await getAnswer(answerOptionsForFact);

                    var selectedAnswer = context.Answers.FirstOrDefault(r => r.AnswerID == selectedAnswerId);
                    if (selectedAnswer != null)
                    {
                        logToListBox($"Выбран ответ: {selectedAnswer.Text}");
                        if (selectedAnswer.Recommendation != "") 
                        new MessageBox { Content = selectedAnswer.Recommendation, CloseButtonText="Ок" }.ShowDialogAsync();

                        var weightAnswers = selectedAnswer.WeightAnswers.Where(r => r.AnswerID == selectedAnswer.AnswerID).ToList();
                        foreach (var weight in weightAnswers)
                        {
                            int factId = weight.FactID;
                            decimal weightValue = weight.Value;
                            bool isPositive = weight.PlusOrMinus;


                            if (LeaderBoard.ContainsKey(factId))
                                LeaderBoard[factId] += isPositive ? weightValue : -weightValue;
                            else
                                LeaderBoard[factId] = isPositive ? weightValue : -weightValue;

                            var thisfact = context.Facts
                                        .Where(f => f.FactID == factId)
                                        .FirstOrDefault();

                            logToListBox($"Факт {thisfact.Name} обновлён: вес {LeaderBoard[factId]}");
                        }
                    }
                }

                logToListBox("Первоначальная таблица лидеров построена.");

                // Шаг 2: Берем лидера с максимальным весом
                var currentLeader = LeaderBoard.OrderByDescending(f => f.Value).FirstOrDefault();
                int currentFactId = currentLeader.Key;

                var fact = context.Facts
                            .Where(f => f.FactID == currentFactId)
                            .FirstOrDefault();
                logToListBox($"Текущий лидер: {fact.Name}, вес: {currentLeader.Value}");

                while (count <= 5)
                {
                    count++;

                    var factQuestionsForLeader = context.Questions
                        .Where(q => q.FactID == currentFactId)
                        .ToList();

                    List<Answers> leaderChosenAnswers = new List<Answers>();
                    HashSet<int> answeredQuestions = new HashSet<int>();

                    foreach (var leaderQuestion in factQuestionsForLeader)
                    {
                        if (answeredQuestions.Contains(leaderQuestion.QuestionID)) continue;

                        displayQuestion(leaderQuestion.Text);
                        logToListBox($"Задан вопрос: {leaderQuestion.Text}");

                        var leaderAnswers = leaderQuestion.Answers
                            .Where(r => r.QuestionID == leaderQuestion.QuestionID)
                            .ToList();
                        var leaderAnswerOptions = leaderAnswers.Select(a => $"{a.AnswerID}: {a.Text}").ToList();

                        int leaderChosenAnswerId = await getAnswer(leaderAnswerOptions);

                        var leaderChosenAnswer = context.Answers
                            .FirstOrDefault(r => r.AnswerID == leaderChosenAnswerId);
                        if (leaderChosenAnswer != null)
                        {
                            leaderChosenAnswers.Add(leaderChosenAnswer);
                            logToListBox($"Выбран ответ: {leaderChosenAnswer.Text}");
                            new MessageBox { Content = leaderChosenAnswer.Recommendation, CloseButtonText = "Ок" }.ShowDialogAsync();

                            var weightAnswersForLeader = leaderChosenAnswer.WeightAnswers
                                .Where(r => r.AnswerID == leaderChosenAnswer.AnswerID)
                                .ToList();
                            foreach (var weight in weightAnswersForLeader)
                            {
                                int factId = weight.FactID;
                                decimal weightValue = weight.Value;
                                bool isPositive = weight.PlusOrMinus;

                                if (LeaderBoard.ContainsKey(factId))
                                    LeaderBoard[factId] += isPositive ? weightValue : -weightValue;
                                else
                                    LeaderBoard[factId] = isPositive ? weightValue : -weightValue;

                                var thisfact = context.Facts
                                       .Where(f => f.FactID == factId)
                                       .FirstOrDefault();

                                logToListBox($"Факт {thisfact.Name} обновлён: вес {LeaderBoard[factId]}");
                            }
                        }

                        answeredQuestions.Add(leaderQuestion.QuestionID);

                        var updatedLeader = LeaderBoard.OrderByDescending(f => f.Value).FirstOrDefault();
                        if (updatedLeader.Key != currentFactId)
                        {
                            currentFactId = updatedLeader.Key;
                            logToListBox($"Изменился лидер: {currentFactId}, вес: {updatedLeader.Value}");
                            break;
                        }

                        currentLeader = LeaderBoard.OrderByDescending(f => f.Value).FirstOrDefault();
                        if (currentLeader.Value >= 0.8m) { break; }
                    }

                    if (currentLeader.Value >= 0.8m)
                    {
                        var thisfact = context.Facts
                                       .Where(f => f.FactID == currentLeader.Key)
                                       .FirstOrDefault();

                        logToListBox($"Факт достиг порога 0.8: ID = {thisfact.Name}, Вес = {currentLeader.Value}");
                        displayQuestion($"Выбранный факт: {thisfact.Name}");
                        exit("");
                        // Если это не число, выводим строковое сообщение
                        break;
                    }

                    if (count == 5)
                    {
                        
                        logToListBox($"Достигнуто максимальное количество попыток. Проверяем лидера...");
                        if (currentLeader.Value > 0.5m)

                        {var thisfact = context.Facts
                                       .Where(f => f.FactID == currentLeader.Key)
                                       .FirstOrDefault();
                            logToListBox($"Факт с весом > 0.5: ID = {thisfact.Name}, Вес = {currentLeader.Value}");
                            displayQuestion($"Выбранный факт: {thisfact.Name}");
                            exit("");
                        }
                        else
                        {
                            logToListBox($"Лидер с весом <= 0.5. Перезапуск системы...");
                            await RunSisAsync(system, displayQuestion, getAnswer, logToListBox,exit);
                        }
                        break;
                    }
                }
            }
        }
      

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            ExpertSystemV2Entities context = ExpertSystemV2Entities.GetContext();
            var sys = context.ExpSystems.Where(x => x.ExpSysID == log).First();
            // Запускаем алгоритм системы
            await RunSisAsync(
                sys,
                DisplayQuestion,    // Передаём метод для отображения вопроса
                GetAnswerFromUI,
                logToListBox: LogToListBox,
                exit:Exit// Передаём метод для логов     // Передаём метод для получения ответа
            );
        }

        // Метод для отображения вопроса (реализация делегата displayQuestion)
        private void DisplayQuestion(string question)
        {
           
            // Используем Dispatcher для обновления UI в потоке UI
            Dispatcher.Invoke(() =>
            {
                VOP.Text = question; // Показываем текст вопроса на UI
               
            });
        }
    private void Exit(string s)
    {
        // Используем Dispatcher для обновления UI в потоке UI
        Dispatcher.Invoke(() =>
        {
            CB.Visibility = Visibility.Hidden;
            Deny.Visibility = Visibility.Hidden;
            Repeat.Visibility = Visibility.Visible;
            Log window = new Log(logs);
            window.Show();
        });
    }

    // Метод для получения ответа (реализация делегата getAnswer)
    private async Task<int> GetAnswerFromUI(List<string> options)
        {
            // Заполняем список ответов
            Dispatcher.Invoke(() =>
            {
                CB.ItemsSource = options;
            });

            // Ждём выбора ответа пользователем
            var tcs = new TaskCompletionSource<int>();

            // Обработчик кнопки выбора ответа
            RoutedEventHandler handler = null;
            handler = (s, e) =>
            {
                if (CB.SelectedItem != null)
                {
                    var selectedOption = CB.SelectedItem.ToString();
                    int id = int.Parse(selectedOption.Split(':')[0]); // Парсим ID ответа
                    tcs.SetResult(id); // Возвращаем выбранный ID
                }
                else
                {
                    var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Предупреждение", Content = "Выберите ваприант ответа." };
                    messagebox.ShowDialogAsync();
                }
            };

            // Подписываемся на событие только один раз
            Deny.Click += handler;

            // Ждем, пока пользователь выберет ответ
            int result = await tcs.Task;

            // Убираем обработчик после завершения
            Deny.Click -= handler;

            return result; // Возвращаем результат, когда пользователь выберет ответ
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            navigateView.GoBack();
        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            navigateView.Navigate(typeof(MainWindow));
            navigateView.Navigate(typeof(AnswersWPF));
        }
        private async void Deny_Click(object sender, RoutedEventArgs e)
        {
            //try
            {
                BNext();
            }
            //catch (Exception r)
            //{
            //    var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Ошибка", Content = r.Message};
            //    messagebox.ShowDialogAsync();
            //}
        }
    }
}
