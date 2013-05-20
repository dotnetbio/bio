import os
import Util
Util.add_biodotnet_reference("Bio")
from Bio.IO import *
from System.IO import *
from Bio.Util import *

def open_seq(filename):
    "Parses a sequence file, returning a list of ISequence objects."
    filename = filename.Trim('"').Trim('\'')
    if not File.Exists(filename):
        print "\nFile does not exist: " + filename
        return None
    parser = SequenceParsers.FindParserByFileName(filename)
    if parser == None:
        print "\nInvalid file extension: " + filename
        return None
    sequenceList = parser.Parse()
    return Helper.ConvertIenumerableToList(sequenceList)
    
def open_all_seq(dir_name):
    "Parses all of the sequence files in a directory, returning a list of ISequence objects."
    seq_list = []
    for filename in os.listdir(dir_name):
        seq_list.extend(open_seq(filename))
    return seq_list
    
def save_seq(seq_list, filename):
    "Saves a list of ISequence objects to file."
    filename = filename.Trim('"').Trim('\'')
    formatter = SequenceFormatters.FindFormatterByFileName(filename)
    if formatter == None:
        raise Exception, "Failed to recognize sequence file extension: " + filename
    formatter.Write(seq_list)
    formatter.Dispose()
    
def save_all_seq(seq_list, dir_name, file_extension):
    "Saves a list of ISequence objects to separate files."
    for seq in seq_list:
        save_seq(filename_base + "\\" + seq.ID + file_extension, seq)

