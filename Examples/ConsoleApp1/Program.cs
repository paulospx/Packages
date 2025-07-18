using ConsoleApp1;

class Program
{
    private static readonly string watchFolder = @"C:\\temp";

    static void Main(string[] args)
    {

        var classcode = CsvClassGenerator.GenerateClassFromCsv(
            Path.Combine(watchFolder, "C:\\temp\\MOCK_DATA.csv"),
            className: "Person",
            sampleSize: 100
        );

        Console.WriteLine(classcode);


        var sqlcode = CsvSqlGenerator.GenerateCreateTableSql(
            Path.Combine(watchFolder, "C:\\temp\\MOCK_DATA.csv"),
            tableName: "Person",
            sampleSize: 100
        );

        Console.WriteLine(sqlcode);

        Console.ReadLine();


    }
}