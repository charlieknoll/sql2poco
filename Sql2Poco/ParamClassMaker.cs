using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Sql2Poco
{
    public static class ParamClassMaker 
    {
        private static string nl = Environment.NewLine;

        public static string MakeGetParamValues(List<ResultFieldDetails> resultFieldDetails) {
            var result = "public object[] GetParamValues()" + nl + "{";
            if (resultFieldDetails.Count == 0)
            {
               result += "return new object[0];" + nl;
            }
            var cols = "";
            foreach (var r in resultFieldDetails) {
                cols += String.IsNullOrWhiteSpace(cols) ? "return new object[] {" + r.CSColumnName : "," + r.CSColumnName;
            }

            return result + cols +  "};" + nl + "}" + nl;
        }


        public static string MakeParams(string className, List<ResultFieldDetails> scriptParamFields)
        {
            var result = ResultClassMaker.StartClass(className);
            foreach (var p in scriptParamFields) {
                result += ResultClassMaker.MakeProperty(p);
            }
            result += MakeGetParamValues(scriptParamFields);
            return result += ResultClassMaker.CloseClass();
        }
    }
}
