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
    public partial class Answers : Page
    {
        Wpf.Ui.Controls.NavigationView navigateView = Application.Current.MainWindow.FindName("MainNavigation") as Wpf.Ui.Controls.NavigationView;
        public Answers()
        {
            int ID = GlobalDATA.IdSisForCREATE;
            InitializeComponent();log = ID;
            if (ExpertSystemEntities.GetContext().ExpSystem.Where(r => r.Id == ID).First().Type == true)
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
                var ruleText = ExpertSystemEntities.GetContext().LinearSystem_Question
                    .Where(r => r.SystemId == id)
                    .Select(r => r.Text)
                    .FirstOrDefault();

                // Получаем Id по IDSis
                var ruleID = ExpertSystemEntities.GetContext().LinearSystem_Question
                    .Where(r => r.SystemId == id)
                    .Select(r => r.Id)
                    .FirstOrDefault();

                // Устанавливаем текст вопроса в интерфейсе
                VOP.Text = ruleText;

                // Обновляем элементы ComboBox
                UpdateItems(ruleID);

                return ruleID;

        }
        private void UpdateItems(int id)
        {
            try
            {
                // Получаем список ответов по Id
                var answers = ExpertSystemEntities.GetContext().LinearSystem_Answer
                    .Where(a => a.QuestionId == id)
                    .ToList();

                // Присваиваем данные источнику ComboBox
                CB.ItemsSource = answers;
                CB.DisplayMemberPath = "Text";
                CB.SelectedValuePath = "Id";

            }
            catch (Exception r)
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок",Title = "Ошибка", Content = r.Message };
                messagebox.ShowDialogAsync();

            }
        }

        public void BNext()
        {
            try
            {
                if (CB.SelectedIndex != -1)
                {
                    var selectedValue = CB.SelectedValue.ToString();
                    int.TryParse(selectedValue, out int sv);

                    if (selectedValue != null)
                    {

                        // Получаем значение поля NextR
                        var nextValue = ExpertSystemEntities.GetContext().LinearSystem_Answer
                            .Where(a => a.Id == sv)
                            .FirstOrDefault();


                      
                        if (nextValue.Out == "" || nextValue.Out == null)
                        {
                            next = int.Parse(nextValue.NextQuestionId.ToString());
                            // Получаем текст ответа
                            var rec = ExpertSystemEntities.GetContext().LinearSystem_Answer
                                .Where(a => a.Id == sv)
                                .Select(a => a.Recomendation)
                                .FirstOrDefault();

                            if (!string.IsNullOrEmpty(rec))
                            {
                                new MessageBox {Content=rec}.ShowDialogAsync();
                            }

                            // Получаем текст следующего вопроса
                            var nextText = ExpertSystemEntities.GetContext().LinearSystem_Question
                                .Where(r => r.Id == next)
                                .Select(r => r.Text)
                                .FirstOrDefault();

                            VOP.Text = nextText;
                            UpdateItems(next);
                        }
                        else
                        {
                            // Если это не число, выводим строковое сообщение
                            VOP.Text = nextValue.Out.ToString();
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
            }
            catch (Exception r)
            {
                new MessageBox { Content = r.Message }.ShowDialogAsync();

            }
        }
        List<string> logs = new List<string>();
        private void LogToListBox(string message)
        {
            // Добавляем сообщение в ListBox
            logs.Add(message);
        }



        public static async Task RunSisAsync(
            ExpSystem system,
            Action< string> displayQuestion,
            Func<List<string>, Task<int>> getAnswer,
            Action<string> logToListBox, Action<string> exit) // Метод для добавления логов в ListBox
        {
            int count = 0;
            if (system != null)
            {
                ExpertSystemEntities context = ExpertSystemEntities.GetContext();
                Dictionary<int, decimal> LeaderBoard = new Dictionary<int, decimal>();

                logToListBox("Запуск системы."); // Логируем запуск системы

                // Шаг 1: Начинаем с факта 1 (стартовый факт)

                var startFact = context.WeightedSystem_Fact
                                        .Where(f => f.SystemId == system.Id)
                                        .FirstOrDefault();
                int startFactId = startFact.Id;

                logToListBox($"Стартовый факт: {startFact.Name}");

                var startFactQuestions = context.WeightedSystem_Question
                    .Where(q => q.FactID == startFactId)
                    .ToList();

                foreach (var factQuestion in startFactQuestions)
                {
                    displayQuestion(factQuestion.Text);
                    logToListBox($"Задан вопрос: {factQuestion.Text}");

                    var factAnswers = factQuestion.WeightedSystem_Answer
                        .Where(r => r.QuestionId == factQuestion.Id)
                        .ToList();
                    var answerOptionsForFact = factAnswers.Select(a => $"{a.Id}: {a.Text}").ToList();

                    int selectedAnswerId = await getAnswer(answerOptionsForFact);

                    var selectedAnswer = context.WeightedSystem_Answer.FirstOrDefault(r => r.Id == selectedAnswerId);
                    if (selectedAnswer != null)
                    {
                        logToListBox($"Выбран ответ: {selectedAnswer.Text}");
                        new MessageBox { Content = selectedAnswer.Recomendation, CloseButtonText="Ок" }.ShowDialogAsync();

                        var weightAnswers = selectedAnswer.WeightFactAnswer.Where(r => r.IdAnswer == selectedAnswer.Id).ToList();
                        foreach (var weight in weightAnswers)
                        {
                            int factId = weight.IdFact;
                            decimal weightValue = weight.Weight;
                            bool isPositive = weight.PlusOrMinus;


                            if (LeaderBoard.ContainsKey(factId))
                                LeaderBoard[factId] += isPositive ? weightValue : -weightValue;
                            else
                                LeaderBoard[factId] = isPositive ? weightValue : -weightValue;

                            var thisfact = context.WeightedSystem_Fact
                                        .Where(f => f.Id == factId)
                                        .FirstOrDefault();

                            logToListBox($"Факт {thisfact.Name} обновлён: вес {LeaderBoard[factId]}");
                        }
                    }
                }

                logToListBox("Первоначальная таблица лидеров построена.");

                // Шаг 2: Берем лидера с максимальным весом
                var currentLeader = LeaderBoard.OrderByDescending(f => f.Value).FirstOrDefault();
                int currentFactId = currentLeader.Key;

                var fact = context.WeightedSystem_Fact
                            .Where(f => f.Id == currentFactId)
                            .FirstOrDefault();
                logToListBox($"Текущий лидер: {fact.Name}, вес: {currentLeader.Value}");

                while (count <= 5)
                {
                    count++;

                    var factQuestionsForLeader = context.WeightedSystem_Question
                        .Where(q => q.FactID == currentFactId)
                        .ToList();

                    List<WeightedSystem_Answer> leaderChosenAnswers = new List<WeightedSystem_Answer>();
                    HashSet<int> answeredQuestions = new HashSet<int>();

                    foreach (var leaderQuestion in factQuestionsForLeader)
                    {
                        if (answeredQuestions.Contains(leaderQuestion.Id)) continue;

                        displayQuestion(leaderQuestion.Text);
                        logToListBox($"Задан вопрос: {leaderQuestion.Text}");

                        var leaderAnswers = leaderQuestion.WeightedSystem_Answer
                            .Where(r => r.QuestionId == leaderQuestion.Id)
                            .ToList();
                        var leaderAnswerOptions = leaderAnswers.Select(a => $"{a.Id}: {a.Text}").ToList();

                        int leaderChosenAnswerId = await getAnswer(leaderAnswerOptions);

                        var leaderChosenAnswer = context.WeightedSystem_Answer
                            .FirstOrDefault(r => r.Id == leaderChosenAnswerId);
                        if (leaderChosenAnswer != null)
                        {
                            leaderChosenAnswers.Add(leaderChosenAnswer);
                            logToListBox($"Выбран ответ: {leaderChosenAnswer.Text}");
                            new MessageBox { Content = leaderChosenAnswer.Recomendation, CloseButtonText = "Ок" }.ShowDialogAsync();

                            var weightAnswersForLeader = leaderChosenAnswer.WeightFactAnswer
                                .Where(r => r.IdAnswer == leaderChosenAnswer.Id)
                                .ToList();
                            foreach (var weight in weightAnswersForLeader)
                            {
                                int factId = weight.IdFact;
                                decimal weightValue = weight.Weight;
                                bool isPositive = weight.PlusOrMinus;

                                if (LeaderBoard.ContainsKey(factId))
                                    LeaderBoard[factId] += isPositive ? weightValue : -weightValue;
                                else
                                    LeaderBoard[factId] = isPositive ? weightValue : -weightValue;

                                var thisfact = context.WeightedSystem_Fact
                                       .Where(f => f.Id == factId)
                                       .FirstOrDefault();

                                logToListBox($"Факт {thisfact.Name} обновлён: вес {LeaderBoard[factId]}");
                            }
                        }

                        answeredQuestions.Add(leaderQuestion.Id);

                        var updatedLeader = LeaderBoard.OrderByDescending(f => f.Value).FirstOrDefault();
                        if (updatedLeader.Key != currentFactId)
                        {
                            currentFactId = updatedLeader.Key;
                            logToListBox($"Изменился лидер: {currentFactId}, вес: {updatedLeader.Value}");
                            break;
                        }

                        currentLeader = LeaderBoard.OrderByDescending(f => f.Value).FirstOrDefault();
                    }

                    if (currentLeader.Value >= 0.8m)
                    {
                        var thisfact = context.WeightedSystem_Fact
                                       .Where(f => f.Id == currentLeader.Key)
                                       .FirstOrDefault();

                        logToListBox($"Факт достиг порога 0.8: ID = {thisfact.Name}, Вес = {currentLeader.Value}");
                        displayQuestion($"Выбранный факт: ID = {thisfact.Name}, Вес = {currentLeader.Value}");
                        exit("");
                        // Если это не число, выводим строковое сообщение
                        break;
                    }

                    if (count == 5)
                    {
                        
                        logToListBox($"Достигнуто максимальное количество попыток. Проверяем лидера...");
                        if (currentLeader.Value > 0.5m)

                        {var thisfact = context.WeightedSystem_Fact
                                       .Where(f => f.Id == currentLeader.Key)
                                       .FirstOrDefault();
                            logToListBox($"Факт с весом > 0.5: ID = {thisfact.Name}, Вес = {currentLeader.Value}");
                            displayQuestion($"Выбранный факт: ID = {thisfact.Name}, Вес = {currentLeader.Value}");
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
            ExpertSystemEntities context = ExpertSystemEntities.GetContext();
            var sys = context.ExpSystem.Where(x => x.Id == log).First();
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
            navigateView.Navigate(typeof(Answers));
        }
        private async void Deny_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BNext();
            }
            catch (Exception r)
            {
                var messagebox =new Wpf.Ui.Controls.MessageBox { CloseButtonText="Ок", Title = "Ошибка", Content = r.Message};
                messagebox.ShowDialogAsync();
            }
        }
    }
}
