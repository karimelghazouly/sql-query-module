using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryModule.QueryParser;
using System.Xml.Serialization;
using System.IO;

namespace QueryModule.FileManager
{
    public class XMLDatabase
    {
        public XMLTable[] Tables { get; set; }
    }
    public class XMLTable
    {
        public string Name { get; set; }
        public Field[] Fields { get; set; }
        public Row[] Rows { get; set; }
    }
    public class Field
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
    public class Row
    {
        public Cell[] Cells { get; set; }
    }
    public class Cell
    {
        public string StringValue { get; set; }
        public float FloatValue { get; set; }
    }
    public class XMLParserException : Exception
    {
        public XMLParserException(string message) : base(message) { }
    }
    class XMLParser
    {
        private static XMLDatabase DB;
        private static string DatabasePath;

        private static void LoadDatabase()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLDatabase));
            DB = (XMLDatabase)serializer.Deserialize(File.OpenRead(DatabasePath));
        }

        public static void SetDatabasePath(string path)
        {
            DatabasePath = path;
            DB = null;
        }

        private static XMLDatabase GetDatabase()
        {
            if(DB == null)
            {
                LoadDatabase();
            }
            return DB;
        }
        public static List<List<Entity> > getTable(string tableName)
        {
            foreach (XMLTable table in GetDatabase().Tables)
            {
                if(table.Name == tableName)
                {
                    return formatTableForInterpreter(table);
                }
            }
            throw new XMLParserException("Table "+tableName+" not found!");
        }
        private static List<List<Entity>> formatTableForInterpreter(XMLTable table)
        {
            List<List<Entity>> ret = new List<List<Entity>>();
            for (int i = 0; i < table.Rows.Length; i++)
            {
                ret.Add(formatRowForInterpreter(table, table.Rows[i]));
            }
            return ret;
        }

        private static List<Entity> formatRowForInterpreter(XMLTable table, Row row)
        {
            List<Entity> ret = new List<Entity>();
            if(row.Cells.Length != table.Fields.Length)
            {
                throw new XMLParserException("A row has different column values than the table");
            }
            for (int i = 0; i < table.Fields.Length; i++)
            {
                ret.Add(formatCellForInterpreter(table.Fields[i], row.Cells[i]));
            }
            return ret;
        }

        private static Entity formatCellForInterpreter(Field field, Cell cell)
        {
            QueryParser.Type type;
            if (field.Type == "String")
            {
                type = QueryParser.Type.STRING;
                return new Entity(field.Name, type, cell.StringValue);
            } else if (field.Type == "Number")
            {
                type = QueryParser.Type.NUM;
                return new Entity(field.Name, type, cell.FloatValue);
            }
            else
            {
                throw new XMLParserException("Unkown field type " + field.Type);
            }
        }
    }
}
