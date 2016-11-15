using System;
using System.IO;
using EnvDTE;

namespace Sql2Poco
{
    class Root
    {
        // singleton
        private static Root instance = null;
        public static Root Get(DTE dte)
        {
            if (instance == null)
            {
                instance = new Root(dte);
            }
            return instance;
        }
        
        private DTE _dte;
        private EnvDTE.Events myEvents;
        private EnvDTE.DocumentEvents myDocumentEvents;
        ProjectItemsEvents CSharpProjectItemsEvents;
        private string _connectionStringName;
        private string _providerName;
        private string _projectPath;

        private Root(DTE dte)
        {
            _dte = dte;
            myEvents = dte.Events;
            myDocumentEvents = dte.Events.DocumentEvents;
            myDocumentEvents.DocumentSaved += myDocumentEvents_DocumentSaved;
            myDocumentEvents.DocumentOpened += MyDocumentEvents_DocumentOpened;
            CSharpProjectItemsEvents = (ProjectItemsEvents)dte.Events.GetObject("CSharpProjectItemsEvents");
            CSharpProjectItemsEvents.ItemRenamed += CSharpItemRenamed;

        }
        private void MyDocumentEvents_DocumentOpened(Document Document)
        {

            if (Document.FullName.EndsWith(".sql2poco.cs"))
            {
                var textDoc = ((TextDocument)Document.Object());
                textDoc.StartPoint.CreateEditPoint().SmartFormat(textDoc.EndPoint);
                Document.Save();
            }


        }
        void CSharpItemRenamed(ProjectItem renamedScript, string oldName)
        {
            if (oldName.EndsWith(".sql"))
            {
                foreach (ProjectItem item in renamedScript.ProjectItems)
                {
                    string folder = Path.GetDirectoryName((string)renamedScript.Properties.Item("FullPath").Value);
                    if (((string)item.Properties.Item("FullPath").Value).StartsWith(folder))
                    {
                        if (item.Name == oldName.Replace(".sql", ".sql2poco.cs"))
                        {
                            item.Name = renamedScript.Name.Replace(".sql", ".sql2poco.cs");
                            var sql = "";
                            using (var streamReader = new StreamReader((string)renamedScript.Properties.Item("FullPath").Value)) {
                                sql = streamReader.ReadToEnd();
                            }
                            _projectPath = renamedScript.ContainingProject.Properties.Item("FullPath").Value.ToString();
                            var documentHelper = new DocumentHelper(renamedScript.Properties.Item("FullPath").Value.ToString(),
                                   _projectPath,
                                   renamedScript.ContainingProject.Properties.Item("DefaultNamespace").Value.ToString()
                                   );
                            buildAndWriteCode(sql, renamedScript,documentHelper);
                        }
                    }
                }
            }
        }

        private string GetConnectionInfo() {
            ConfigurationAccessor config = new ConfigurationAccessor(_dte, null);
            var connectionStringName = String.Empty;
            try
            {
                connectionStringName = config.AppSettings["Sql2PocoConnectionName"].Value;
            }
            catch {}
            var connSection = config.ConnectionStrings;
            string result = "";
            //if the connectionString is empty - which is the defauls
            //look for count-1 - this is the last connection string
            //and takes into account AppServices and LocalSqlServer
            if (string.IsNullOrEmpty(connectionStringName))
            {
                if (config.ConnectionStrings.Count > 1)
                {
                    _connectionStringName = connSection[connSection.Count - 1].Name;
                    result = connSection[connSection.Count - 1].ConnectionString;
                    _providerName = connSection[connSection.Count - 1].ProviderName;
                }
            }
            else
            {
                try
                {
                    result = connSection[connectionStringName].ConnectionString;
                    _providerName = connSection[connectionStringName].ProviderName;
                }
                catch
                {
                    throw new Exception("There is no connection string name called '" + connectionStringName + "'");
                }
            }

            if (result.Contains("|DataDirectory|"))
            {
                //have to replace it
                string dataFilePath = _projectPath + "\\App_Data\\"; ;
                result = result.Replace("|DataDirectory|", dataFilePath);
            }


            if (result == "") throw new Exception("Sql2Poco: Please set up the Sql2PocoConnection in the config file.");
            return result;
        }
        private static ProjectItem GetItemByFilename(ProjectItems items, string filename)
        {
            foreach (ProjectItem item in items)
            {
                for (short i = 0; i < item.FileCount; i++)
                {
                    if (item.FileNames[i].Equals(filename))
                        return item;
                }
            }
            return null;
        }
        void buildAndWriteCode(string sql, ProjectItem projectItem, DocumentHelper documentHelper) {


            var pocoBuilder = new PocoBuilder(documentHelper, sql, (string m) => { LogToVSOutputWindow(m); });
            var connStr = GetConnectionInfo();

            var code = pocoBuilder.GenerateCode(connStr, _providerName);


            using (var stream = new StreamWriter(documentHelper.GeneratedClassFullFilename))
            {
                stream.Write(code);

            }


        }
        void myDocumentEvents_DocumentSaved(Document doc)
        {

            if (doc.FullName.EndsWith(".sql"))
                try
                {
                    var textDoc = ((TextDocument)doc.Object());
                    var start = textDoc.StartPoint;
                    var text = start.CreateEditPoint().GetText(textDoc.EndPoint);
                    if (text.Contains(SqlConst.TestSectionBeginDelimiter) && text.Contains(SqlConst.TestSectionEndDelimiter))
                    {
                        _projectPath = doc.ProjectItem.ContainingProject.Properties.Item("FullPath").Value.ToString();
                        var documentHelper = new DocumentHelper(doc.ProjectItem.Properties.Item("FullPath").Value.ToString(),
                               _projectPath,
                               doc.ProjectItem.ContainingProject.Properties.Item("DefaultNamespace").Value.ToString()
                               );

                        buildAndWriteCode(text, doc.ProjectItem, documentHelper);

                        //TODO file pull request on this fix (== null vs != null)
                        var genDoc = GetItemByFilename(doc.ProjectItem.Collection, documentHelper.GeneratedClassFullFilename);
                        if ( genDoc == null)
                        //doc.ProjectItem.Collection.AddFromFile(documentHelper.GeneratedClassFullFilename);
                        {
                            
                            ProjectItem parent = _dte.Solution.FindProjectItem(doc.FullName);
                            genDoc = parent?.ProjectItems.AddFromFile(documentHelper.GeneratedClassFullFilename);
                        }

                    }
                    else
                    {
                        LogToVSOutputWindow("Add the following to the Sql file to enable Sql2Poco support:" + Environment.NewLine +
                            SqlConst.ResultNamesDelimiter + Environment.NewLine +
                            SqlConst.TestSectionBeginDelimiter + Environment.NewLine +
                            SqlConst.TestSectionEndDelimiter + Environment.NewLine
                            );
                    }

                }
                catch (Exception ex)
                {
                    LogToVSOutputWindow(ex.Message + ex.StackTrace);
                }
        }

        public void LogToVSOutputWindow(string message)
        {
            Window window = _dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
            OutputWindow outputWindow = (OutputWindow)window.Object;
            OutputWindowPane outputWindowPane = null;

            for (uint i = 1; i <= outputWindow.OutputWindowPanes.Count; i++)
            {
                if (outputWindow.OutputWindowPanes.Item(i).Name.Equals("Sql2Poco", StringComparison.CurrentCultureIgnoreCase))
                {
                    outputWindowPane = outputWindow.OutputWindowPanes.Item(i);
                    break;
                }
            }

            if (outputWindowPane == null)
                outputWindowPane = outputWindow.OutputWindowPanes.Add("Sql2Poco");

            outputWindowPane.OutputString(message);
        }


    }
}