using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace Sql2Poco
{
    public static class ScriptClassMaker 
    {
        private static string nl = Environment.NewLine;

        public static string MakeMultilineStringProperty(string propname, string s) {
            return "public static string " + propname + @" = @""" + nl + s + nl + @""";" + nl;
        }
        public static string MakeMapperFunc(List<string> resultTypes, List<string> resultNames, string resultWrapperName) {
            var result = "//Usage: _db.FetchMultiple<T1,T2,T3,T4>(SqlScript.Sql, new SqlParams {}.GetParams(), SqlScript.MakeMapperFunc)" + nl;
            result += "public static Func<";
            foreach (var r in resultTypes)
            {
                result += $"List<{r}>, ";
            }
            result += resultWrapperName + "> DefaultMapper()" + nl + "{" + nl;
            var varNames = "wxyz";
            
            result += "return (";
            for (int i = 0; i < resultTypes.Count && i < 4; i++)
            {
                result += varNames[i] + ",";
            }
            result = result.Remove(result.Length - 1) + ") =>" + nl + "{" + nl;
            result += "return new " + resultWrapperName + nl + "{" + nl;

            for (int i = 0; i < resultNames.Count && i < 4; i++)
            {
                var resultName = resultNames[i];
                var selector = ".First()";
                if (resultNames[i] == resultTypes[i]) {
                    resultName += "Results";
                    selector = "";
                } 
                result += $"{resultName} = {varNames[i]}{selector},";
            }
            result = result.Remove(result.Length - 1) + nl + "};" + nl + "};" + nl + "}" + nl;
            return result;
        }
        //     public static object[] TestParamValues =>
        //new CalendarParams
        //{
        //    StartDate = DateTime.Parse("8/26/2016")
        //    ,
        //    CheckRunDate = DateTime.Parse("9/1/2016")
        //    ,
        //    Days = 41
        //    ,
        //    CheckRunType = "w"
        //}.GetParamValues();
        public static string MakeTestParams(string paramsClassName, List<ResultFieldDetails> scriptParams)
        {
            var result = "//Usage: _db.FetchMultiple<T1,T2,T3,T4>(SqlScript.Sql, SqlScript.TestParamValues, SqlScript.MakeMapperFunc)" + nl;
            result += "public static object[] TestParamValues => new " + paramsClassName + nl + "{";
            foreach (var sp in scriptParams)
            {
                if (!String.IsNullOrEmpty(sp.FieldValue.ToString())) {
                    result += nl + sp.CSColumnName + " = " + TypeMapping.FieldValueString(sp.CSType, sp.FieldValue) + ",";
                }
            }
            if (result[result.Length-1] == ',')
            {
                result = result.Remove(result.Length - 1);
            }
            return result + "}.GetParamValues();" + nl + nl;

        }
        public static string StartClass(string className)
        {
            return string.Format("public static class {0} {{" + nl, className);
        }
        public static string CloseClass()
        {
            return "}" + nl;
        }
        public static string MakeScript(string baseName, SqlScript script, List<string> resultTypes, List<ResultFieldDetails> scriptParams)
        {
            var result = StartClass(baseName);
            if (resultTypes.Count > 1)
            {
                result += MakeMapperFunc(resultTypes, script.ResultNames, baseName + "Result");
            }
            result += MakeTestParams(baseName + "Params", scriptParams);
            result += MakeMultilineStringProperty("Sql", script.DeclareText + script.InitializeParamsText + script.ExecutionText);
            return result += CloseClass();
        }

    }
}
