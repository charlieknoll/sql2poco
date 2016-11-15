using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sql2Poco.Tests
{
    public class SqlScriptTests
    {
        [Fact]
        public void ShouldParseParams() {
            var sqlScript = new SqlScript(ExampleSql.TestMultiPocoSql, "Test");
            Assert.Contains("select @test1", sqlScript.TestingText.ToLower());
        }
    }
}
