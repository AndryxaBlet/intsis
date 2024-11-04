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
        public void ExportData(int systemId, string filePath)
        {
            try
            {
                var context = intsisEntities.GetContext();
                {
                    // Извлекаем выбранную систему по ID
                    var system = context.NameSis
                        .Where(s => s.ID == systemId)
                        .Select(s => new ImportedSystem
                        {
                            ID = s.ID,
                            Name = s.Name,
                            ScopeOfApplication = s.ScopeOfApplication,
                            Comment = s.Comment,
                            Rules = s.Rules.Select(r => new ImportedRule
                            {
                                IDRule = r.IDRule,
                                Text = r.Text,
                                Answers = r.Answer.Select(a => new ImportedAnswer
                                {
                                    ID = a.ID,
                                    Ans = a.Ans,
                                    NextR = a.NextR.ToString(),
                                    Rec = string.IsNullOrEmpty(a.Rec) ? null : a.Rec,
                                    Out = a.Out
                                }).ToList()
                            }).ToList()
                        }).ToList();

                    // Сериализация данных в JSON
                    var jsonData = JsonConvert.SerializeObject(system, Formatting.Indented);

                    // Запись данных в файл
                    File.WriteAllText(filePath, jsonData);
                    MessageBox.Show("Файл успешно сохранён");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }


        public void ImportData(string filePath)
        {
            
            var context = intsisEntities.GetContext();
            {
                var jsonData = File.ReadAllText(filePath);
                var importedSystems = JsonConvert.DeserializeObject<List<ImportedSystem>>(jsonData);

                foreach (var system in importedSystems)
                {
                    // Создаем новую систему
                    var newSystem = new NameSis
                    {
                        Name = system.Name,
                        ScopeOfApplication = system.ScopeOfApplication,
                        Comment = system.Comment
                    };

                    context.NameSis.Add(newSystem);
                    context.SaveChanges(); // Сохраняем систему, чтобы получить ID
                    try
                    {
                        var ruleMapping = new Dictionary<int, int>(); // Словарь для соответствия оригинальных и новых ID

                        // Добавляем правила и заполняем словарь
                        foreach (var rule in system.Rules)
                        {
                            var newRule = new Rules
                            {
                                IDSis = newSystem.ID,
                                Text = rule.Text
                            };

                            context.Rules.Add(newRule);
                            context.SaveChanges(); // Сохраняем правило, чтобы получить IDRule

                            // Сохраняем соответствие IDRule из JSON и IDRule из базы данных
                            ruleMapping[rule.IDRule] = newRule.IDRule;
                        }

                        // Добавляем ответы, используя словарь для преобразования NextR
                        foreach (var rule in system.Rules)
                        {
                            // Получаем IDRule для текущего правила
                            var newRuleId = ruleMapping[rule.IDRule];

                            foreach (var answer in rule.Answers)
                            {
                                // Преобразуем NextR на основе словаря
                                int? nextR = string.IsNullOrEmpty(answer.NextR)
                                    ? (int?)null
                                    : (ruleMapping.TryGetValue(int.Parse(answer.NextR), out var mappedId) ? (int?)mappedId : null);

                                var newAnswer = new Answer
                                {
                                    IDRule = newRuleId, // Здесь используем правильный IDRule из словаря
                                    Ans = answer.Ans,
                                    NextR = nextR, // Здесь устанавливаем новое значение
                                    Rec = answer.Rec,
                                    Out = answer.Out
                                };

                                context.Answer.Add(newAnswer);

                            }
                        }
                    

                          context.SaveChanges(); // Сохраняем все изменения
                        MessageBox.Show("Файл успешно импортирован");
                    }
                    catch (Exception)
                    {
                        context.NameSis.Remove(newSystem);
                    }
                    finally
                    {
                        context.SaveChanges(); // Сохраняем все изменения в конце

                    }
                   
                }
            }
        }
    

        // Класс для десериализации JSON
        public class ImportedSystem
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string ScopeOfApplication { get; set; }
            public string Comment { get; set; }
            public List<ImportedRule> Rules { get; set; }
        }

        public class ImportedRule
        {
            public int IDRule { get; set; }
            public string Text { get; set; }
            public List<ImportedAnswer> Answers { get; set; }
        }

        public class ImportedAnswer
        {
            public int ID { get; set; }
            public string Ans { get; set; }
            public string NextR { get; set; }
            public string Rec { get; set; }
            public string Out { get; set; }
            
        }
    }
}
