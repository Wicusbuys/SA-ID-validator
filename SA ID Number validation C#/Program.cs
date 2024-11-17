using System.Globalization;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        DateTime startTime = DateTime.Now;

        if (args.Length != 1 || !int.TryParse(args[0] , out int num))
        {
            Console.WriteLine("Usage: dotnet run <number>");
            return;
        }

        GenerateSAIDNumbers(num);
        validateSAIDNumbers();

        DateTime endTime = DateTime.Now;
        Console.WriteLine($"{(endTime - startTime).TotalSeconds}s");
    }

    public static void GenerateSAIDNumbers(int num)
    {
        Console.WriteLine("How many South African ID's would you like to generate?");
        Console.Write("Amount = ");

        //Generate ID's and store in list
        Random random = new Random();
        var records = new List<string>();
        StringBuilder SAID = new StringBuilder();

        for (int i = 0; i < num; i++)
        {
            SAID.Append($"{random.Next(0, 100):00}"); //YY
            SAID.Append($"{random.Next(0, 13):00}");  //MM
            SAID.Append($"{random.Next(0, 32):00}");  //dd
            SAID.Append($"{random.Next(0, 10_000):0000}");
            SAID.Append($"{random.Next(0, 3):0}");
            SAID.Append($"{random.Next(0, 10):0}");
            SAID.Append($"{random.Next(0, 10):0}");
            
            records.Add(SAID.ToString());
            SAID.Clear();
        }

        //Write ID's to csv
        using StreamWriter writer = new StreamWriter("data.csv");
        foreach (string record in records)
        {
            writer.Write($"{record},");
        }

        Console.WriteLine("File created!");
    }

    public static void validateSAIDNumbers()
    {
        //Read file and store in list
        using StreamReader reader = new StreamReader("data.csv");
        var records = reader.ReadToEnd().Split(",").ToList();

        //Remove invalid errors.
         records.RemoveAll(record => 
                !record.All(char.IsDigit) ||
                record.Length != 13 ||
                !isValidDate(record.Substring(0,6)) ||
                Convert.ToInt32(record.Substring(11, 1)) < 8 ||
                Convert.ToInt32(record.Substring(10, 1)) > 2
            );
        
        //Write ID's to csv
        using StreamWriter writer = new StreamWriter("validated_SAIDs.csv");
        foreach (string record in records)
        {
            writer.Write($"{record},");
        }

        Console.WriteLine("Validated file created!");

    }

    public static bool isValidDate(string date)
    {
       return DateTime.TryParseExact(date, "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }
}