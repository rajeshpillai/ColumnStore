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
            BuildSkillsTable(db);

            RunTests();

        }

        static void RunTests()
        {
            //Test_dimensions_measures_multiple_filters();
            Test_simple_dimensions();
        }

        static void Test_simple_dimensions()
        {
            var query = new QueryParam();
            //query.Dimensions.Add("ename");
            //query.Dimensions.Add("city");

            query.Dimensions.Add(new Dimension() { Name = "ename", TableName="employees" });
            query.Dimensions.Add(new Dimension() { Name = "skill", TableName = "skills" });


            var result = db.Query(query);
            Console.WriteLine(JsonConvert.SerializeObject(result));
        }

        static void Test_dimensions_measures_multiple_filters()
        {
            var query = new QueryParam();
            query.Dimensions.Add(new Dimension() { Name = "ename", TableName = "employees" });
            query.Dimensions.Add(new Dimension() { Name = "city" , TableName="employees"});
            query.Measures.Add(new Measure() { Expression = "sum(salary)" });
            query.Measures.Add(new Measure() { Expression = "count(ename)" });

            query.Filters.Add(new Filter() { ColName = "ename", Values = new string[1] { "name 0" } });
            query.Filters.Add(new Filter() { ColName = "city", Values = new string[1] { "mumbai" } });
            //query.Filters.Add(new Filter() { ColName = "empid", Values = new string[1] { "0" } });


            var result = db.Query(query);
            Console.WriteLine(JsonConvert.SerializeObject(result));

        }

        static void Test_measures()
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
                table.Columns["salary"].Values.Add(i + 100);
            }

            table.Columns["ename"].Values.Add("name 0");
            table.Columns["city"].Values.Add("mumbai");
            table.Columns["empid"].Values.Add("20");
            table.Columns["salary"].Values.Add(1000);

            table.Columns["ename"].Values.Add("name 2");
            table.Columns["city"].Values.Add("mumbai");
            table.Columns["empid"].Values.Add("22");
            table.Columns["salary"].Values.Add(2000);

            table.RowCount = table.Columns["ename"].Values.Count();
            //return table;
        }

        static void BuildSkillsTable(Database db)
        {
            var table = new Table("skills");
            db.Tables.Add("skills", table);
            table.Columns.Add("empid", new Column("empid"));
            table.Columns.Add("skill", new Column("skill"));


            for (int i = 0; i < 10; i++)
            {
                table.Columns["empid"].Values.Add(i);
                table.Columns["skill"].Values.Add(i % 2 == 0 ? "nodejs" : ".net");

            }
            table.RowCount = table.Columns["empid"].Values.Count();
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
            //var tableName = "employees";
            //var table = this.Tables[tableName];
            //var result1 = new Dictionary<string, List<object>>();
            //int[] matchedIndexes = null;

            var tables =  GetTablesInvoled(queryParam);

            if (tables.Count == 1)
            {
                //Single table involved
                queryResult.Result = GetResultForSingleTable(queryParam, tables[0]);
                return queryResult;
            }
            else
            {
                //Multiple tables involved
                //int[] matchedIndexes = ApplyFilters(queryParam);
                var result1 = new Dictionary<string, List<object>>();
                int[] matchedIndexes = null;
                var preTableResult = new IntermidiateQueryResult();
                var curTableResult = new IntermidiateQueryResult();
                var tempQResult = new IntermidiateQueryResult();
                for (int t = 0; t < tables.Count(); t++)
                //foreach (var table in tables)
                {
                    //queryResult = new QueryResult();

                    var table = tables[t];
                    var nextTable = (t == tables.Count() - 1) ? tables[t - 1] : tables[t + 1];
                    var keyFieldList = GetKeyFields(table, nextTable);
                    //table.key


                    var dims = queryParam.Dimensions.Where(d => d.TableName == table.Name).ToList();

                    var keyDims = keyFieldList.Where(k => dims.Select(d => d.Name).ToList().Contains(k) != true).ToList();
                    foreach (var kdName in keyDims)
                    {
                        dims.Add(new Dimension() { Name = kdName, TableName = table.Name });
                    }

                    queryResult.KeyFields.Add(table.Name, keyFieldList[0]);
                    if (t == 0)
                    {
                        foreach (var dim in dims)
                        {
                            //curTableResult.ColumnNames.Add(dim.Name);
                            curTableResult.Values.Add(dim.Name,table.Columns[dim.Name].Values);
                        }
                    }
                    else
                    {
                        if (t > 1)
                        {
                            preTableResult = tempQResult;
                        } else
                        {
                            preTableResult = curTableResult;
                        }                        
                        curTableResult = new IntermidiateQueryResult();

                        //t != 0                      
                        var prevTable = tables[t - 1];
                        var keyField = GetKeyFields(table, nextTable)[0];
                        //var keyIndex =   queryResult.ColumnNames.FindIndexes<object>(v => v.ToString() == keyField).ToArray()[0];

                        //var tempQResult = new QueryResult();
                        foreach (var dim in dims)
                        {
                            //tempQResult.ColumnNames.Add(dim.Name);
                            curTableResult.Values.Add(dim.Name,table.Columns[dim.Name].Values);
                        }

                        tempQResult = new IntermidiateQueryResult();
                        foreach (var key in preTableResult.Values.Keys)
                        {
                            if (!tempQResult.Values.ContainsKey(key))
                            {
                                tempQResult.Values.Add(key, new List<object>());
                            }
                        }
                        foreach (var key in curTableResult.Values.Keys)
                        {
                            if (!tempQResult.Values.ContainsKey(key))
                            {
                                tempQResult.Values.Add(key, new List<object>());
                            }
                        }
                        //Loop Key fields Data e.g. empId
                        var preTableKeyColData = preTableResult.Values[keyField];

                        //var firstTableREsult, secondTableResult

                        for (int prevTblRowIndex =0; prevTblRowIndex< preTableKeyColData.Count(); prevTblRowIndex++)
                        {
                           var indexArray = curTableResult.Values[keyField].FindIndexes<object>(v => v.ToString() == preTableKeyColData[prevTblRowIndex].ToString()).ToArray();
                            if (null != indexArray && indexArray.Length > 0)
                            {
                                foreach (var key in tempQResult.Values.Keys)
                                {           
                                    ////If no matching found then we need that data in final result as outer join
                                    //if(indexArray.Length == 0)
                                    //{
                                    //    if (curTableResult.Values[key].Contains(preTableKeyColData[prevTblRowIndex]))
                                    //    {
                                    //        tempQResult.Values[key].Add(curTableResult.Values[key][ia].ToString());
                                    //    } else
                                    //    {
                                    //        tempQResult.Values[key].Add(preTableResult.Values[key][prevTblRowIndex]);
                                    //    }
                                    //        continue;
                                    //}
                                    foreach(int ia in indexArray)
                                    {
                                        if (curTableResult.Values.ContainsKey(key))
                                        {                                            
                                            tempQResult.Values[key].Add(curTableResult.Values[key][ia].ToString());
                                        }
                                        if (preTableResult.Values.ContainsKey(key))
                                        {
                                            if (key != keyField)
                                            {
                                                tempQResult.Values[key].Add(preTableResult.Values[key][prevTblRowIndex]);
                                            }
                                               
                                        }

                                    }
                                   
                                      
                                }
                                
                            }
                        }
                        var test = tempQResult;

                    }
                    queryResult.Result = result1;
                }
            }

           

            //queryResult.Result = queryResult.Values.Select(a => a.ToArray()).ToArray();
            //queryResult.Result = result1;
            return queryResult;
        }

        private string[] GetKeyFields(Table table1, Table table2)
        {
            string[] fields = new string[1] { "empid" };

            return fields;
        }

    
        private Dictionary<string, List<object>> GetResultForSingleTable(QueryParam queryParam, Table table)
        {
            var result1 = new Dictionary<string, List<object>>();
            int[] matchedIndexes = ApplyFilters(queryParam);

            if (queryParam.Measures.Count == 0)
            {
                foreach (var dim in queryParam.Dimensions)
                {
                    var col = table.Columns[dim.Name];
                    result1.Add(dim.Name, col.Values);
                    //queryResult.ColumnNames.Add(dim);                                     
                    //queryResult.Values.Add(col.Values);                    
                }
            }
            else
            {
                //Measures exist

                //var totalColCount = queryParam.Dimensions.Count + queryParam.Measures.Count;
                int startIndex = 0, endIndex = table.RowCount;
                bool isMatchedIndexExist = (null != matchedIndexes && matchedIndexes.Length > 0);

                if (isMatchedIndexExist)
                {
                    endIndex = matchedIndexes.Length;
                }
                for (int i = startIndex; i < endIndex; i++)
                {
                    int r = isMatchedIndexExist ? matchedIndexes[i] : i;
                    var list = new List<object>();
                    var hKey = string.Empty;
                    var tempList = new List<object>();
                    bool isAlreadyExist = false;
                    foreach (var dim in queryParam.Dimensions)
                    {
                        hKey += table.Columns[dim.Name].Values[r];
                        tempList.Add(table.Columns[dim.Name].Values[r]);
                    }

                    if (result1.ContainsKey(hKey))
                    {
                        list = result1[hKey];
                        isAlreadyExist = true;
                    }
                    else
                    {
                        list = tempList;
                    }

                    var curColCount = queryParam.Dimensions.Count;
                    foreach (var measure in queryParam.Measures)
                    {
                        curColCount++;

                        //var operation = "sum";

                        var curMeasureDtlList = Utility.GetMeasureDetails(measure);
                        foreach (var mDetails in curMeasureDtlList)
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

            return result1;
        }


        private int[] ApplyFilters(QueryParam queryParam)
        {
            int[] matchedIndexes = null;
            //if filter Exist
            if (queryParam.Filters.Count > 0)
            {
                queryParam.Filters[0].TableName = "employees"; //Todo : Remove hardcoded
                var table = this.Tables[queryParam.Filters[0].TableName];

                var filterCol = table.Columns[queryParam.Filters[0].ColName];
                matchedIndexes = filterCol.Values.FindIndexes<object>(v => queryParam.Filters[0].Values.Contains(v) == true).ToArray();
                //filter Exist
                for (int f = 1; f < queryParam.Filters.Count; f++)// (var filter  in queryParam.Filters)
                {
                    var finalMatchedIndexes = new List<int>();
                    var filter = queryParam.Filters[f];
                    filterCol = table.Columns[filter.ColName];

                    for (int m = 0; m < matchedIndexes.Length; m++)
                    {
                        var mIndex = matchedIndexes[m];
                        if (filter.Values.Contains(filterCol.Values[mIndex]))
                        {
                            finalMatchedIndexes.Add(mIndex);
                        }
                    }

                    matchedIndexes = finalMatchedIndexes.ToArray();
                    //matchedIndexes = filterCol.Values.FindIndexes<object>(v => filter.Values.Contains(v) == true).ToArray();   

                }
            }

            return matchedIndexes;
        }


        private List<Table> GetTablesInvoled(QueryParam queryParam)
        {
            var tables = new List<string>();

            tables =  queryParam.Dimensions.Select(d => d.TableName).Distinct().ToList();

            tables.AddRange(queryParam.Measures.Select(d => d.TableName).Distinct().Where(m => !tables.Contains(m)).ToList());

            tables.AddRange(queryParam.Filters.Select(d => d.TableName).Distinct().Where(m => !tables.Contains(m)).ToList());

            List<Table> tableList = new List<Table>();
            foreach ( var t in tables)
            {
                tableList.Add(this.Tables[t]);
            }
            return tableList;
        }
    }

    static class Utility
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

        public static IEnumerable<int> FindIndexes<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            int index = 0;
            foreach (T item in items)
            {
                if (predicate(item))
                {
                    yield return index;
                }

                index++;
            }
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
            this.Dimensions = new List<Dimension>();
            this.Measures = new List<Measure>();
            this.Filters = new List<Filter>();
        }
        public List<Dimension>  Dimensions { get; set; }

        public List<Measure> Measures { get; set; }

        public List<Filter> Filters { get; set; }
    }


    class QueryResult
    {
        public QueryResult()
        {
            this.Values = new List<List<object>>();
            this.ColumnNames = new List<string>();
            this.KeyFields = new Dictionary<string, string>();
        }

        public List<string> ColumnNames { get; set; }

        public List<List<object>> Values { get; set; }

        public Dictionary<string,string> KeyFields { get; set; }

        public Dictionary<string, List<object>> Result { get; set; }


    }

    class IntermidiateQueryResult
    {
        public IntermidiateQueryResult()
        {
            this.Values = new Dictionary<string, List<object>>();
            //this.ColumnNames = new List<string>();
            this.KeyFields = new Dictionary<string, string>();
        }

        //public List<string> ColumnNames { get; set; }

        public Dictionary<string, List<object>> Values { get; set; }

        public Dictionary<string, string> KeyFields { get; set; }

        //public Dictionary<string, List<object>> Result { get; set; }


    }

    public class Dimension
    {
        private string _dimType = "Simple";

        public string Name { get; set; }

        public string Type
        {
            get
            {
                return _dimType;
            }
            set
            {
                _dimType = value;
            }
        }       

        public string TableName { get; set; }
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

        public string TableName { get; set; }
    }

    public class Filter
    {
        private string _operType = "in";

        public string ColName { get; set; }

        public string[] Values { get; set; }

        public string OperationType
        {
            get
            {
                return _operType;
            }
            set
            {
                _operType = value;
            }
        }

        public string TableName { get; set; }
    }

}
