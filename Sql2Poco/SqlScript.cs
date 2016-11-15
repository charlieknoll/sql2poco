using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EnvDTE;

namespace Sql2Poco
{
    public static class SqlConst {
        public static string TestSectionBeginDelimiter = "--Sql2Poco Begin Set Test Params";
        public static string TestSectionEndDelimiter = "--Sql2Poco End Set Test Params";
        public static string ResultNamesDelimiter = "--Sql2Poco-Result Names(";
    }
    public class SqlScript
    {

        
        private readonly string _text;
        private string _baseName;

        public SqlScript(string text, string baseName)
        {
            _baseName = baseName;
            _text = text;
        }
        public string DeclareText { get {
                
                return _text.Split(new[] { SqlConst.TestSectionBeginDelimiter }, StringSplitOptions.None)[0];
            } }
        public string DeclareAndInitTestParamsText { get {
                return _text.Split(new[] { SqlConst.TestSectionEndDelimiter }, StringSplitOptions.None)[0];
            } }
        public List<string> ParamNames { get {
                var result = new List<string>();

                string pattern = "declare (.*)";
                Match m = Regex.Match(DeclareText, pattern, RegexOptions.IgnoreCase);
                while (m.Success)
                {
                    string[] parts = m.Value.Split(' ');
                    result.Add(parts[1]);
                    m = m.NextMatch();
                }
                return result;
            }
        }

        public string SelectTestParamsQuery
        {
            get
            {
                string query = "";
                foreach (var p in ParamNames)
                {
                    query += (String.IsNullOrEmpty(query) ? "SELECT " + p : ", " + p) + " as " + p.Substring(1);
                }
                return query;
            }
        }
        public string InitializeParamsText { get {
                string initSql = "";
                int i = 0;
                foreach (var p in ParamNames) {
                    initSql = initSql + "SELECT " + p + " = @" + i + Environment.NewLine;
                    i++;
                }
                return initSql; 
            } }
        public List<string> ResultNames { get {
                var result = new List<string>();

                var pos = _text.IndexOf(SqlConst.ResultNamesDelimiter, StringComparison.Ordinal);
                if (pos < 0)
                {
                    result.Add(_baseName + "Result");
                    return result;
                }
                var str = _text.Split(new[] { SqlConst.ResultNamesDelimiter }, StringSplitOptions.None)[1];
                pos = str.IndexOf(")", StringComparison.Ordinal);
                if (pos < 0) return result;
                result.AddRange(str.Substring(0,pos).Split(','));
                return result;

                
            } }  
        public string ExecutionText { get {
                return _text.Split(new[] { SqlConst.TestSectionEndDelimiter }, StringSplitOptions.None)[1];
            } }
        public string TestingText { get { return _text; } }

    }
}
