using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace ColumnStore
{
    /*
	    An Experimental Columnar DB using c#
	    Author: Rajesh Pillai
    */

    class Database
    {
        public Dictionary<string, Table> Tables { get; set; }
        public Database()
        {
            this.Tables = new Dictionary<string, Table>();
        }

        public object Query(dynamic param)
        {
            var result = new Dictionary<int, string>();

            Console.WriteLine(param);
            var table = this.Tables[param.Table];
            var column = table.Columns[param.Field];
            var filterValue = param.Value;
            var select = param.Select;

            var selectColumn = table.Columns[select];

            Console.WriteLine(table.Columns.Keys);

            StringBuilder record = new StringBuilder();


            for (int row = 0; row < selectColumn.Values.Count; row++)
            {
                foreach (var col in table.Columns)
                {
                    //Console.WriteLine(col.Value);
                    record.Append(col.Value.Values[row] + ",");
                }
                record.AppendLine();
            }
            Console.WriteLine(record.ToString());

            /*
            foreach(var c in table.Columns) {
                Console.WriteLine(c.Key);
                foreach(var val in c.Value.Values) {
                    if (val.Value.ToLower() == filterValue.ToLower()) {
                        result.Add(val.Key, selectColumn.Values[val.Key]);
                    }
                }
            }
            */

            return result;
        }
    }

    class Table
    {

        public Table(string name)
        {
            this.Name = name;
            this.Columns = new Dictionary<string, Column>();
        }
        public Dictionary<string, Column> Columns { get; set; }


        public string Name { get; set; }

        /*
        public string this[int i]
        {
            get {
                return names[i];
            }
            set {
                names[i] = value;
            }
        }
        */



        public DataTable ToDataTable()
        {
            DataTable result = new DataTable();

            if (Columns.Count == 0)
                return result;

            foreach (KeyValuePair<string, Column> col in this.Columns)
            {
                result.Columns.Add(col.Key);
            }

            bool processed = false;

            int i = 0;
            int cols = this.Columns.Count
                ;
            foreach (KeyValuePair<string, Column> col in this.Columns)
            {
                foreach (var value in col.Value.Values)
                {
                    DataRow row = null;
                    if (!processed)
                    {
                        row = result.NewRow();
                    }
                    else
                    {
                        row = result.Rows[i++];
                    }
                    row[col.Key] = value.Value;


                    if (!processed) result.Rows.Add(row);
                }
                processed = true;
                i = 0;
            }
            return result;
        }
    }

    class Column
    {
        public string Name { get; set; }
        public Dictionary<int, string> Values { get; set; }

        public Column()
        {
            this.Values = new Dictionary<int, string>();
        }

        public Column(string Name) : this()
        {
            this.Name = Name;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var col in this.Values)
            {
                sb.Append(col.Value);
            }
            return string.Format("{0}", sb.ToString());
        }
    }

    class Program
    {
        static Database db = new Database();

        static void Run()
        {
            db = new Database();
            var employees = BuildEmployeeTable();
            var skills = BuildSkillsTable();

            var query = new
            {
                Table = "employees",
                Field = "city",
                Operator = "=",
                Value = "chennai",
                Select = "ename"
            };

            var mumbai = db.Query(query);
            Console.WriteLine(mumbai);
            Console.ReadLine();
        }


        static Table BuildEmployeeTable()
        {
            var table = new Table("employees");
            db.Tables.Add("employees", table);
            table.Columns.Add("ename", new Column("ename"));
            table.Columns.Add("city", new Column("city"));
            table.Columns.Add("empid", new Column("empid"));


            for (int i = 0; i < 10; i++)
            {
                table.Columns["ename"].Values.Add(i, "name " + i.ToString());
                table.Columns["city"].Values.Add(i, (i % 2 == 0 ? "mumbai" : "chennai"));
                table.Columns["empid"].Values.Add(i, i.ToString());

            }
            return table;
        }

        static Table BuildSkillsTable()
        {
            var table = new Table("skills");
            db.Tables.Add("skills", table);
            table.Columns.Add("empid", new Column("empid"));
            table.Columns.Add("skill", new Column("skill"));


            for (int i = 0; i < 10; i++)
            {
                table.Columns["empid"].Values.Add(i, i.ToString());
                table.Columns["skill"].Values.Add(i, (i % 2 == 0 ? "nodejs" : ".net"));

            }
            return table;
        }
    }
}
