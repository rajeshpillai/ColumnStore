using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace ColumnStore
{
    class Database
    {
        public List<Table> Tables { get; set; }
        public Database()
        {
            this.Tables = new List<Table>();
        }
    }

    class Table {

        public Table(string name)
        {
            this.Name = name;
            this.Columns = new List<Column>();
        }
        public List<Column> Columns { get; set; }
        public string Name { get; set; }
    }

    class Column {
        public string Name { get; set; }
        public Dictionary<int,string> Values { get; set; }

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
            return string.Format("{0}", Values.ToString());
        }
    }


    class Program
    {
        static Database db = new Database();
        static void Main(string[] args)
        {
            var employee = BuildEmployeeTable();
            Console.WriteLine(employee.Columns.Count);
            Console.ReadKey();
        }


        static Table BuildEmployeeTable()
        {
            var table = new Table("employees");
            db.Tables.Add(table);
            table.Columns.Add(new Column("name"));
            table.Columns.Add(new Column("city"));

            table.Columns[0].Values.Add(1, "jai");
            table.Columns[0].Values.Add(2, "urvashi");
            table.Columns[0].Values.Add(3, "rajesh");
            table.Columns[0].Values.Add(4, "smeeta");

            table.Columns[1].Values.Add(1, "delhi");
            table.Columns[1].Values.Add(2, "mumbai");
            table.Columns[1].Values.Add(3, "kerala");
            table.Columns[1].Values.Add(4, "mumbai");

            return table;
        }

    }
}
