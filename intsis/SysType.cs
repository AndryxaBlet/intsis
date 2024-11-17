using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Data.Entity.Validation;
using System.Windows;

namespace intsis
{

    public class SqlJSON
    {
        public void ExportData(int systemId, string filePath) { }
        //{ try
        //    {
        //        var context = ExpertSystemEntities.GetContext();
        //        {
        //            // Извлекаем выбранную систему по ID
        //            var system = context.ExpSystem
        //                .Where(s => s.Id == systemId)
        //                .Select(s => new ImportedSystem
        //                {
        //                    ID = s.Id,
        //                    Name = s.Name,
        //                    ScopeOfApplication = s.ScopeOfApplication,
        //                    Comment = s.Description,
        //                    LinearSystem_Question = s.LinearSystem_Question.Select(r => new ImportedRule
        //                    {
        //                        Id = r.Id,
        //                        Text = r.Text,
        //                        Answers = r.LinearSystem_Answer.Select(a => new ImportedAnswer
        //                        {
        //                            ID = a.ID,
        //                            Ans = a.Ans,
        //                            NextR = a.NextR.ToString(),
        //                            Rec = string.IsNullOrEmpty(a.Rec) ? null : a.Rec,
        //                            Out = a.Out
        //                        }).ToList()
        //                    }).ToList()
        //                }).ToList();

        //            // Сериализация данных в JSON
        //            var jsonData = JsonConvert.SerializeObject(system, Formatting.Indented);

        //            // Запись данных в файл
        //            File.WriteAllText(filePath, jsonData);
        //            MessageBox.Show("Файл успешно сохранён");
        //        }
        //    }
        //    catch (Exception ex) { MessageBox.Show(ex.Message); }



        public void ImportData(string filePath)
        {


            var context = ExpertSystemEntities.GetContext();
            {
                var jsonData = File.ReadAllText(filePath);
                var importedSystems = JsonConvert.DeserializeObject<List<ExpSystem>>(jsonData);

                foreach (var system in importedSystems)
                {
                    // Создаем новую систему
                    var newSystem = new ExpSystem
                    {
                        Name = system.Name,
                        ScopeOfApplication = system.ScopeOfApplication,
                        Description = system.Description,
                        Type = system.Type
                    };

                    context.ExpSystem.Add(newSystem);
                    context.SaveChanges(); // Сохраняем систему, чтобы получить ID
                    if (newSystem.Type == false)
                    {
                        try
                        {
                            var ruleMapping = new Dictionary<int, int>(); // Словарь для соответствия оригинальных и новых ID

                            // Добавляем правила и заполняем словарь
                            foreach (var rule in system.LinearSystem_Question)
                            {
                                var newRule = new LinearSystem_Question
                                {
                                    SystemId = newSystem.Id,
                                    Text = rule.Text
                                };

                                context.LinearSystem_Question.Add(newRule);
                                context.SaveChanges(); // Сохраняем правило, чтобы получить Id

                                // Сохраняем соответствие Id из JSON и Id из базы данных
                                ruleMapping[rule.Id] = newRule.Id;
                            }

                            // Добавляем ответы, используя словарь для преобразования NextR
                            foreach (var rule in system.LinearSystem_Question)
                            {
                                // Получаем Id для текущего правила
                                var newRuleId = ruleMapping[rule.Id];

                                foreach (var answer in rule.LinearSystem_Answer)
                                {
                                    int? nextq = -1;
                                    if (answer.NextQuestionId != null)
                                    { nextq = ruleMapping[Convert.ToInt32(answer.NextQuestionId)]; }
                                    else nextq = null;

                                    var newAnswer = new LinearSystem_Answer
                                    {
                                        NextQuestionId = nextq, // Здесь используем правильный Id из словаря
                                        Text = answer.Text,
                                        Recomendation = answer.Recomendation,
                                        Out = answer.Out,
                                        QuestionId = newRuleId
                                    };

                                    context.LinearSystem_Answer.Add(newAnswer);
                                    context.SaveChanges();
                                }

                            }





                            context.SaveChanges(); // Сохраняем все изменения
                            MessageBox.Show("Файл успешно импортирован");
                        }
                        catch (Exception)
                        {
                            context.ExpSystem.Remove(newSystem);
                        }
                        finally
                        {
                            context.SaveChanges(); // Сохраняем все изменения в конце
                        }
                    }
                    else
                    {
                        try
                        {
                            var factMapping = new Dictionary<int, int>(); // Словарь для соответствия оригинальных и новых ID

                            // Добавляем факты
                            foreach (var fact in system.WeightedSystem_Fact)
                            {
                                var newFact = new WeightedSystem_Fact
                                {
                                    SystemId = newSystem.Id,
                                    Name = fact.Name,
                                    Text = fact.Text
                                };

                                context.WeightedSystem_Fact.Add(newFact);
                                context.SaveChanges(); // Сохраняем факт, чтобы получить ID

                                // Сохраняем соответствие IDFact
                                factMapping[fact.Id] = newFact.Id;
                            }

                            var QuestMapping = new Dictionary<int, int>(); // Словарь для соответствия оригинальных и новых ID
                                                                           // Добавляем вопросы, используя словарь для преобразования NextR
                            foreach (var fact in system.WeightedSystem_Fact)
                            {
                                // Получаем IDFact для текущего факта
                                var newFactId = factMapping[fact.Id];

                                foreach (var question in fact.WeightedSystem_Question)
                                {
                                    var newQuestion = new WeightedSystem_Question
                                    {
                                        FactID = newFactId, // Используем правильный IDFact из словаря
                                        Text = question.Text
                                    };

                                    context.WeightedSystem_Question.Add(newQuestion);
                                    context.SaveChanges(); // Сохраняем все изменения

                                    QuestMapping[question.Id] = newQuestion.Id;

                                    var AnsMapping = new Dictionary<int, int>(); // Словарь для соответствия оригинальных и новых ID
                                    var weightedSystem_Answer = question.WeightedSystem_Answer.ToList(); // Создаём копию коллекции

                                    foreach (var ans in weightedSystem_Answer)
                                    {
                                        var newQuestId = newQuestion.Id;
                                        var newAnswer = new WeightedSystem_Answer
                                        {
                                            QuestionId = newQuestId,
                                            Text = ans.Text,
                                            Recomendation = ans.Recomendation
                                        };

                                        context.WeightedSystem_Answer.Add(newAnswer);
                                        context.SaveChanges();
                                        AnsMapping[ans.Id] = newAnswer.Id; // Сохраняем соответствие оригинальных и новых ID

                                        // Привязываем WeightFactAnswer
                                        foreach (var wfa in ans.WeightFactAnswer)
                                        {
                                            var newWFA = new WeightFactAnswer
                                            {
                                                IdAnswer = newAnswer.Id,
                                                IdFact = factMapping[wfa.IdFact],
                                                PlusOrMinus = wfa.PlusOrMinus,
                                                Weight = wfa.Weight
                                            };

                                            context.WeightFactAnswer.Add(newWFA);
                                            context.SaveChanges();
                                        }
                                    }
                                }
                            }

                            // Сохраняем все изменения в конце, чтобы не вызывать SaveChanges() слишком часто
                            context.SaveChanges();

                            MessageBox.Show("Файл успешно импортирован");
                        }
                        catch (Exception ex)
                        {
                            // В случае ошибки удаляем созданную систему
                            context.ExpSystem.Remove(newSystem);
                            MessageBox.Show("Ошибка импорта данных: " + ex.Message);
                        }
                        finally
                        {
                            // Сохраняем все изменения в конце
                            context.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}
                   
                
            
   