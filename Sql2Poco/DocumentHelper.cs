using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using EnvDTE;

namespace Sql2Poco
{
    public class DocumentHelper
    {
        private readonly string _filePath;
        private readonly string _projectPath;
        private readonly string _defaultNamespace;
        public string BaseName => Path.GetFileNameWithoutExtension(_filePath);
        public string AppRoot => Path.GetDirectoryName(_projectPath) + "\\";
        public string PathFromAppRoot => Path.GetDirectoryName(_filePath)?.Substring(_projectPath.Length);
        public string FileNameSpace => _defaultNamespace + '.' + PathFromAppRoot.Replace('\\', '.');
        public string GeneratedClassFullFilename => AppRoot + PathFromAppRoot + '\\' + BaseName + ".sql2poco.cs";

        public DocumentHelper(string filePath, string projectPath, string defaultNamespace)
        {
            _filePath = filePath;
            _projectPath = projectPath;
            _defaultNamespace = defaultNamespace;


        }


    }
}