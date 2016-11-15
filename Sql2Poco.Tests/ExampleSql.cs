using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sql2Poco.Tests
{
    public static class ExampleSql
    {
        public static string TestMultiPocoSql = SqlConst.ResultNamesDelimiter + @"TestMaster,RecordCt,TestDetail)
            declare @test1 int; 
            declare @test2 varchar(2);  
            declare @test3 decimal(12,2)
            declare @test4 datetime  
        " + SqlConst.TestSectionBeginDelimiter + @"
            Select @test1 = 1
            select @test4 = getdate()
        " + SqlConst.TestSectionEndDelimiter + @"
             select @test1 as Id, @test2 Descr, @test3 ItemCost, @test4 LastModDate
             select @test1 as RecordCt
             select @test1 as Id, 5 as TagId
               Union
             select @test1, 10
        ";
        public static string TestSinglePocoSql = SqlConst.ResultNamesDelimiter + @"TestMaster,RecordCt,TestDetail)
            declare @test1 int; 
            declare @test2 varchar(2);  
            declare @test3 decimal(12,2)
            declare @test4 datetime  
        " + SqlConst.TestSectionBeginDelimiter + @"
            Select @test1 = 1
            select @test4 = getdate()
        " + SqlConst.TestSectionEndDelimiter + @"
             select @test1 as Id, @test2 Descr, @test3 ItemCost, @test4 LastModDate
        ";
        public static string TestScalarSql = SqlConst.ResultNamesDelimiter + @"TestMaster,RecordCt,TestDetail)
            declare @test1 int; 
            declare @test2 varchar(2);  
            declare @test3 decimal(12,2)
            declare @test4 datetime  
        " + SqlConst.TestSectionBeginDelimiter + @"
            Select @test1 = 1
            select @test4 = getdate()
        " + SqlConst.TestSectionEndDelimiter + @"
             select @test1 as RecordCt
        ";
        public static string TestBrokenSql = SqlConst.ResultNamesDelimiter + @"TestMaster,RecordCt,TestDetail)
            declare @test1 int; 
            declare @test2 varchar(2);  
            declare @test3 decimal(12,2)
            declare @test4 datetime  
        " + SqlConst.TestSectionBeginDelimiter + @"
            Select @test1 = 1
            select @test4 = getdate()
        " + SqlConst.TestSectionEndDelimiter + @"
             select splat as RecordCt
        ";
    }
}
