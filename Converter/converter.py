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

    messagesToConvert = {}
    convertedMessages = []

    for message in messages['Messages']:
        messageText = message['Text']
        if str(message['Format']).lower() == convertToFormat:
            messagesToConvert.update({messageText: "false"})
            continue
        match str(message['Format']).lower():
            case 'json':
                messagesToConvert.update({messageText: "true"})
                #jsonMessages.append(messageText)
            case 'xml':
                messagesToConvert.update({xmlToJson(messageText): "true"})
                #jsonMessages.append(xmlToJson(messageText))
            case 'csv':
                messagesToConvert.update({csvToJson(messageText): "true"})
                #jsonMessages.append(csvToJson(messageText))
            case 'tsv':
                messagesToConvert.update({tsvToJson(messageText): "true"})
                #jsonMessages.append(tsvToJson(messageText))
    
    if convertToFormat == 'json':
        return json.dumps(list(messagesToConvert.keys()))
    
    for message in messagesToConvert.items():
        if message[1] == "false":
            convertedMessages.append(message[0])
            continue
        match convertToFormat:
            case 'xml':
                convertedMessages.append(jsonToXml(message[0]))
            case 'csv':
                convertedMessages.append(jsonToCsv(message[0]))
            case 'tsv':
                convertedMessages.append(jsonToTsv(message[0]))
    
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
    preparedTsv2 = csv.DictReader(io.StringIO(message), delimiter='\t')
    #preparedTsv.to_json()
    return json.dumps(list(preparedTsv2))


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