using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using intsis.Views;
namespace intsis
{

    public class SqlJSON
    {
       
        
        public void ExportData(int systemId, string filePath)
        {
            try
            {
                var context = ExpertSystemV2Entities.GetContext();

                var system = context.ExpSystems.FirstOrDefault(s => s.ExpSysID == systemId);
                if (system == null)
                {
                    throw new Exception("Система с указанным ID не найдена.");
                }

                object systemToExport;

                if (system.TypeID == 1) // весовой
                {
                    var facts = context.Facts
                        .Where(f => f.ExpSysID == systemId)
                        .Select(f => new
                        {
                            f.FactID,
                            f.Name,
                            f.Description,
                            Questions = context.Questions
                                .Where(q => q.FactID == f.FactID)
                                .Select(q => new
                                {
                                    q.QuestionID,
                                    q.Text,
                                    Answers = context.Answers
                                        .Where(a => a.QuestionID == q.QuestionID)
                                        .Select(a => new
                                        {
                                            a.AnswerID,
                                            a.Text,
                                            a.Recommendation,
                                            WeightAnswers = context.WeightAnswers
                                                .Where(w => w.AnswerID == a.AnswerID)
                                                .Select(w => new
                                                {
                                                    w.WAID,
                                                    w.FactID,
                                                    w.Value,
                                                    w.PlusOrMinus
                                                })
                                        })
                                })
                        })
                        .ToList();

                    systemToExport = new[]
                    {
                new
                {
                    system.ExpSysID,
                    system.NameSys,
                    system.ScopeOfApplication,
                    system.Description,
                    system.TypeID,
                    Facts = facts
                }
            };
                }
                else // линейный
                {
                    var questions = context.Questions
                        .Where(q => q.ExpSysID == systemId)
                        .Select(q => new
                        {
                            q.QuestionID,
                            q.Text,
                            Answers = context.Answers
                                .Where(a => a.QuestionID == q.QuestionID)
                                .Select(a => new
                                {
                                    a.AnswerID,
                                    a.Text,
                                    a.Recommendation,
                                    a.NextQuestion
                                })
                        })
                        .ToList();

                    systemToExport = new[]
                    {
                new
                {
                    system.ExpSysID,
                    system.NameSys,
                    system.ScopeOfApplication,
                    system.Description,
                    system.TypeID,
                    Questions = questions
                }
            };
                }

                var jsonData = JsonConvert.SerializeObject(systemToExport, Formatting.Indented);
                if (filePath != "wise-choice")
                {
                    File.WriteAllText(filePath, jsonData);

                    var messagebox = new Wpf.Ui.Controls.MessageBox { CloseButtonText = "Ок", Title = "Экспорт", Content = "Система успешно экспортирована" }.ShowDialogAsync();
                }
                else
                {
                   
                    LogIn.ExpSysForEx = systemToExport;
    }
                
            }
            catch (Exception ex)
            {
                var messagebox = new Wpf.Ui.Controls.MessageBox { CloseButtonText = "Ок", Title = "Экспорт", Content = "Ошибка экспорта: " + ex.Message };
            }
        }





        public void ImportData(string filePath)
        {
            var context = ExpertSystemV2Entities.GetContext();
            try
            {
                var jsonData = File.ReadAllText(filePath);
                var importedSystems = JsonConvert.DeserializeObject<List<dynamic>>(jsonData);

                foreach (var system in importedSystems)
                {
                    // 1. Создаём новую систему
                    var newSystem = new ExpSystems
                    {
                        NameSys = system.NameSys,
                        ScopeOfApplication = system.ScopeOfApplication,
                        Description = system.Description,
                        TypeID = system.TypeID
                    };
                    GlobalDATA.SystemType= newSystem.TypeID;
                    context.ExpSystems.Add(newSystem);
                    context.SaveChanges(); // получаем ExpSysID

                    // --- ЛИНЕЙНАЯ СИСТЕМА ---
                    if (newSystem.TypeID == 0)
                    {
                        var questionMap = new Dictionary<int, int>();

                        // 2. Добавляем вопросы
                        foreach (var q in system.Questions)
                        {
                            var newQuestion = new Questions
                            {
                                ExpSysID = newSystem.ExpSysID,
                                Text = q.Text
                            };
                            context.Questions.Add(newQuestion);
                            context.SaveChanges();
                            questionMap[(int)q.QuestionID] = newQuestion.QuestionID;
                        }

                        // 3. Добавляем ответы (со связью NextQuestion)
                        foreach (var q in system.Questions)
                        {
                            int newQId = questionMap[(int)q.QuestionID];
                            foreach (var a in q.Answers)
                            {
                                int? nextId = null;
                                if (a.NextQuestion != null && questionMap.ContainsKey((int)a.NextQuestion))
                                {
                                    nextId = questionMap[(int)a.NextQuestion];
                                }

                                var newAnswer = new Answers
                                {
                                    QuestionID = newQId,
                                    Text = a.Text,
                                    Recommendation = a.Recommendation,
                                    NextQuestion = nextId
                                };
                                context.Answers.Add(newAnswer);
                            }
                        }

                        context.SaveChanges();
                    }
                    // --- ВЕСОВАЯ СИСТЕМА ---
                    else
                    {
                        var factMap = new Dictionary<int, int>();
                        var questionMap = new Dictionary<int, int>();
                        var answerMap = new Dictionary<int, int>();

                        // 2. Добавляем факты
                        foreach (var f in system.Facts)
                        {
                            var newFact = new Facts
                            {
                                ExpSysID = newSystem.ExpSysID,
                                Name = f.Name,
                                Description = f.Description
                            };
                            context.Facts.Add(newFact);
                            context.SaveChanges();
                            factMap[(int)f.FactID] = newFact.FactID;
                        }

                        // 3. Добавляем вопросы
                        foreach (var f in system.Facts)
                        {
                            int newFactId = factMap[(int)f.FactID];
                            foreach (var q in f.Questions)
                            {
                                var newQuestion = new Questions
                                {
                                    ExpSysID = newSystem.ExpSysID,
                                    FactID = newFactId,
                                    Text = q.Text
                                };
                                context.Questions.Add(newQuestion);
                                context.SaveChanges();
                                questionMap[(int)q.QuestionID] = newQuestion.QuestionID;

                                // 4. Добавляем ответы
                                foreach (var a in q.Answers)
                                {
                                    var newAnswer = new Answers
                                    {
                                        QuestionID = newQuestion.QuestionID,
                                        Text = a.Text,
                                        Recommendation = a.Recommendation
                                    };
                                    context.Answers.Add(newAnswer);
                                    context.SaveChanges();
                                    answerMap[(int)a.AnswerID] = newAnswer.AnswerID;

                                    // 5. Добавляем веса (WeightAnswers)
                                    foreach (var w in a.WeightAnswers ?? new List<dynamic>())
                                    {
                                        if (!factMap.ContainsKey((int)w.FactID)) continue;

                                        var newWeight = new WeightAnswers
                                        {
                                            AnswerID = newAnswer.AnswerID,
                                            FactID = factMap[(int)w.FactID],
                                            PlusOrMinus = w.PlusOrMinus,
                                            Value = w.Value
                                        };
                                        context.WeightAnswers.Add(newWeight);
                                    }
                                }
                            }
                        }

                        context.SaveChanges();
                    }

                    var messagebox = new Wpf.Ui.Controls.MessageBox
                    {
                        CloseButtonText = "Ок",
                        Title = "Импорт",
                        Content = "Файл успешно импортирован"
                    };
                }
            }
            catch (Exception ex)
            {
                var messagebox = new Wpf.Ui.Controls.MessageBox
                {
                    CloseButtonText = "Ок",
                    Title = "Импорт",
                    Content = "Ошибка импорта: " + ex.Message
                };
            }
        }


    }
}



