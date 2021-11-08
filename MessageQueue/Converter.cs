using System;

namespace MessageQueue
{
    public class Converter
    {
        //Ideas for coverting------------------------------------ Types = CSV, JSON, XML, TSV, optional YAML
        //1. have converters From every type TO every type (format ^ number of formats =  total amount of converters)
        //2. Have 1 central type e.g JSON, which evry type can become. 
        //Json can then become any type ( number of formats * 1(JSON) + 1(JSON) * number of converters = number of converters)
        //3. Have a method that registers a format and converts to the then wished for format. (1 converter)
        public Converter(string fromFormat, string toFormat){

        }

        
    }
}
