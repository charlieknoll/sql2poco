using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.OLE.Interop;

namespace Sql2Poco
{
    public class PocoBuilder
    {
        
        
        private Action<string> _logger;
        private DocumentHelper _documentHelper;
        private SqlScript _sqlScript;
        
        public PocoBuilder(DocumentHelper helper, string sqlText,  Action<string> logger)
        {
            _logger = logger;
            _documentHelper = helper;
            _sqlScript = new SqlScript(sqlText, _documentHelper.BaseName);
        }

        public List<ResultFieldDetails> ResultFields { get; set; }
        public string GenerateCode(string connectionStr, string providerName)
        {
            try
            {

                List<ResultFieldDetails> scriptParamFields;
                List<List<ResultFieldDetails>> resultSets;
                // Execute script
                try {
                    var paramSql = _sqlScript.DeclareAndInitTestParamsText +
                          Environment.NewLine + _sqlScript.SelectTestParamsQuery;
                    scriptParamFields = AdoHelper.GetResults(connectionStr, paramSql).First();
                    scriptParamFields = AdoHelper.SetFieldDetailValues(scriptParamFields, connectionStr, paramSql);
                    resultSets = AdoHelper.GetResults(connectionStr,_sqlScript.TestingText );
                    //ResultFields = _adoHelper.GetFields(connectionStr, _sqlScript.TestingText);
                }
                catch (Exception ex)
                {
                    _logger($"Error running query: {_documentHelper.BaseName}.sql");
                    StringBuilder bldr = new StringBuilder();
                    bldr.AppendLine("/*The last attempt to run this query failed with the following error. This class is no longer synced with the query");
                    bldr.AppendLine("You can compile the class by deleting this error information, but it will likely generate runtime errors.");
                    bldr.AppendLine("-----------------------------------------------------------");
                    bldr.AppendLine(ex.Message);
                    bldr.AppendLine("-----------------------------------------------------------");
                    bldr.AppendLine(ex.StackTrace);
                    bldr.AppendLine("*/");
                    return bldr.ToString();
                }
                
                var code = new StringBuilder();
                code.Append(WrapperClassMaker.Usings());
                code.Append(WrapperClassMaker.StartNamespace(_documentHelper.FileNameSpace));
                code.Append(ParamClassMaker.MakeParams(_documentHelper.BaseName + "Params", scriptParamFields));
                if (_sqlScript.ResultNames.Count != resultSets.Count)
                {
                    code.Append(WrapperClassMaker.Comment("Add a '" + 
                        SqlConst.ResultNamesDelimiter + 
                        "' section to the top of the sql file to override resultset names"));
                }
                var i = 0;
                foreach (var r in resultSets) {
                    if (r.Count > 1)
                    {
                        code.Append(ResultClassMaker.MakeClass(i > _sqlScript.ResultNames.Count - 1 ? "Result" + i : _sqlScript.ResultNames[i], r));
                    }
                    i++;
                }
                var resultTypes = new List<string>();
                if (resultSets.Count > 1) {
                    i = 0;
                    code.Append(ResultClassMaker.StartClass(_documentHelper.BaseName + "Result"));

                    foreach (var r in resultSets)
                    {
                        if (r.Count > 1)
                        {
                            var resultName = i > _sqlScript.ResultNames.Count - 1 ? "Result" + i : _sqlScript.ResultNames[i];
                            code.Append(ResultClassMaker.MakeResultProperty(resultName));
                            resultTypes.Add(resultName);
                        }
                        else {
                            code.Append(ResultClassMaker.MakeProperty(r[0]));
                            resultTypes.Add(r[0].CSType);
                        }

                        i++;
                    }
                    code.Append(ResultClassMaker.CloseClass());
                }

                code.Append(ScriptClassMaker.MakeScript(_documentHelper.BaseName, _sqlScript,resultTypes,scriptParamFields));
                code.Append(WrapperClassMaker.CloseNamespace());
                _logger(Environment.NewLine + "Sql2Poco generated parameter and results classes for " + _documentHelper.BaseName + ".sql");
                return code.ToString();

            }
            catch (Exception ex)
            {
                _logger(Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return "";
        }

    }
}
