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
            //Query3();
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
            query.Measures.Add(new Measure() {Expression = "sum(salary)" });
            query.Measures.Add(new Measure() { Expression = "count(ename)" });

            var result = db.Query(query);
            Console.WriteLine(JsonConvert.SerializeObject(result));

        }

        static void Query3()
        {
            var query = new QueryParam();          
            query.Measures.Add(new Measure() { Expression = "sum(salary)" });

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
                    foreach (var measure in queryParam.Measures)
                    {
                        curColCount++;
                       
                        //var operation = "sum";

                        var curMeasureDtlList = Utility.GetMeasureDetails(measure);
                        foreach(var mDetails in curMeasureDtlList)
                        {
                            var mName = mDetails.MName;
                            switch (mDetails.Operation)
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

    class Utility
    {
        public static List<MeasureDetails> GetMeasureDetails(Measure measure)
        {
            var mDetailList = new List<MeasureDetails>();
            char[] delimiters = new char[] { '/', '+', '-', '*', ')', '(' };
            var formula = measure.Expression;
            var formulaParts = formula.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

         
            var measureDetails = new MeasureDetails();
            measureDetails.MName = measure.Expression.Replace("sum(", "").Replace(")", "").Trim();
            if (measure.Expression.IndexOf("sum(") != -1)
            {
                measureDetails.Operation = "sum";
            }
            else if (measure.Expression.IndexOf("count(") != -1)
            {
                measureDetails.Operation = "count";
            }
            mDetailList.Add(measureDetails);
           

            //for (int i = 0; i < formulaParts.Length; i++)
            //{
            //    var measureDetails = new MeasureDetails();
            //    measureDetails.MName = formulaParts[i].Replace("sum(", "").Replace(")", "").Trim();
            //    if (formulaParts[i].IndexOf("sum(") != -1)
            //    {                    
            //        measureDetails.Operation = "sum";
            //    } else if (formulaParts[i].IndexOf("count(") != -1)
            //    {
            //        measureDetails.Operation = "count";
            //    }
            //    mDetailList.Add(measureDetails);
            //}
            return mDetailList;
        }
    }

    class MeasureDetails
    {
        public string MName { get; set; }
        public string Operation { get; set; }
    }

    class QueryParam
    {
        public QueryParam()
        {
            this.Dimensions = new List<string>();
            this.Measures = new List<Measure>();
        }
        public List<string>  Dimensions { get; set; }

        public List<Measure> Measures { get; set; }
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

    public class Measure
    {
        //private bool _isVisible = true;

        private bool _isExpression = true;

        public string Expression { get; set; }
        public string DisplayName { get; set; }

        public bool ShowTotal { get; set; }

        public bool IsExpression
        {
            get
            {
                return _isExpression;
            }
            set
            {
                _isExpression = value;
            }
        }

        public string Type { get; set; }
    }
}
