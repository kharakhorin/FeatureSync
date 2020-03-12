using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Gherkin;
using Gherkin.Ast;
using TFS.Client;

namespace FeatureSync
{
    public static class TestCaseParser
    {
        public static List<TestDefinition> ParseFeatures(string[] featureFiles)
        {
            var parsedTests = new List<TestDefinition>();

            foreach (var path in featureFiles)
            {
                Console.WriteLine($"Парсинг файла: \n{path}");

                var gherkinDocument = new Parser().Parse(path);

                foreach (Scenario scenario in gherkinDocument.Feature.Children.Where(c => c.GetType() == typeof(Scenario)))
                    parsedTests.Add(ConvertScenarioToTest(scenario, gherkinDocument.Feature));
            }

            ValidateTests(parsedTests);
            return parsedTests;
        }

        private static TestDefinition ConvertScenarioToTest(Scenario scenario, Feature feature)
        {
            var test = new TestDefinition();
            var background = (Background)feature.Children.FirstOrDefault(c => c.GetType() == typeof(Background));
            var testsNamespace = GetFeatureNamespace(feature);

            test.Name = scenario.Name;

            var tagId = scenario.Tags.FirstOrDefault(t => new Regex(@"^@\d+$").IsMatch(t.Name));
            if (tagId != default(Tag))
            {
                test.Id = Convert.ToInt32(tagId.Name.Remove(0, 1));
                test.AutomatedTestStorage = testsNamespace + ".dll";
                test.AutomatedTestName = string.Join("", string.Join("_", $"{testsNamespace}.{feature.Name}Feature.{scenario.Name}"
                    .Split('-').Select(w => w.Substring(0, 1).ToUpper() + w.Substring(1)))
                    .Split(' ').Select(w => w.Substring(0, 1).ToUpper() + w.Substring(1)))
                    .Replace(" ", "").Replace("-", "_").Replace("[", "").Replace("]", "")
                    .Replace("(", "").Replace(")", "").Replace(",", "").Replace("=", "")
                    .Replace("+", "");
            }

            if (background != null)
            {
                var sb = new StringBuilder();
                foreach (var bStep in background.Steps)
                    sb.AppendLine(ContextStepToString(bStep));
                test.Summary = sb.ToString();
            }

            foreach (var step in scenario.Steps)
                test.Steps.AddStep(HttpUtility.HtmlEncode(StepToString(step)));

            return test;
        }

        private static string StepToString(Step step)
        {
            var strBuld = new StringBuilder();
            strBuld.Append($"<b>{step.Keyword}</b>");
            strBuld.Append(HttpUtility.HtmlEncode($"{step.Text.Substring(0, 1).ToLower() + step.Text.Substring(1)}\n"));
            if (step.Argument != null && step.Argument.GetType() == typeof(DataTable))
            {
                strBuld.AppendLine();
                strBuld.AppendLine(DataTableToHtml((DataTable)step.Argument));
            }
            return strBuld.ToString();
        }

        private static string ContextStepToString(Step step)
        {
            var strBuld = new StringBuilder();
            strBuld.Append($"<b>{step.Keyword}</b>");
            strBuld.Append($"{step.Text.Substring(0, 1).ToLower() + step.Text.Substring(1)}\n");
            if (step.Argument != null && step.Argument.GetType() == typeof(DataTable))
            {
                strBuld.AppendLine();
                strBuld.AppendLine(DataTableToHtml((DataTable)step.Argument));
            }
            return $"<div>{strBuld.ToString()}</div>"; ;
        }

        private static string DataTableToHtml(DataTable table)
        {
            var strBuld = new StringBuilder();

            strBuld.AppendLine(@"<table style='border:1pt solid black; border-spacing: 1px' border=1 cellpadding=3>");
            strBuld.AppendLine("<tr>");
            foreach (var cell in table.Rows.First().Cells)
                strBuld.AppendLine(@$"<th align=""center""><i>{cell.Value}</i></th>");
            strBuld.AppendLine("<tr>");

            foreach (var row in table.Rows.Skip(1))
            {
                strBuld.AppendLine("<tr>");
                foreach (var cell in row.Cells)
                    strBuld.AppendLine(@$"<td align=""center"">{HttpUtility.HtmlEncode(cell.Value)}</td>");
                strBuld.AppendLine("<tr>");
            }

            strBuld.Append(@"</table>");
            return strBuld.ToString();
        }

        private static string GetFeatureNamespace(Feature feature)
        {
            var testsNamespace = "";
            try
            {
                testsNamespace = feature.Tags.First(t => new Regex(@"^@Namespace:(.*)$").IsMatch(t.Name)).Name.Replace("@Namespace:", "");
            }
            catch
            {
                throw new Exception($"У Функции {feature.Name} не найден тег Namespace! Добавте тег, пример '@Namespace:Product.Autotests'");
            }
            return testsNamespace;
        }

        private static void ValidateTests(List<TestDefinition> tests)
        {
            var duplicateId = tests.GroupBy(x => x.Id).Where(x => x.Count() > 1 && x.First().Id > 0).Select(x => x.First().Id);

            foreach (var test in tests.ToList())
            {
                var delete = false;

                if (duplicateId.Contains(test.Id))
                {
                    Console.WriteLine($"Не синхронизирован сценарий '{test.Name}' c Id = '{test.Id}'. Несколько сценариев имеют такой Id");
                    delete = true;
                }
                if (test.Id == 0)
                {
                    Console.WriteLine($"Не синхронизирован сценарий '{test.Name}'. Отсутствует Id");
                    delete = true;
                }
                if (string.IsNullOrEmpty(test.Name))
                {
                    Console.WriteLine($"Не синхронизирован сценарий '{test.Name}'. Отсутствует имя сценария");
                    delete = true;
                }               
                if (test.Steps.StepCount == 0)
                {
                    Console.WriteLine($"Не синхронизирован сценарий '{test.Name}'. Отсутствуют шаги");
                    delete = true;
                }

                if (delete)
                    tests.Remove(test);
            }
        }
    }
}
