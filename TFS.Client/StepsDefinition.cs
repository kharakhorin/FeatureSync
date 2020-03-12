using System;
using System.Collections.Generic;

namespace TFS.Client
{
    public class StepsDefinition
    {
        class TestStep
        {
            private const string StepContainerStr = @"<step type=""{0}"" id=""{1}""><parameterizedString isformatted=""true"">{2}</parameterizedString><parameterizedString isformatted=""true"">{3}</parameterizedString></step>";
            public string Action { get; set; }
            public string Validation { get; set; }

            /// <summary>
            /// Format string for the current step
            /// </summary>
            /// <param name="StepIndex">index in the steps order (+2 to the index in the steps list)</param>
            /// <returns></returns>
            public string StepStr(int StepIndex)
            {
                string StepType = "ActionStep";
                if (Validation != null)
                    StepType = "ValidateStep";

                return String.Format(StepContainerStr, StepType, StepIndex, Action, Validation);
            }
        }


        private const string StepsContainerStr = @"<steps id=""0"" last=""{0}"">{1}</steps>";

        private List<TestStep> localTestSteps = new List<TestStep>();

        public void AddStep(string Action, string Validation = null)
        {
            localTestSteps.Add(new TestStep { Action = Action, Validation = Validation });
        }

        public int StepCount
        {
            get { return localTestSteps.Count; }
        }

        /// <summary>
        /// Format string for all steps (Field: Microsoft.VSTS.TCM.Steps)
        /// </summary>
        public string StepsDefinitionStr
        {
            get
            {
                if (localTestSteps.Count == 0) return null;

                string stepsStr = "";

                //add definition for each step
                for (int i = 0; i < localTestSteps.Count; i++) stepsStr += localTestSteps[i].StepStr(i + 2);

                return String.Format(StepsContainerStr, localTestSteps.Count + 1, stepsStr);
            }
        }
    }

}
