using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sql2Poco
{
    public static class WrapperClassMaker 
    {
        public static  string Usings()
        {
            return @"using System;
using System.Collections.Generic;
using System.Linq;" + nl ;

        }
        public static string StartNamespace(string  ns)
        {
            if (!string.IsNullOrEmpty(ns))
                return "namespace " + ns + "{" + nl;
            else
                return "";
        }
        public static string Comment(string comment) {
            return "//" + comment + nl;
        }

        private static string nl = Environment.NewLine;
        public static string StartClass(string className)
        {
            return string.Format("public class {0} {{" + nl, className);
        }

        public static string CloseClass()
        {
            return "}" +nl;
        }
        public static string CloseNamespace()
        {
                return "}" + nl;
        }
    }
}
