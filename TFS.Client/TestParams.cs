using System;
using System.Collections.Generic;
using System.Linq;

namespace TFS.Client
{
    public class TestParams
    {
        private const string ParamDataSourceContainerStr = @"<NewDataSet>{0}{1}{2}{3}</NewDataSet>";
        private const string XSSchemaStrStart = @"<xs:schema id='NewDataSet' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:msdata='urn:schemas-microsoft-com:xml-msdata'><xs:element name='NewDataSet' msdata:IsDataSet='true' msdata:Locale=''><xs:complexType><xs:choice minOccurs='0' maxOccurs = 'unbounded'><xs:element name='Table1'><xs:complexType><xs:sequence>";
        private const string XSSchemaStrEnd = @"</xs:sequence></xs:complexType></xs:element></xs:choice></xs:complexType></xs:element></xs:schema>";
        private const string XSParamStrConatiner = @"<xs:element name='{0}' type='xs:string' minOccurs='0' />";
        private const string DSParamTableStrContainer = @"<Table1>{0}</Table1>";
        private const string DSParamTableParamStrContainer = @"<{0}>{1}</{0}>";
        private const string ParametersDefinitionStrContainer = @"<parameters>{0}</parameters>";
        private const string ParameterDefinitionStrContainer = @"<param name=""{0}"" bind=""default""/>";

        private Dictionary<string, string[]> ParamValues = new Dictionary<string, string[]>();

        public int ParamCount
        {
            get { return ParamValues.Count; }
        }

        public void AddParam(string Name, string[] Values)
        {
            ParamValues.Add(Name, Values);
        }

        /// <summary>
        /// Format string for parameters definition (Field: Microsoft.VSTS.TCM.Parameters)
        /// </summary>
        public string ParamDefinitionStr
        {
            get
            {
                if (ParamValues.Count == 0) return null;

                string parameters = "";

                foreach (var param in ParamValues) parameters += String.Format(ParameterDefinitionStrContainer, param.Key);

                return String.Format(ParametersDefinitionStrContainer, parameters);
            }
        }

        /// <summary>
        /// Format string for the table with values
        /// </summary>
        private string ParamValuesStr
        {
            get
            {
                if (ParamValues.Count == 0) return null;

                int paramValuesCount = ParamValues.ElementAt(0).Value.Length; //just get the count from the first parameter;

                string tableRowStr = "";

                for (int i = 0; i < paramValuesCount; i++)
                {
                    string tableRowParams = "";

                    foreach (var param in ParamValues)
                        if (i < param.Value.Length)
                            tableRowParams += String.Format(DSParamTableParamStrContainer, param.Key, param.Value[i]); //add parameter value for the iteration i

                    tableRowStr += String.Format(DSParamTableStrContainer, tableRowParams);
                }

                return tableRowStr;
            }
        }

        /// <summary>
        /// Format string for the definition in the schema 
        /// </summary>
        private string ParamsSchemaDefStr
        {
            get
            {
                if (ParamValues.Count == 0) return null;

                string parameters = "";

                foreach (var param in ParamValues) parameters += String.Format(XSParamStrConatiner, param.Key);

                return parameters;
            }
        }

        /// <summary>
        /// Form string for parameters values (Field: Microsoft.VSTS.TCM.LocalDataSource)
        /// </summary>
        public string ParamDataSetStr
        {
            get
            {
                return String.Format(ParamDataSourceContainerStr, XSSchemaStrStart, ParamsSchemaDefStr, XSSchemaStrEnd, ParamValuesStr);
            }
        }
    }

}
