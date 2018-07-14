using System;
using System.Collections.Generic;

namespace ColumnStore
{
    class Database
    {
        public List<Table> Tables { get; set; }
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
        public int Key { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }


    class Program
    {
        static List<Dictionary<string, string>> data = new List<Dictionary<string,string>>();
        static Database db = new Database();
        static void Main(string[] args)
        {
            InitializeData();

            BuildDatabase(data);
            Console.ReadKey();

        }

        static void BuildDatabase(List<Dictionary<string, string>> d)
        {
            var table = new Table("employees");

            int key = 1;
            foreach(var row in d)
            {
                Console.WriteLine(row["name"]);
                var column = new Column();
                column.Key = key;
                column.Name = "name";
                column.Value = row["name"];

                table.Columns.Add(column);
            }
        }

        static void InitializeData()
        {
            var row = new Dictionary<string, string>();
            row.Add("name", "rajesh");
            row.Add("city", "mumbai");

            data.Add(row);


            row = new Dictionary<string, string>();
            row.Add("name", "urvashi");
            row.Add("city", "mumbai");
            data.Add(row);


            row = new Dictionary<string, string>();
            row.Add("name", "jai");
            row.Add("city", "delhi");
            data.Add(row);


            row = new Dictionary<string, string>();
            row.Add("name", "anand");
            row.Add("city", "pune");
            data.Add(row);

        }
    }
}
