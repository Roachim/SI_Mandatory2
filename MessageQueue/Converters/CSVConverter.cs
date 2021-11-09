using System;
using CsvHelper;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Collections;
using System.Text;

namespace MessageQueue.Converters
{
    public static class CSVConverter
    {
        public static IEnumerable DeserializeCSV(string PathToCSVFile)
        {

            TextReader reader = new StreamReader(PathToCSVFile);
            var csvReader = new CsvReader(reader, CultureInfo.CurrentCulture);
            
            var records = csvReader.GetRecords<dynamic>();

            return records;
        }
        public static string SerializeCSV(dynamic records, string pathToMessages){
         
            var csv = new StringBuilder();

            using var textWriter = new StringWriter(csv);
            using var csvWriter = new CsvWriter(textWriter, CultureInfo.CurrentCulture);

            // automatically map the properties of T
            csvWriter.Configuration.AutoMap<T>();

            // write our record
            csvWriter.WriteRecord(input);

            // make sure all records are flushed to stream
            csvWriter.FlushAsync();

            Console.WriteLine(csv.ToString());
           
        }
        // public static string SerializeCSV(dynamic records, string pathToMessages){
        //     string CSVDUMPPath = System.IO.Path.Combine(pathToMessages, "CSVDump.CSV");
        //     System.IO.Directory.CreateDirectory(CSVDUMPPath);
        //     System.IO.File.WriteAllText(CSVDUMPPath, "");

        //     using (var writer = new StreamWriter(CSVDUMPPath))
        //     using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //     {               
        //         csv.WriteRecords(records);
        //     }
        // }
    }
}