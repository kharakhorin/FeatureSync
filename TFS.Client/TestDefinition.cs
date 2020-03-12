using System;
using System.Collections.Generic;
using System.Linq;

namespace TFS.Client
{
    public class TestDefinition
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; }
        public string AutomatedTestName { get; set; }
        public string AutomatedTestStorage { get; set; }
        public string AutomatedTestId { get; set; } = Guid.NewGuid().ToString();
        public StepsDefinition Steps { get; set; } = new StepsDefinition();
        public string Summary { get; set; }

        public Dictionary<string, object> Fields
        {
            get
            {
                var fields = new Dictionary<string, object>{
                    { "Microsoft.VSTS.TCM.AutomatedTestName", AutomatedTestName },
                    { "Microsoft.VSTS.TCM.AutomatedTestStorage", AutomatedTestStorage },
                    { "Microsoft.VSTS.TCM.AutomatedTestType", "Unit Test" },
                    { "Microsoft.VSTS.TCM.AutomationStatus", "Automated" },
                    { "Microsoft.VSTS.TCM.AutomatedTestId", AutomatedTestId }};

                if (Name != null)
                    fields.Add("System.Title", Name);
                if (Summary != null)
                    fields.Add("System.Description", Summary);
                if (Steps.StepCount > 0)
                    fields.Add("Microsoft.VSTS.TCM.Steps", Steps.StepsDefinitionStr);

                return fields;
            }
        }

        public Dictionary<string, object> GetDiffFields(IDictionary<string, object> existingFields)
        {
            if (existingFields.ContainsKey("Microsoft.VSTS.TCM.AutomatedTestId") && existingFields["Microsoft.VSTS.TCM.AutomatedTestId"] != null)
                AutomatedTestId = existingFields["Microsoft.VSTS.TCM.AutomatedTestId"].ToString();

            var newFields = Fields;
            return newFields.Where(x => !existingFields.ContainsKey(x.Key) || x.Value.ToString() != existingFields[x.Key].ToString())
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
