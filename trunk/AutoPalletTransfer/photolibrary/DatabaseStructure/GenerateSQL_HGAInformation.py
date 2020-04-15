#!/usr/bin/python
# -*- coding: utf-8 -*-

"""
Generate data by SQL command

Author: LotK
Last modified: 8/19/2015
"""

import sys
from random import randint

sys.stdout = open('HGAInformation.sql', 'w')

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
    for j in range(1, 9):
        for i in range(1, 11):
            attributes = ["ProcessTrayId", "Position", "SerialNumber", "SuspensionLotId", "SliderLotId", "PackId", "Status"]
            data = ["Univer" + str(j), i, randomString(10), randomString(10), randomString(10), k, "None"]
            print generate('hgainformation', attributes, data)
        if j % 3 == 0:
            k += 1

if __name__ == '__main__':
    main()
