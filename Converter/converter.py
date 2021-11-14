from bottle import run, post, request
from json2xml import json2xml
from json2xml.utils import readfromstring
import pandas as pd
import xmltodict
import json
import csv
import io



@post("/convert")
def do():
    messages = json.load(request.body)
    convertToFormat = str(messages['FormatTo']).lower()

    jsonMessages = []
    convertedMessages = []

    for message in messages['Messages']:
        messageText = message['Text']
        match str(message['Format']).lower():
            case 'json':
                jsonMessages.append(messageText)
            case 'xml':
                jsonMessages.append(xmlToJson(messageText))
            case 'csv':
                jsonMessages.append(csvToJson(messageText))
            case 'tsv':
                jsonMessages.append(tsvToJson(messageText))
    
    if convertToFormat == 'json':
        return json.dumps(jsonMessages)
    
    for message in jsonMessages:
        match convertToFormat:
            case 'xml':
                convertedMessages.append(jsonToXml(message))
            case 'csv':
                convertedMessages.append(jsonToCsv(message))
            case 'tsv':
                convertedMessages.append(jsonToTsv(message))
    
    return json.dumps(convertedMessages)

#Functions for converting XML, CSV and TSV to JSON
def xmlToJson(message):
    preparedXml = xmltodict.parse(message)
    return json.dumps(preparedXml)

def csvToJson(message):
    preparedCsv = csv.DictReader(io.StringIO(message))
    return json.dumps(list(preparedCsv))

def tsvToJson(message):
    preparedTsv = pd.read_csv(io.StringIO(message), sep='\t')
    return preparedTsv.to_json()


#Functions for converting JSON to XML, CSV and TSV
def jsonToXml(message):
    preparedJson = readfromstring(message)
    return json2xml.Json2xml(preparedJson).to_xml()

def jsonToCsv(message):
    preparedJson = pd.json_normalize(json.loads(message))
    return preparedJson.to_csv(index=False)

def jsonToTsv(message):
    preparedJson = pd.json_normalize(json.loads(message))
    return preparedJson.to_csv(sep='\t', index=False)



run(host='127.0.0.1', port=6000, reloader=True, server="paste")