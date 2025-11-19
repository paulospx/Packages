
using Microsoft.VisualBasic.FileIO;
using System.Data;

namespace ConsoleApp6
{
    internal class IngestDatabase
    {
        public void Ingest()
        {
            Console.WriteLine("Ingesting database...");
        }

        public static DataTable ReadCsvToDataTable(string filePath, bool hasHeader = true)
        {
            var dt = new DataTable();
            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;

                string[] columns = [];

                if (hasHeader)
                {
                    columns = parser.ReadFields();
                    foreach (string column in columns)
                    {
                        dt.Columns.Add(column);
                    }
                }

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    if (columns == null)
                    {
                        // First row is data, create columns
                        columns = new string[fields.Length];
                        for (int i = 0; i < fields.Length; i++)
                        {
                            columns[i] = "Column" + i;
                            dt.Columns.Add(columns[i]);
                        }
                    }
                    dt.Rows.Add(fields);
                }
            }
            return dt;
        }






    }
}
