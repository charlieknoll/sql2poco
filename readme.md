# Sql2poco

Parses a sql script to generate POCOs for input parameters and 
for scalar, single or multiple resultsets.  Triggered on the 
save event of a script file.  See the Sql2Poco output pane for help.

##Usage

- Create a sql file and add it to your project
- Sql2Poco uses the last DSN connection in your config file (a la PetaPoco)
- add the following lines to your sql file:
````
--Sql2Poco-Result Names()
--DECLARE Variables here
--Sql2Poco Begin Set Test Params
--SET Test Variables here
--Sql2Poco End Set Test Params
--Write SQL here and return at least 1 result set
````
- Sql2Poco will use the sql file name appended with "Result" for the first result set or you can enter result names in the parens
- Sql2Poco will generate ResultClasses, Sql Properties and TestParmam classes which you can use in your sql implementation of choice, for example here's some test code for PetaPoco:
````
            var db = new Database("DefaultConnection");
            using (new Sql2PocoWrapperDb(db))
            {
                var result = db.Fetch<CalendarResult>(CalendarScript.Sql, CalendarScript.TestParamValues);
                Assert.True(result.Count > 0);
            }
````
Which uses the Sql2PocoWrapperDb which enforces that EnableNamedParams and EnableAutoSelect are false:

````
    public class Sql2PocoWrapperDb : IDisposable
    {
        readonly bool _enableNamedParams;
        readonly bool _enableAutoSelect;
        readonly IDatabase _db;
        public Sql2PocoWrapperDb(IDatabase db)
        {
            _enableNamedParams = db.EnableNamedParams;
            _enableAutoSelect = db.EnableAutoSelect;
            _db = db;
            db.EnableNamedParams = false;
            db.EnableAutoSelect = false;
        }

        public void Dispose()
        {
            _db.EnableNamedParams = _enableNamedParams;
            _db.EnableAutoSelect = _enableAutoSelect;
        }
    }
````

### Changes from the [QueryFirst](https://github.com/bbsimonbb/query-first) extension

- Better support for DI injection of connection
- vsix testability
- Multi results
- Params object for input model binding (partial)



