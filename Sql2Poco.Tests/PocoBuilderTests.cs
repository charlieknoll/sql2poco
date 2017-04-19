using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sql2Poco.Tests
{
    public class PocoBuilderTests
    {

        private static string providerName = "System.Data.SqlClient";
        //private static string connectionString = @"Data Source=.\SQLEXPRESS2012;Initial Catalog=corporate;Integrated Security=true";
        private static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        
        [Fact]
        public void ShouldBuildMultiPocoCode() {
            var helper = new DocumentHelper(@"c:\test\models\Test.sql", @"c:\test", "TestProject.Models");
            var builder = new PocoBuilder(helper, ExampleSql.TestMultiPocoSql,(m) => { Debug.WriteLine(m); });
            var code = builder.GenerateCode(connectionString, providerName);

        }
        [Fact]
        public void ShouldBuildSinglePocoCode()
        {
            var helper = new DocumentHelper(@"c:\test\models\Test.sql", @"c:\test", "TestProject.Models");
            var builder = new PocoBuilder(helper, ExampleSql.TestSinglePocoSql, (m) => { Debug.WriteLine(m); });
            var code = builder.GenerateCode(connectionString, providerName);
        }
        [Fact]
        public void ShouldBuildScalarCode()
        {
            var helper = new DocumentHelper(@"c:\test\models\Test.sql", @"c:\test", "TestProject.Models");
            var builder = new PocoBuilder(helper, ExampleSql.TestScalarSql, (m) => { Debug.WriteLine(m); });
            var code = builder.GenerateCode(connectionString, providerName);
        }
        [Fact]
        public void ShouldWriteInformationCodeOnSqlError()
        {
            var helper = new DocumentHelper(@"c:\test\models\Test.sql", @"c:\test", "TestProject.Models");
            var builder = new PocoBuilder(helper, ExampleSql.TestBrokenSql, (m) => { Debug.WriteLine(m); });
            var code = builder.GenerateCode(connectionString, providerName);
        }
    }
}
