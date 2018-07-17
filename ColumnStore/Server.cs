using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ColumnStore
{
    class Server
    {
        static Database db = null;

        static void Main(string[] args)
        {
            Run();
            Console.ReadLine();
        }

        static void Run()
        {
            //Create DB
            db = new Database();
            BuildEmployeeTable(db);

            RunTests();

        }

        static void RunTests()
        {
            //Query1();
            Query2();
        }

        static void Query1 ()
        {
            var query = new QueryParam();
            query.Dimensions.Add("ename");
            query.Dimensions.Add("city");

            var result = db.Query(query);
            Console.WriteLine(JsonConvert.SerializeObject(result));
        }

        static void Query2()
        {
            var query = new QueryParam();
            query.Dimensions.Add("ename");
            query.Dimensions.Add("city");
            query.Measures.Add("sum(salary)");

            var result = db.Query(query);
            Console.WriteLine(JsonConvert.SerializeObject(result));

        }

        static void BuildEmployeeTable(Database db)
        {
            
            var table = new Table("employees");
            db.Tables.Add("employees", table);
            table.Columns.Add("ename", new Column("ename"));
            table.Columns.Add("city", new Column("city"));
            table.Columns.Add("empid", new Column("empid"));
            table.Columns.Add("salary", new Column("salary"));

            for (int i = 0; i < 10; i++)
            {
                table.Columns["ename"].Values.Add("name " + i.ToString());
                table.Columns["city"].Values.Add((i % 2 == 0 ? "mumbai" : "chennai"));
                table.Columns["empid"].Values.Add(i.ToString());
                table.Columns["salary"].Values.Add(i+100);
            }

            table.Columns["ename"].Values.Add("name 0");
            table.Columns["city"].Values.Add("mumbai");
            table.Columns["empid"].Values.Add("0");
            table.Columns["salary"].Values.Add(1000);

            table.Columns["ename"].Values.Add("name 2");
            table.Columns["city"].Values.Add("mumbai");
            table.Columns["empid"].Values.Add("2");
            table.Columns["salary"].Values.Add(2000);

            table.RowCount = table.Columns["ename"].Values.Count();
            //return table;
        }
    }



    class Column
    {
        public Column(string name)
        {
            this.Name = name;
            this.Values = new List<object>();
        }
        public string Name { get; set; }
        public List<object> Values { get; set; }
    }

    class Table
    {
        public Table(string name)
        {
            this.Name = name;
            this.Columns = new Dictionary<string, Column>();
        }
        public string Name { get; set; }

        public Dictionary<string, Column> Columns { get; set; }

        public int RowCount { get; set; }

    }

    class Database
    {
        public Database()
        {
            this.Tables = new Dictionary<string, Table>();
        }
        public Dictionary<string,Table> Tables { get; set; }

        public QueryResult Query(QueryParam queryParam)
        {
            //Get tables involved.

            var queryResult = new QueryResult();
            var tableName = "employees";
            var table = this.Tables[tableName];
            var result1 = new Dictionary<string, List<object>>();

            if (queryParam.Measures.Count == 0)
            {
                foreach(var dim in queryParam.Dimensions)
                {
                    var col = table.Columns[dim];   
                    result1.Add(dim, col.Values);
                    //queryResult.ColumnNames.Add(dim);                                     
                    //queryResult.Values.Add(col.Values);                    
                }
            } else
            {
                //Measures exist
                
                var totalColCount = queryParam.Dimensions.Count + queryParam.Measures.Count; 
                for(int r=0; r< table.RowCount; r++)
                {
                    var list = new List<object>();
                    var hKey = string.Empty;
                    var tempList = new List<object>();
                    bool isAlreadyExist = false;
                    foreach (var dim in queryParam.Dimensions)
                    {
                        hKey += table.Columns[dim].Values[r];
                        tempList.Add(table.Columns[dim].Values[r]);
                    }

                    if (result1.ContainsKey(hKey))
                    {
                        list = result1[hKey];
                        isAlreadyExist = true;
                    } else
                    {
                        list = tempList;                        
                    }

                    var curColCount = queryParam.Dimensions.Count;
                    foreach (var m in queryParam.Measures)
                    {
                        curColCount++;
                        var mName = "salary";
                        var operation = "sum";

                        switch (operation)
                        {
                            case "count":
                                 if (list.Count > curColCount - 1)
                                {
                                    list[curColCount - 1] = Convert.ToInt32(list[curColCount - 1]) + 1;
                                }
                                else
                                {
                                    list.Add(1);
                                }

                                break;
                            case "sum":
                                if (list.Count > curColCount - 1)
                                {
                                    list[curColCount - 1] = Convert.ToDouble(list[curColCount - 1]) + Convert.ToDouble(table.Columns[mName].Values[r]);
                                }
                                else
                                {
                                    list.Add(table.Columns[mName].Values[r]);
                                }

                                break;
                        }
                       
                    }

                    if (!isAlreadyExist)
                    {
                        result1.Add(hKey, list);
                    }
                        
                }
              //  queryResult.Result = result1.Select(a => a.Value).ToArray();

               // int t = 0;
            }


            //queryResult.Result = queryResult.Values.Select(a => a.ToArray()).ToArray();

            queryResult.Result = result1;
            return queryResult;
        }
    }

    class QueryParam
    {
        public QueryParam()
        {
            this.Dimensions = new List<string>();
            this.Measures = new List<string>();
        }
        public List<string>  Dimensions { get; set; }

        public List<string> Measures { get; set; }
    }


    class QueryResult
    {
        public QueryResult()
        {
            this.Values = new List<List<string>>();
            this.ColumnNames = new List<string>();
        }

        public List<string> ColumnNames { get; set; }

        public List<List<string>> Values { get; set; }

        public Dictionary<string, List<object>> Result { get; set; }


    }
}
