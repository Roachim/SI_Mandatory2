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

    #Loops through all the received messages and converts them to json
    for message in messages['Messages']:
        messageText = message['Text']
        #If a message's current format matches the desired format then it will be added to the conversion dictionary
        #as a key with the value false, which means that the conversion should skip this message.
        if str(message['Format']).lower() == convertToFormat:
            messagesToConvert.update({messageText: "false"})
            continue
        match str(message['Format']).lower():
            case 'json':
                messagesToConvert.update({messageText: "true"})
            case 'xml':
                messagesToConvert.update({xmlToJson(messageText): "true"})
            case 'csv':
                messagesToConvert.update({csvToJson(messageText): "true"})
            case 'tsv':
                messagesToConvert.update({tsvToJson(messageText): "true"})

    #If the desired format is Json, all keys from the dictionary will be added to a list,
    #which then gets json serialized and returned
    if convertToFormat == 'json':
        return json.dumps(list(messagesToConvert.keys()))
    
    #Loops through the conversion dictionary and converts all keys with a value of true to the desired format
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
    preparedTsv = csv.DictReader(io.StringIO(message), delimiter='\t')
    return json.dumps(list(preparedTsv))


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