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
            try
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
                            VOP.Text = nextValue.Recommendation.ToString(); 
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
     ExpSystems system,
     Action<string> displayQuestion,
     Func<List<string>, Task<int>> getAnswer,
     Action<string> logToListBox,
     Action<string> exit)
        {
            if (system == null)
            {
                logToListBox("Система не найдена.");
                return;
            }

            var context = ExpertSystemV2Entities.GetContext();
            var leaderBoard = new Dictionary<int, decimal>();
            var answeredQuestions = new HashSet<int>();
            const decimal THRESHOLD = 0.8m;
            const decimal MIN_THRESHOLD = 0.5m;
            const int MAX_ITERATIONS = 5;
            int iterationCount = 0;

            logToListBox("Запуск экспертной системы.");

            // Получаем стартовый факт
            var startFact = context.Facts.FirstOrDefault(f => f.ExpSysID == system.ExpSysID);
            if (startFact == null)
            {
                logToListBox("Стартовый факт не найден. Невозможно продолжить работу.");
                return;
            }

            logToListBox($"Стартовый факт системы: {startFact.Name} [ID: {startFact.FactID}]");

            // ЭТАП 1: Обработка всех вопросов стартового факта для построения первичной таблицы лидеров
            logToListBox("=========== ЭТАП 1: Построение начальной таблицы лидеров ===========");

            var startFactQuestions = context.Questions
                .Where(q => q.FactID == startFact.FactID)
                .ToList();

            if (startFactQuestions.Count == 0)
            {
                logToListBox("Предупреждение: У стартового факта нет вопросов. Невозможно построить таблицу лидеров.");
                return;
            }

            logToListBox($"Найдено {startFactQuestions.Count} вопросов для стартового факта.");

            // Обрабатываем все вопросы стартового факта
            foreach (var question in startFactQuestions)
            {
                displayQuestion(question.Text);
                logToListBox($"Задан вопрос: \"{question.Text}\" [ID: {question.QuestionID}]");

                var answers = question.Answers.ToList();
                if (answers.Count == 0)
                {
                    logToListBox($"Предупреждение: Вопрос ID: {question.QuestionID} не имеет вариантов ответа. Пропускаем.");
                    continue;
                }

                var options = answers.Select(a => $"{a.AnswerID}: {a.Text}").ToList();

                logToListBox($"Доступно вариантов ответа: {answers.Count}");

                int answerId = await getAnswer(options);
                var selectedAnswer = context.Answers.FirstOrDefault(r => r.AnswerID == answerId);
                if (selectedAnswer == null)
                {
                    logToListBox($"Ошибка: Выбранный ответ ID: {answerId} не найден. Пропускаем вопрос.");
                    continue;
                }

                logToListBox($"Выбран ответ: \"{selectedAnswer.Text}\" [ID: {selectedAnswer.AnswerID}]");

                // Отображаем рекомендацию, если она есть
                if (!string.IsNullOrEmpty(selectedAnswer.Recommendation))
                {
                    logToListBox($"Рекомендация: \"{selectedAnswer.Recommendation}\"");
                    await new MessageBox { Content = selectedAnswer.Recommendation, CloseButtonText = "Ок" }.ShowDialogAsync();
                }

                // Учитываем влияние ответа на веса фактов
                var weightCount = selectedAnswer.WeightAnswers?.Count() ?? 0;
                logToListBox($"Ответ влияет на {weightCount} факт(ов)");

                foreach (var weight in selectedAnswer.WeightAnswers)
                {
                    int factId = weight.FactID;
                    decimal weightValue = weight.PlusOrMinus ? weight.Value : -weight.Value;
                    string sign = weight.PlusOrMinus ? "+" : "-";

                    var beforeValue = leaderBoard.TryGetValue(factId, out var existing) ? existing : 0;
                    leaderBoard[factId] = beforeValue + weightValue;

                    var affectedFact = context.Facts.FirstOrDefault(f => f.FactID == factId);
                    if (affectedFact != null)
                        logToListBox($"Факт \"{affectedFact.Name}\" [ID: {factId}] обновлён: {beforeValue} {sign} {Math.Abs(weightValue)} = {leaderBoard[factId]}");
                }

                // Помечаем вопрос как отвеченный
                answeredQuestions.Add(question.QuestionID);
            }

            // Анализируем результаты после обработки всех вопросов стартового факта
            logToListBox("Первоначальная таблица лидеров построена.");

            if (leaderBoard.Count == 0)
            {
                logToListBox("Предупреждение: Таблица лидеров пуста. Ответы не повлияли ни на один факт.");
                return;
            }

            logToListBox("Состояние таблицы лидеров:");
            foreach (var entry in leaderBoard.OrderByDescending(kv => kv.Value))
            {
                var fact = context.Facts.FirstOrDefault(f => f.FactID == entry.Key);
                logToListBox($"- \"{fact?.Name}\" [ID: {entry.Key}]: {entry.Value}");
            }

            // Определяем текущего лидера
            var currentLeader = leaderBoard.OrderByDescending(f => f.Value).FirstOrDefault();
            int currentFactId = currentLeader.Key;
            var leaderFact = context.Facts.FirstOrDefault(f => f.FactID == currentFactId);

            logToListBox($"Текущий лидер: \"{leaderFact?.Name}\" [ID: {currentFactId}], вес: {currentLeader.Value}");

            // Проверяем, достиг ли лидер порогового значения
            if (currentLeader.Value >= THRESHOLD)
            {
                logToListBox($"Факт \"{leaderFact?.Name}\" достиг порога {THRESHOLD}: Вес = {currentLeader.Value}");
                displayQuestion($"Выбранный факт: {leaderFact?.Name}");
                exit("");
                return;
            }

            // ЭТАП 2: Задаем дополнительные вопросы по фактам-лидерам
            logToListBox("=========== ЭТАП 2: Задание дополнительных вопросов по фактам-лидерам ===========");

            // Основной цикл алгоритма для уточнения лидеров
            while (iterationCount < MAX_ITERATIONS)
            {
                iterationCount++;
                logToListBox($"Итерация #{iterationCount} из {MAX_ITERATIONS}");

                // Получаем вопросы для текущего факта-лидера, которые еще не были заданы
                var factQuestions = context.Questions
                    .Where(q => q.FactID == currentFactId && !answeredQuestions.Contains(q.QuestionID))
                    .ToList();

                if (factQuestions.Count == 0)
                {
                    logToListBox($"Нет доступных вопросов для факта \"{leaderFact?.Name}\" [ID: {currentFactId}]");

                    // Если нет доступных вопросов, но есть лидер с весом >= THRESHOLD
                    if (currentLeader.Value >= THRESHOLD)
                    {
                        logToListBox($"Факт-лидер достиг порога {THRESHOLD}. Завершаем работу.");
                        break;
                    }

                    // Находим следующий факт с максимальным весом, для которого есть незаданные вопросы
                    var nextFactCandidate = leaderBoard.OrderByDescending(f => f.Value)
                        .FirstOrDefault(f => f.Key != currentFactId &&
                                 context.Questions.Any(q => q.FactID == f.Key && !answeredQuestions.Contains(q.QuestionID)));

                    if (nextFactCandidate.Key == 0)
                    {
                        logToListBox("Нет доступных вопросов для других фактов. Завершаем процесс уточнения.");
                        break;
                    }

                    currentFactId = nextFactCandidate.Key;
                    leaderFact = context.Facts.FirstOrDefault(f => f.FactID == currentFactId);
                    logToListBox($"Переход к следующему факту с наибольшим весом: \"{leaderFact?.Name}\" [ID: {currentFactId}], вес: {nextFactCandidate.Value}");
                    continue;
                }

                logToListBox($"Найдено {factQuestions.Count} незаданных вопросов для факта \"{leaderFact?.Name}\"");

                // Процесс задания вопросов и обработки ответов
                foreach (var question in factQuestions)
                {
                    displayQuestion(question.Text);
                    logToListBox($"Задан вопрос: \"{question.Text}\" [ID: {question.QuestionID}]");

                    var answers = question.Answers.ToList();
                    var options = answers.Select(a => $"{a.AnswerID}: {a.Text}").ToList();

                    logToListBox($"Доступно вариантов ответа: {answers.Count}");

                    int answerId = await getAnswer(options);
                    var selectedAnswer = context.Answers.FirstOrDefault(r => r.AnswerID == answerId);
                    if (selectedAnswer == null)
                    {
                        logToListBox($"Ошибка: Выбранный ответ ID: {answerId} не найден. Пропускаем вопрос.");
                        continue;
                    }

                    logToListBox($"Выбран ответ: \"{selectedAnswer.Text}\" [ID: {selectedAnswer.AnswerID}]");

                    // Отображаем рекомендацию, если она есть
                    if (!string.IsNullOrEmpty(selectedAnswer.Recommendation))
                    {
                        logToListBox($"Рекомендация: \"{selectedAnswer.Recommendation}\"");
                        await new MessageBox { Content = selectedAnswer.Recommendation, CloseButtonText = "Ок" }.ShowDialogAsync();
                    }

                    // Учитываем влияние ответа на веса фактов
                    var weightCount = selectedAnswer.WeightAnswers?.Count() ?? 0;
                    logToListBox($"Ответ влияет на {weightCount} факт(ов)");

                    foreach (var weight in selectedAnswer.WeightAnswers)
                    {
                        int factId = weight.FactID;
                        decimal weightValue = weight.PlusOrMinus ? weight.Value : -weight.Value;
                        string sign = weight.PlusOrMinus ? "+" : "-";

                        var beforeValue = leaderBoard.TryGetValue(factId, out var existing) ? existing : 0;
                        leaderBoard[factId] = beforeValue + weightValue;

                        var affectedFact = context.Facts.FirstOrDefault(f => f.FactID == factId);
                        if (affectedFact != null)
                            logToListBox($"Факт \"{affectedFact.Name}\" [ID: {factId}] обновлён: {beforeValue} {sign} {Math.Abs(weightValue)} = {leaderBoard[factId]}");
                    }

                    // Помечаем вопрос как отвеченный
                    answeredQuestions.Add(question.QuestionID);

                    // Проверяем, достиг ли какой-либо факт порогового значения
                    currentLeader = leaderBoard.OrderByDescending(f => f.Value).FirstOrDefault();

                    if (currentLeader.Value >= THRESHOLD)
                    {
                        var winningFact = context.Facts.FirstOrDefault(f => f.FactID == currentLeader.Key);
                        logToListBox($"Факт \"{winningFact?.Name}\" достиг порога {THRESHOLD}: Вес = {currentLeader.Value}");
                        displayQuestion($"Выбранный факт: {winningFact?.Name}");
                        exit("");
                        return;
                    }

                    // Если изменился лидер, переходим к вопросам по новому факту
                    if (currentLeader.Key != currentFactId && currentLeader.Value > 0)
                    {
                        currentFactId = currentLeader.Key;
                        var newLeaderFact = context.Facts.FirstOrDefault(f => f.FactID == currentFactId);
                        logToListBox($"Изменился лидер: \"{newLeaderFact?.Name}\" [ID: {currentFactId}], вес: {currentLeader.Value}");
                        break; // выйти и пересобрать список вопросов
                    }
                }
            }

            // Проверяем результаты после завершения итераций
            logToListBox("=========== РЕЗУЛЬТАТЫ РАБОТЫ СИСТЕМЫ ===========");
            logToListBox($"Достигнуто максимальное количество итераций ({MAX_ITERATIONS}) или исчерпаны вопросы.");

            // Вывод окончательной таблицы лидеров
            logToListBox("Финальная таблица лидеров фактов:");
            foreach (var entry in leaderBoard.OrderByDescending(kv => kv.Value))
            {
                var fact = context.Facts.FirstOrDefault(f => f.FactID == entry.Key);
                logToListBox($"- \"{fact?.Name}\" [ID: {entry.Key}]: {entry.Value}");
            }

            var finalLeader = leaderBoard.OrderByDescending(f => f.Value).FirstOrDefault();
            var finalFact = context.Facts.FirstOrDefault(f => f.FactID == finalLeader.Key);

            logToListBox($"Итоговый лидер: \"{finalFact?.Name}\" [ID: {finalLeader.Key}], вес: {finalLeader.Value}");

            if (finalLeader.Value >= MIN_THRESHOLD)
            {
                logToListBox($"Вес факта превышает минимальный порог {MIN_THRESHOLD}. Система успешно определила факт.");
                displayQuestion($"Выбранный факт: {finalFact?.Name}");
                exit("");
            }
            else
            {
                logToListBox($"Недостаточный вес для определения факта ({finalLeader.Value} < {MIN_THRESHOLD}).");
                logToListBox($"Требуется перезапуск системы с новыми вопросами...");

                // Перезапуск системы
                await RunSisAsync(system, displayQuestion, getAnswer, logToListBox, exit);
            }
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            ExpertSystemV2Entities context = ExpertSystemV2Entities.GetContext();
            var sys = context.ExpSystems.Where(x => x.ExpSysID == log).First();

            // Запускаем алгоритм системы
            await RunSisAsync(
                sys,
                DisplayQuestion,
                GetAnswerFromUI,
                LogToListBox,
                Exit
            );
        }

        // Метод для отображения вопроса
        private void DisplayQuestion(string question)
        {
            Dispatcher.Invoke(() =>
            {
                VOP.Text = question;
            });
        }

        // Метод для завершения работы
        private void Exit(string s)
        {
            Dispatcher.Invoke(() =>
            {
                CB.Visibility = Visibility.Hidden;
                Deny.Visibility = Visibility.Hidden;
                Repeat.Visibility = Visibility.Visible;
                Log window = new Log(logs);
                window.Show();
            });
        }

        // Метод для получения ответа
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
                    var messagebox = new Wpf.Ui.Controls.MessageBox
                    {
                        CloseButtonText = "Ок",
                        Title = "Предупреждение",
                        Content = "Выберите вариант ответа."
                    };
                    messagebox.ShowDialogAsync();
                }
            };

            // Подписываемся на событие
            Deny.Click += handler;

            // Ждем, пока пользователь выберет ответ
            int result = await tcs.Task;

            // Убираем обработчик после завершения
            Deny.Click -= handler;

            return result;
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
