using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Sql2Poco
{
    public static class ResultClassMaker
    {

        private static string nl = Environment.NewLine;
        public static string StartClass(string className)
        {
            return string.Format("public partial class {0} {{" + nl, className);
        }
        public static string MakeProperty(ResultFieldDetails fld)
        {
            return string.Format("public {0} {1} {{ get; set; }} //({2} {3})" + nl, fld.CSType, fld.ColumnName, fld.DataTypeName, fld.AllowDBNull ? "null" : "not null");
        }
        public static string MakeResultProperty(string className) {
            return string.Format("public List<{0}> {1}Results {{ get; set; }}" + nl, className, className);
        }
        public static string CloseClass()
        {
            return "}" + nl;
        }

        public static string MakeClass(string className, List<ResultFieldDetails> fields)
        {
            var result = ResultClassMaker.StartClass(className);
            foreach (var p in fields)
            {
                result += ResultClassMaker.MakeProperty(p);
            }
            return result += ResultClassMaker.CloseClass();
        }
    }
}
