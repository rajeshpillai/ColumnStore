﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

//namespace ColumnStore
//{
//    class Server_old
//    {
//        static Database db = null;

//        //static void Main(string[] args)
//        //{
//        //    Run();
//        //    Console.ReadLine();
//        //}

//        //static void Run()
//        //{
//        //    //Create DB
//        //    db = new Database();
//        //    BuildEmployeeTable(db);
//        //    BuildSkillsTable(db);
//        //    BuildIndustryTable(db);

//        //    RunTests();

//        //}

//        static void RunTests()
//        {
//            //Test_dimensions_measures_multiple_filters();
//            Test_simple_dimensions();
//        }

//        static void Test_simple_dimensions()
//        {
//            var query = new QueryParam();
//            //query.Dimensions.Add("ename");
//            //query.Dimensions.Add("city");

//            query.Dimensions.Add(new Dimension() { Name = "ename", TableName="employees" });
//            query.Dimensions.Add(new Dimension() { Name = "city", TableName = "employees" });
//            query.Dimensions.Add(new Dimension() { Name = "skill", TableName = "skills" });
//            query.Dimensions.Add(new Dimension() { Name = "industryname", TableName = "industry" });

//            //query.Filters.Add(new Filter() { ColName = "ename", Values = new string[1] { "name 0" } , TableName = "employees" });
//            query.Filters.Add(new Filter() { ColName = "city", Values = new string[1] { "mumbai" }, TableName = "employees" });
//            //query.Filters.Add(new Filter() { ColName = "skill", Values = new string[1] { "nodejs" }, TableName = "skills" });
//            //query.Filters.Add(new Filter() { ColName = "industryname", Values = new string[1] { "ibm" }, TableName = "industry" });

//            var result = db.Query(query);
//            Console.WriteLine(JsonConvert.SerializeObject(result));
//        }

//        static void Test_dimensions_measures_multiple_filters()
//        {
//            var query = new QueryParam();
//            query.Dimensions.Add(new Dimension() { Name = "ename", TableName = "employees" });
//            query.Dimensions.Add(new Dimension() { Name = "city" , TableName="employees"});
//            query.Measures.Add(new Measure() { Expression = "sum(salary)" });
//            query.Measures.Add(new Measure() { Expression = "count(ename)" });

//            query.Filters.Add(new Filter() { ColName = "ename", Values = new string[1] { "name 0" } });
//            query.Filters.Add(new Filter() { ColName = "city", Values = new string[1] { "mumbai" } });
//            //query.Filters.Add(new Filter() { ColName = "empid", Values = new string[1] { "0" } });


//            var result = db.Query(query);
//            Console.WriteLine(JsonConvert.SerializeObject(result));

//        }

//        static void Test_measures()
//        {
//            var query = new QueryParam();
//            query.Measures.Add(new Measure() { Expression = "sum(salary)" });

//            var result = db.Query(query);
//            Console.WriteLine(JsonConvert.SerializeObject(result));

//        }

//        static void BuildEmployeeTable(Database db)
//        {

//            var table = new Table("employees");
//            db.Tables.Add("employees", table);
//            table.Columns.Add("ename", new Column("ename"));
//            table.Columns.Add("city", new Column("city"));
//            table.Columns.Add("empid", new Column("empid"));
//            table.Columns.Add("salary", new Column("salary"));

//            for (int i = 0; i < 100; i++)
//            {
//                table.Columns["ename"].Values.Add("name " + i.ToString());
//                table.Columns["city"].Values.Add((i % 2 == 0 ? "mumbai" : "chennai"));
//                table.Columns["empid"].Values.Add(i.ToString());
//                table.Columns["salary"].Values.Add(i + 100);
//            }

//            //table.Columns["ename"].Values.Add("name 0");
//            //table.Columns["city"].Values.Add("delhi");
//            //table.Columns["empid"].Values.Add("0");
//            //table.Columns["salary"].Values.Add(1000);

//            //table.Columns["ename"].Values.Add("name 22");
//            //table.Columns["city"].Values.Add("mumbai");
//            //table.Columns["empid"].Values.Add("22");
//            //table.Columns["salary"].Values.Add(2000);

//            table.RowCount = table.Columns["ename"].Values.Count();
//            //return table;
//        }

//        static void BuildSkillsTable(Database db)
//        {
//            var table = new Table("skills");
//            db.Tables.Add("skills", table);
//            table.Columns.Add("empid", new Column("empid"));
//            table.Columns.Add("skill", new Column("skill"));
//            table.Columns.Add("industryid", new Column("industryid"));


//            for (int i = 0; i < 100; i++)
//            {
//                table.Columns["empid"].Values.Add(i);
//                table.Columns["skill"].Values.Add(i % 2 == 0 ? "nodejs" : ".net");
//                table.Columns["industryid"].Values.Add("i" +i.ToString());
//            }

//            //table.Columns["empid"].Values.Add("22");
//            //table.Columns["skill"].Values.Add("expressjs");
//            //table.Columns["industryid"].Values.Add("i22");

//            table.RowCount = table.Columns["empid"].Values.Count();
//            //return table;        
//        }

//        static void BuildIndustryTable(Database db)
//        {
//            var table = new Table("industry");
//            db.Tables.Add("industry", table);
//            table.Columns.Add("industryid", new Column("industryid"));
//            table.Columns.Add("industryname", new Column("industryname"));


//            for (int i = 0; i < 25; i++)
//            {
//                table.Columns["industryid"].Values.Add("i" + i.ToString());
//                table.Columns["industryname"].Values.Add(i % 2 == 0 ? "microsoft" : "ibm");

//            }

//            //table.Columns["industryid"].Values.Add("i22");
//            //table.Columns["industryname"].Values.Add("tcs");

//            table.RowCount = table.Columns["industryid"].Values.Count();
//            //return table;        
//        }

//    }

//    class Column
//    {
//        public Column(string name)
//        {
//            this.Name = name;
//            this.Values = new List<object>();
//        }
//        public string Name { get; set; }
//        public List<object> Values { get; set; }
//    }

//    class Table
//    {
//        public Table(string name)
//        {
//            this.Name = name;
//            this.Columns = new Dictionary<string, Column>();
//        }
//        public string Name { get; set; }

//        public Dictionary<string, Column> Columns { get; set; }

//        public int RowCount { get; set; }


//    }

//    class Database
//    {
//        public Database()
//        {
//            this.Tables = new Dictionary<string, Table>();
//        }
//        public Dictionary<string,Table> Tables { get; set; }

//        public QueryResult Query(QueryParam queryParam)
//        {
//            //Get tables involved.

//            var queryResult = new QueryResult();
//            //var tableName = "employees";
//            //var table = this.Tables[tableName];
//            //var result1 = new Dictionary<string, List<object>>();
//            //int[] matchedIndexes = null;

//            var tables =  GetTablesInvoled(queryParam);

//            if (tables.Count == 1)
//            {
//                //Single table involved
//                queryResult.Result = GetResultForSingleTable(queryParam, tables[0]);
//                return queryResult;
//            }
//            else
//            {
//                //Multiple tables involved
//                //int[] matchedIndexes = ApplyFilters(queryParam);
//                //var result1 = new Dictionary<string, List<object>>();
//                int[] matchedIndexes = null;
//                var prevTableResult = new IntermidiateQueryResult();
//                var curTableResult = new IntermidiateQueryResult();
//                var tempQResult = new IntermidiateQueryResult();
//                for (int t = 0; t < tables.Count(); t++)
//                //foreach (var table in tables)
//                {
//                    //queryResult = new QueryResult();

//                    var table = tables[t];
//                    var nextTable = (t == tables.Count() - 1) ? tables[t - 1] : tables[t + 1];
//                    var keyFieldList = GetKeyFields(table, nextTable);
//                    //table.key


//                    var dims = queryParam.Dimensions.Where(d => d.TableName == table.Name).ToList();

//                    var keyDims = keyFieldList.Where(k => dims.Select(d => d.Name).ToList().Contains(k) != true).ToList();
//                    foreach (var kdName in keyDims)
//                    {
//                        dims.Add(new Dimension() { Name = kdName, TableName = table.Name });
//                    }

//                    queryResult.KeyFields.Add(table.Name, keyFieldList[0]);

//                    ////If filter exists get matched indexes.
//                    //if(queryParam.Filters.Count> 0)
//                    //{
//                    //    matchedIndexes = ApplyFiltersForMultipleJoins(queryParam, curTableResult);
//                    //}

//                    int startIndex = 0, endIndex = 0;
//                    bool isMatchedIndexExist = false ;

//                    if (t == 0)
//                    {
//                        //foreach (var dim in dims)
//                        //{
//                        //    //curTableResult.ColumnNames.Add(dim.Name);
//                        //    curTableResult.Values.Add(dim.Name, table.Columns[dim.Name].Values);
//                        //}
//                        //If filter exists get matched indexes.
//                        if (queryParam.Filters.Count > 0)
//                        {
//                            matchedIndexes = ApplyFiltersForMultipleJoins(queryParam, null, table);
//                        }
//                        startIndex = 0; endIndex = table.RowCount;
//                        isMatchedIndexExist = (null != matchedIndexes && matchedIndexes.Length > 0);
//                        if (isMatchedIndexExist)
//                        {
//                            endIndex = matchedIndexes.Length;
//                        }



//                        foreach (var dim in dims)
//                        {
//                            var data = new List<object>();
//                            for (int i = startIndex; i < endIndex; i++)
//                            {
//                                int r = isMatchedIndexExist ? matchedIndexes[i] : i;
//                                data.Add(table.Columns[dim.Name].Values[r]);
//                            }
//                            curTableResult.Values.Add(dim.Name, data);
//                        }
//                    }
//                    else
//                    {
//                        if (t > 1)
//                        {
//                            prevTableResult = tempQResult;
//                        }
//                        else
//                        {
//                            prevTableResult = curTableResult;
//                        }
//                        curTableResult = new IntermidiateQueryResult();

//                        //t != 0                      
//                        var prevTable = tables[t - 1];
//                        var keyField = GetKeyFields(table, prevTable)[0];

//                        if (null == dims.Where(d => d.Name == keyField).FirstOrDefault())
//                        {
//                            dims.Add(new Dimension() { Name = keyField, TableName = table.Name });
//                        }
//                        //var keyIndex =   queryResult.ColumnNames.FindIndexes<object>(v => v.ToString() == keyField).ToArray()[0];

//                        //var tempQResult = new QueryResult();
//                        foreach (var dim in dims)
//                        {
//                            //tempQResult.ColumnNames.Add(dim.Name);
//                            curTableResult.Values.Add(dim.Name, table.Columns[dim.Name].Values);
//                        }

//                        tempQResult = new IntermidiateQueryResult();
//                        foreach (var key in prevTableResult.Values.Keys)
//                        {
//                            if (!tempQResult.Values.ContainsKey(key))
//                            {
//                                tempQResult.Values.Add(key, new List<object>());
//                            }
//                        }
//                        foreach (var key in curTableResult.Values.Keys)
//                        {
//                            if (!tempQResult.Values.ContainsKey(key))
//                            {
//                                tempQResult.Values.Add(key, new List<object>());
//                            }
//                        }
//                        //Loop Key fields Data e.g. empId
//                        var preTableKeyColData = prevTableResult.Values[keyField];

//                        //IntermidiateQueryResult firstTableResult = null;
//                        //IntermidiateQueryResult secondTableResult = null;

//                        if (queryParam.Filters.Count() > 0)
//                        {
//                            //check  if curTableResult has filter column or not
//                            if (null != queryParam.Filters.Where(qf => curTableResult.Values.ContainsKey(qf.ColName)  && curTableResult.Values.ContainsKey(qf.ColName)).FirstOrDefault())
//                            {
//                                GetQueryJoinResult(curTableResult, prevTableResult, keyField, tempQResult, queryParam);
//                            } else if (null != queryParam.Filters.Where(qf => prevTableResult.Values.ContainsKey(qf.ColName) && prevTableResult.Values.ContainsKey(qf.ColName) ).FirstOrDefault())
//                            {
//                                GetQueryJoinResult(prevTableResult,curTableResult, keyField, tempQResult, queryParam);
//                            } else
//                            {
//                                GetQueryJoinResult(curTableResult, prevTableResult, keyField, tempQResult, queryParam);
//                            }
//                        } else
//                        {
//                            GetQueryJoinResult(curTableResult, prevTableResult, keyField, tempQResult, queryParam);
//                        }

                     



//                        if (queryParam.Filters.Count == 0) { 
//                            GetQueryJoinResult(prevTableResult, curTableResult, keyField, tempQResult,queryParam, false);
//                        }

//                        //aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
//                        var test = tempQResult;
//                        queryResult.Result = tempQResult.Values;

//                    }
//                    //queryResult.Result = result1;
//                }
//            }

           

//            //queryResult.Result = queryResult.Values.Select(a => a.ToArray()).ToArray();
//            //queryResult.Result = result1;
//            return queryResult;
//        }

//        private void GetQueryJoinResult(IntermidiateQueryResult firstTableResult, IntermidiateQueryResult secondTableResult, string keyField, IntermidiateQueryResult tempQResult, QueryParam queryParam, bool isFirtsTime = true)
//        {
//            try
//            {
//                //if (preTableResult.Values[keyField].Count() > curTableResult.Values[keyField].Count())
//                //{
//                //    firstTableResult = preTableResult;
//                //    secondTableResult = curTableResult;
//                //}
//                //else
//                //{
//                //    firstTableResult = curTableResult;
//                //    secondTableResult = preTableResult;
//                //}
//                int[] matchedIndexes = null;
//                if (queryParam.Filters.Count > 0)
//                {
//                    matchedIndexes = ApplyFiltersForMultipleJoins(queryParam, firstTableResult, null);
//                }

//                int startIndex = 0, endIndex = firstTableResult.Values[keyField].Count();
//                bool isMatchedIndexExist = (null != matchedIndexes && matchedIndexes.Length > 0);
//                if (isMatchedIndexExist)
//                {
//                    endIndex = matchedIndexes.Length;
//                }
              
//                //var data = new List<object>();
//                for (int i = startIndex; i < endIndex; i++)
//                {
//                    int firstTblRowIndex = isMatchedIndexExist ? matchedIndexes[i] : i;
//                    //data.Add(table.Columns[dim.Name].Values[r]);


//                    var firstTableKeyColData = firstTableResult.Values[keyField];
//                    //for (int firstTblRowIndex = 0; firstTblRowIndex < firstTableKeyColData.Count(); firstTblRowIndex++)
//                    //{
//                    var indexArray = secondTableResult.Values[keyField].FindIndexes<object>(v => v.ToString() == firstTableKeyColData[firstTblRowIndex].ToString()).ToArray();

//                    //If no matching found then we need that data in final result as outer join
//                    if (indexArray.Length == 0 && queryParam.Filters.Count() == 0)
//                    {
//                        if (secondTableResult.Values[keyField].Contains(firstTableKeyColData[firstTblRowIndex]))
//                        {
//                            //Second table has key
//                            tempQResult.Values[keyField].Add(secondTableResult.Values[keyField][secondTableResult.Values[keyField].IndexOf(firstTableKeyColData[firstTblRowIndex])].ToString());
//                            foreach (var key in tempQResult.Values.Keys)
//                            {
//                                if (secondTableResult.Values.ContainsKey(key))
//                                {
//                                    if (key != keyField)
//                                    {
//                                        tempQResult.Values[key].Add(secondTableResult.Values[key][secondTableResult.Values[keyField].IndexOf(firstTableKeyColData[firstTblRowIndex])].ToString());
//                                    }
//                                } else if (firstTableResult.Values.ContainsKey(key))
//                                {
//                                    if (key != keyField)
//                                    {
//                                        tempQResult.Values[key].Add("");
//                                    }

//                                }
//                            }
//                        }
//                        else
//                        {
//                            //First table has key
//                            tempQResult.Values[keyField].Add(firstTableResult.Values[keyField][firstTblRowIndex]);
//                            foreach (var key in tempQResult.Values.Keys)
//                            {
//                                if (secondTableResult.Values.ContainsKey(key))
//                                {
//                                    if (key != keyField)
//                                    {
//                                        tempQResult.Values[key].Add("");
//                                    }
//                                } else if (firstTableResult.Values.ContainsKey(key))
//                                {
//                                    if (key != keyField)
//                                    {
//                                        tempQResult.Values[key].Add(firstTableResult.Values[key][firstTableResult.Values[keyField].IndexOf(firstTableKeyColData[firstTblRowIndex])].ToString());
//                                    }
//                                }
//                            }
//                        }


//                    }
//                    else if (null != indexArray && indexArray.Length > 0 && isFirtsTime)
//                    {
//                        foreach (var key in tempQResult.Values.Keys)
//                        {
//                            foreach (int ia in indexArray)
//                            {
//                                if (secondTableResult.Values.ContainsKey(key))
//                                {
//                                    tempQResult.Values[key].Add(secondTableResult.Values[key][ia].ToString());
//                                } else if (firstTableResult.Values.ContainsKey(key))
//                                {
//                                    if (key != keyField)
//                                    {
//                                        tempQResult.Values[key].Add(firstTableResult.Values[key][firstTblRowIndex]);
//                                    }

//                                }

//                            }


//                        }

//                    }
//                }
//            }
//            catch(Exception ex)
//            {
//                throw ex;
//            }
//        }

//        private string[] GetKeyFields(Table table1, Table table2)
//        {
//            string[] fields = new string[1] { "empid" };

//            if((table1.Name == "employees"  && table2.Name  == "skills") || (table1.Name == "skills" && table2.Name == "employees"))
//            {
//                fields = new string[1] { "empid" };
//            } else
//            {
//                fields = new string[1] { "industryid" };
//            }

//            return fields;
//        }

//        private int[] ApplyFiltersForMultipleJoins(QueryParam queryParam, IntermidiateQueryResult curTableResult, Table curTable)
//        {
//            int[] matchedIndexes = null;
//            //if filter Exist
//            if (queryParam.Filters.Count > 0)
//            {
//                List<Filter> curTableFilters = new List<Filter>();
//                List<object> filterColValues = new List<object>();

//                if (null != curTable)
//                {
//                    curTableFilters = queryParam.Filters.Where(f => curTable.Columns.ContainsKey(f.ColName)).ToList();                   
//                }
//                else
//                {
//                    curTableFilters = queryParam.Filters.Where(f => curTableResult.Values.Keys.Contains(f.ColName)).ToList();                    
//                }

//                //queryParam.Filters[0].TableName = "employees"; //Todo : Remove hardcoded
//                //var table = curTableResult;
//                //if (null != curTable)
//                //{
//                //    curTableFilters = queryParam.Filters.Where(f => curTable.Columns.ContainsKey(f.ColName)).ToList(); 
//                //    if(curTableFilters.Count > 0)
//                //    {
//                //        filterColValues = curTable.Columns[curTableFilters[0].ColName].Values;
//                //        matchedIndexes = filterColValues.FindIndexes<object>(v => curTableFilters[0].Values.Contains(v) == true).ToArray();
//                //    }

//                //} else
//                //{
//                //    curTableFilters = queryParam.Filters.Where(f => curTableResult.Values.Keys.Contains(f.ColName)).ToList();                    
//                //    if (curTableFilters.Count > 0)
//                //    {
//                //        filterColValues = curTableResult.Values[curTableFilters[0].ColName];
//                //        matchedIndexes = filterColValues.FindIndexes<object>(v => curTableFilters[0].Values.Contains(v) == true).ToArray();
//                //    }
//                //}


//                //filter Exist
//                for (int f =0; f < curTableFilters.Count; f++)// (var filter  in queryParam.Filters)
//                {
//                    var finalMatchedIndexes = new List<int>();
//                    var filter = curTableFilters[f];
//                    //filterCol = table.Columns[filter.ColName];
//                    var prevMatchedIndexes = matchedIndexes;

//                    if (null != curTable)
//                    {
//                        curTableFilters = queryParam.Filters.Where(fl => curTable.Columns.ContainsKey(fl.ColName)).ToList();
//                        if (curTableFilters.Count > 0)
//                        {
//                            filterColValues = curTable.Columns[filter.ColName].Values;
//                            matchedIndexes = filterColValues.FindIndexes<object>(v => filter.Values.Contains(v) == true).ToArray();
//                        }
//                    }
//                    else
//                    {
//                        curTableFilters = queryParam.Filters.Where(fl => curTableResult.Values.Keys.Contains(fl.ColName)).ToList();
//                        if (curTableFilters.Count > 0)
//                        {
//                            filterColValues = curTableResult.Values[filter.ColName];
//                            matchedIndexes = filterColValues.FindIndexes<object>(v => filter.Values.Contains(v) == true).ToArray();
//                        }
//                    }

//                    if(null != prevMatchedIndexes)
//                    {
//                        matchedIndexes = matchedIndexes.Intersect(prevMatchedIndexes).ToArray();
//                    }

//                    for (int m = 0; m < matchedIndexes.Length; m++)
//                    {
//                        var mIndex = matchedIndexes[m];
//                        if (filter.Values.Contains(filterColValues[mIndex]))
//                        {
//                            finalMatchedIndexes.Add(mIndex);
//                        }
//                    }

//                    matchedIndexes = finalMatchedIndexes.ToArray();
//                    //matchedIndexes = filterCol.Values.FindIndexes<object>(v => filter.Values.Contains(v) == true).ToArray();   

//                }
//            }

//            return matchedIndexes;
//        }


//        private Dictionary<string, List<object>> GetResultForSingleTable(QueryParam queryParam, Table table)
//        {
//            var result1 = new Dictionary<string, List<object>>();
//            int[] matchedIndexes = ApplyFilters(queryParam);

//            if (queryParam.Measures.Count == 0)
//            {
//                foreach (var dim in queryParam.Dimensions)
//                {
//                    var col = table.Columns[dim.Name];
//                    result1.Add(dim.Name, col.Values);
//                    //queryResult.ColumnNames.Add(dim);                                     
//                    //queryResult.Values.Add(col.Values);                    
//                }
//            }
//            else
//            {
//                //Measures exist

//                //var totalColCount = queryParam.Dimensions.Count + queryParam.Measures.Count;
//                int startIndex = 0, endIndex = table.RowCount;
//                bool isMatchedIndexExist = (null != matchedIndexes && matchedIndexes.Length > 0);

//                if (isMatchedIndexExist)
//                {
//                    endIndex = matchedIndexes.Length;
//                }
//                for (int i = startIndex; i < endIndex; i++)
//                {
//                    int r = isMatchedIndexExist ? matchedIndexes[i] : i;
//                    var list = new List<object>();
//                    var hKey = string.Empty;
//                    var tempList = new List<object>();
//                    bool isAlreadyExist = false;
//                    foreach (var dim in queryParam.Dimensions)
//                    {
//                        hKey += table.Columns[dim.Name].Values[r];
//                        tempList.Add(table.Columns[dim.Name].Values[r]);
//                    }

//                    if (result1.ContainsKey(hKey))
//                    {
//                        list = result1[hKey];
//                        isAlreadyExist = true;
//                    }
//                    else
//                    {
//                        list = tempList;
//                    }

//                    var curColCount = queryParam.Dimensions.Count;
//                    foreach (var measure in queryParam.Measures)
//                    {
//                        curColCount++;

//                        //var operation = "sum";

//                        var curMeasureDtlList = Utility.GetMeasureDetails(measure);
//                        foreach (var mDetails in curMeasureDtlList)
//                        {
//                            var mName = mDetails.MName;
//                            switch (mDetails.Operation)
//                            {
//                                case "count":
//                                    if (list.Count > curColCount - 1)
//                                    {
//                                        list[curColCount - 1] = Convert.ToInt32(list[curColCount - 1]) + 1;
//                                    }
//                                    else
//                                    {
//                                        list.Add(1);
//                                    }

//                                    break;
//                                case "sum":
//                                    if (list.Count > curColCount - 1)
//                                    {
//                                        list[curColCount - 1] = Convert.ToDouble(list[curColCount - 1]) + Convert.ToDouble(table.Columns[mName].Values[r]);
//                                    }
//                                    else
//                                    {
//                                        list.Add(table.Columns[mName].Values[r]);
//                                    }

//                                    break;
//                            }
//                        }
//                    }

//                    if (!isAlreadyExist)
//                    {
//                        result1.Add(hKey, list);
//                    }

//                }
//                //  queryResult.Result = result1.Select(a => a.Value).ToArray();

//                // int t = 0;
//            }

//            return result1;
//        }


//        private int[] ApplyFilters(QueryParam queryParam)
//        {
//            int[] matchedIndexes = null;
//            //if filter Exist
//            if (queryParam.Filters.Count > 0)
//            {
//                queryParam.Filters[0].TableName = "employees"; //Todo : Remove hardcoded
//                var table = this.Tables[queryParam.Filters[0].TableName];

//                var filterCol = table.Columns[queryParam.Filters[0].ColName];
//                matchedIndexes = filterCol.Values.FindIndexes<object>(v => queryParam.Filters[0].Values.Contains(v) == true).ToArray();
//                //filter Exist
//                for (int f = 1; f < queryParam.Filters.Count; f++)// (var filter  in queryParam.Filters)
//                {
//                    var finalMatchedIndexes = new List<int>();
//                    var filter = queryParam.Filters[f];
//                    filterCol = table.Columns[filter.ColName];

//                    for (int m = 0; m < matchedIndexes.Length; m++)
//                    {
//                        var mIndex = matchedIndexes[m];
//                        if (filter.Values.Contains(filterCol.Values[mIndex]))
//                        {
//                            finalMatchedIndexes.Add(mIndex);
//                        }
//                    }

//                    matchedIndexes = finalMatchedIndexes.ToArray();
//                    //matchedIndexes = filterCol.Values.FindIndexes<object>(v => filter.Values.Contains(v) == true).ToArray();   

//                }
//            }

//            return matchedIndexes;
//        }


//        private List<Table> GetTablesInvoled(QueryParam queryParam)
//        {
//            var tables = new List<string>();

//            tables =  queryParam.Dimensions.Select(d => d.TableName).Distinct().ToList();

//            tables.AddRange(queryParam.Measures.Select(d => d.TableName).Distinct().Where(m => !tables.Contains(m)).ToList());

//            tables.AddRange(queryParam.Filters.Select(d => d.TableName).Distinct().Where(m => !tables.Contains(m)).ToList());

//            List<Table> tableList = new List<Table>();
//            foreach ( var t in tables)
//            {
//                tableList.Add(this.Tables[t]);
//            }
//            return tableList;
//        }
//    }

//    static class Utility
//    {
//        public static List<MeasureDetails> GetMeasureDetails(Measure measure)
//        {
//            var mDetailList = new List<MeasureDetails>();
//            char[] delimiters = new char[] { '/', '+', '-', '*', ')', '(' };
//            var formula = measure.Expression;
//            var formulaParts = formula.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

         
//            var measureDetails = new MeasureDetails();
//            measureDetails.MName = measure.Expression.Replace("sum(", "").Replace(")", "").Trim();
//            if (measure.Expression.IndexOf("sum(") != -1)
//            {
//                measureDetails.Operation = "sum";
//            }
//            else if (measure.Expression.IndexOf("count(") != -1)
//            {
//                measureDetails.Operation = "count";
//            }
//            mDetailList.Add(measureDetails);
           

//            //for (int i = 0; i < formulaParts.Length; i++)
//            //{
//            //    var measureDetails = new MeasureDetails();
//            //    measureDetails.MName = formulaParts[i].Replace("sum(", "").Replace(")", "").Trim();
//            //    if (formulaParts[i].IndexOf("sum(") != -1)
//            //    {                    
//            //        measureDetails.Operation = "sum";
//            //    } else if (formulaParts[i].IndexOf("count(") != -1)
//            //    {
//            //        measureDetails.Operation = "count";
//            //    }
//            //    mDetailList.Add(measureDetails);
//            //}
//            return mDetailList;
//        }

//        public static IEnumerable<int> FindIndexes<T>(this IEnumerable<T> items, Func<T, bool> predicate)
//        {
//            int index = 0;
//            foreach (T item in items)
//            {
//                if (predicate(item))
//                {
//                    yield return index;
//                }

//                index++;
//            }
//        }
//    }

//    class MeasureDetails
//    {
//        public string MName { get; set; }
//        public string Operation { get; set; }
//    }

//    class QueryParam
//    {
//        public QueryParam()
//        {
//            this.Dimensions = new List<Dimension>();
//            this.Measures = new List<Measure>();
//            this.Filters = new List<Filter>();
//        }
//        public List<Dimension>  Dimensions { get; set; }

//        public List<Measure> Measures { get; set; }

//        public List<Filter> Filters { get; set; }
//    }


//    class QueryResult
//    {
//        public QueryResult()
//        {
//            this.Values = new List<List<object>>();
//            this.ColumnNames = new List<string>();
//            this.KeyFields = new Dictionary<string, string>();
//        }

//        public List<string> ColumnNames { get; set; }

//        public List<List<object>> Values { get; set; }

//        public Dictionary<string,string> KeyFields { get; set; }

//        public Dictionary<string, List<object>> Result { get; set; }


//    }

//    class IntermidiateQueryResult
//    {
//        public IntermidiateQueryResult()
//        {
//            this.Values = new Dictionary<string, List<object>>();
//            //this.ColumnNames = new List<string>();
//            this.KeyFields = new Dictionary<string, string>();
//        }

//        //public List<string> ColumnNames { get; set; }

//        public Dictionary<string, List<object>> Values { get; set; }

//        public Dictionary<string, string> KeyFields { get; set; }

//        //public Dictionary<string, List<object>> Result { get; set; }


//    }

//    public class Dimension
//    {
//        private string _dimType = "Simple";

//        public string Name { get; set; }

//        public string Type
//        {
//            get
//            {
//                return _dimType;
//            }
//            set
//            {
//                _dimType = value;
//            }
//        }       

//        public string TableName { get; set; }
//    }

//    public class Measure
//    {
//        //private bool _isVisible = true;

//        private bool _isExpression = true;

//        public string Expression { get; set; }
//        public string DisplayName { get; set; }

//        public bool ShowTotal { get; set; }

//        public bool IsExpression
//        {
//            get
//            {
//                return _isExpression;
//            }
//            set
//            {
//                _isExpression = value;
//            }
//        }

//        public string Type { get; set; }

//        public string TableName { get; set; }
//    }

//    public class Filter
//    {
//        private string _operType = "in";

//        public string ColName { get; set; }

//        public string[] Values { get; set; }

//        public string OperationType
//        {
//            get
//            {
//                return _operType;
//            }
//            set
//            {
//                _operType = value;
//            }
//        }

//        public string TableName { get; set; }
//    }

//}
