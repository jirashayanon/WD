#!/usr/bin/python
# -*- coding: utf-8 -*-

"""
Generate data by SQL command

Author: LotK
Last modified: 8/19/2015
"""

import sys
from random import randint

sys.stdout = open('HGAInspectionData.sql', 'wt')

def convertStringToSQLString(text):
    if type(text) == type("a"):
        newText = '"' + text + '"'
    elif text == None:
        newText = "null"
    else:
        newText = str(text)
    return newText

def generate(table_name, attributes, data):
    if (len(attributes) != len(data)):
        return None

    command = 'insert into ' + table_name + ' ('
    delimiter = ', '
    attributes_str = delimiter.join(attributes)
    command += attributes_str

    command += ') values ('

    len_data = len(data)
    command += convertStringToSQLString(data[0])
    for i in range(1, len_data):
        command += ", " + convertStringToSQLString(data[i])

    command += ');'

    return command

def randomString(length):
    totalCharacter = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
    listTotal = list(totalCharacter)
    text = []
    for i in range(0, length):
        x = randint(0, len(listTotal))
        text.append(totalCharacter[x-1])
    return "".join(text)

def main():
    k = 4
    list_machine = ["ADM", "ASMC", "AVOR"]
    list_module = ["left", "right"]
    for HGAId in range(6, 86):
        for machine in list_machine:
            for module in list_module:
                attributes = ["HGAId", "Machine", "Module", "Datetime", "StatusFromMachine", "StatusFromOQA"]

                datetime = "2015-8-13"
                if HGAId >= 60:
                    datetime = "2015-8-14"
                status = randint(1, 100)
                if status == 1:
                    status = "Reject"
                else:
                    status = "Good" 
                data = [HGAId, machine, module, datetime, status, "None"]
                print generate('HGAInspectionData', attributes, data)

if __name__ == '__main__':
    main()
