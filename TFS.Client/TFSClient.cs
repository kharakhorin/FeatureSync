using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace TFS.Client
{
    public class TFSClient
    {
        private WorkItemTrackingHttpClient WitClient { get; }
        private TestPlanHttpClient TestPlanClient { get; }

        public TFSClient(string url, string token)
        {
            var connection = new VssConnection(new Uri(url), new VssBasicCredential(string.Empty, token));

            WitClient = connection.GetClient<WorkItemTrackingHttpClient>();
            TestPlanClient = connection.GetClient<TestPlanHttpClient>();
        }

        public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem CreateWorkItem(string ProjectName, string WorkItemTypeName, Dictionary<string, object> Fields)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            foreach (var key in Fields.Keys)
                patchDocument.Add(new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/" + key,
                    Value = Fields[key]
                });

            return WitClient.CreateWorkItemAsync(patchDocument, ProjectName, WorkItemTypeName).Result;
        }

        public Task<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> UpdateWorkItem(int WIId, Dictionary<string, object> Fields)
        {

            JsonPatchDocument patchDocument = new JsonPatchDocument();

            foreach (var key in Fields.Keys)
                patchDocument.Add(new JsonPatchOperation()
                {

                    Operation = Operation.Add,
                    Path = "/fields/" + key,
                    Value = Fields[key]

                });

            return WitClient.UpdateWorkItemAsync(patchDocument, WIId);

        }

        public int CreateTestCase(string TeamProjectName, TestDefinition test)
        {
            return CreateWorkItem(TeamProjectName, "Test Case", test.Fields).Id.Value;
        }

        async public Task UpdateTestCase(TestDefinition test)
        {
            var existingTest = GetWorkItem(test.Id);

            if (existingTest is null)
            {
                Console.WriteLine($"Не синхронизирован тест '{test.Id}'. В TFS отсутствует элемент с таким Id");
                return;
            }

            var fieldsForUpdate = test.GetDiffFields(existingTest.Fields);

            if (fieldsForUpdate.Count() > 0)
            {
                await UpdateWorkItem(test.Id, fieldsForUpdate);
                Console.WriteLine(@$"Cинхронизирован тест {test.Id}, поля: {string.Join(", ", fieldsForUpdate.Select(x => x.Key))}");
            }
            else
                Console.WriteLine($"Тест {test.Id} не нуждается в синхронизации");
        }

        public void AddTestCasesToSuite(List<int> testCasesIds, string teamProjectName, int testPlanId, int testSuiteId)
        {
            TestSuite testSuite = TestPlanClient.GetTestSuiteByIdAsync(teamProjectName, testPlanId, testSuiteId).Result;

            if (testSuite.SuiteType == TestSuiteType.StaticTestSuite || testSuite.SuiteType == TestSuiteType.DynamicTestSuite)
            {
                List<SuiteTestCaseCreateUpdateParameters> suiteTestCaseCreateUpdate = new List<SuiteTestCaseCreateUpdateParameters>();

                foreach (int testCaseId in testCasesIds)
                    suiteTestCaseCreateUpdate.Add(new SuiteTestCaseCreateUpdateParameters()
                    {
                        workItem = new Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.WorkItem()
                        {
                            Id = testCaseId
                        }
                    });

                TestPlanClient.AddTestCasesToSuiteAsync(suiteTestCaseCreateUpdate, teamProjectName, testPlanId, testSuiteId).Wait();
            }
        }

        public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem GetWorkItem(int Id)
        {
            try
            {
                return WitClient.GetWorkItemAsync(Id).Result;
            }
            catch
            {
                return null;
            }
        }
    }
}
