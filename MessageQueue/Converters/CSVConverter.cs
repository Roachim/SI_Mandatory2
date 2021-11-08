using System;
using CsvHelper;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Collections;

namespace MessageQueue
{
    public static class CSVConverter
    {
        private static IEnumerable DeserializeCSV(string CSV)
        {

            TextReader reader = new StreamReader(CSV);
            var csvReader = new CsvReader(reader, CultureInfo.CurrentCulture);
            
            var records = csvReader.GetRecords<dynamic>();

            return records;
        }
    }
}