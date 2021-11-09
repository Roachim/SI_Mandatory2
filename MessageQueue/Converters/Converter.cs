using System;
using CsvHelper;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Collections;

namespace MessageQueue.Converters
{
    public static class Converter
    {
        //Ideas for coverting------------------------------------ Types = CSV, JSON, XML, TSV, optional YAML
        //1. have converters From every type TO every type (number of formats ^ number of formats =  total amount of converters)
        //2. Have 1 central type e.g JSON, which evry type can become. 
        //Json can then become any type ( number of formats * 1(JSON) + 1(JSON) * number of converters = number of converters)
        //3. Have a method that registers a format and converts to the then wished for format. (1 converter)
        public static string Convert(string filePath, string toFormat){
            //Find the format of the file by looking after the '.'
            //get path to messages
        }
        
        
        
        
        
    }
}
