import Util
Util.add_biodotnet_reference("Bio")
from Bio import *
from Bio.Algorithms import *
from Bio.Algorithms.Alignment import *
from Bio.Algorithms.Assembly import *
from BioIronPython.Algorithms import *
from BioIronPython.IO import *
from BioIronPython.Util import *
from BioIronPython.Web import *
import string
from System.Collections.Generic import List
from Bio.Algorithms.Kmer import *
from Bio.Util import *

_kmerBuilder = SequenceToKmerBuilder()

def ConcatenateSequences():
    "Concatenates sequences present in two files into a single sequence\ne.g. If Sequence1 = 'AAAATGC' and Sequence2 = 'TTTAAG', then the\nConcatenated sequence = 'AAAATGCTTTAAG'"
    
    # This program accepts two file names as input. It parses these two files
    # and concatenates all the sequences present in the files
    
    print ConcatenateSequences.__doc__
    
    again = "y"
    while "yY".find(again[0]) != -1:

        # Get the first file name
        firstFile = GetInputFileName("\nPlease enter the first sequence filename: ")

        # Parse the first list
        firstSet = open_seq(firstFile)
        if firstSet == None:
            firstFile = GetInputFileName("\nInvalid filename. Please enter the first sequence filename: ")
            firstSet = open_seq(firstFile)
            if firstSet == None:
                return None
        
        # Get the second file name
        secondFile = GetInputFileName("\nPlease enter the second sequence filename: ")    

        # Parse the second file
        secondSet = open_seq(secondFile) 
        if secondSet == None:
            secondFile = GetInputFileName("\nInvalid filename. Please enter the second sequence filename: ")
            secondSet = open_seq(secondFile)
            if secondSet == None:
                return None
        
        # Get the output file name
        outPutFile = GetInputFileName("\nPlease enter the target sequence filename: ")
        
        # Stores the string which will have all sequences from the first set concatenated
        firstSetConcatenated = ""
        
        # Stores the string which will have all sequences from the second set concatenated
        secondSetConcatenated = ""
        
        # Concatenate all sequences present in the first file
        for sequence in firstSet:
            firstSetConcatenated += Helper.ConvertSequenceToString(sequence)
        
        # Concatenate all sequences present in the second file    
        for sequence in secondSet:
            secondSetConcatenated += Helper.ConvertSequenceToString(sequence)
        
        # Concatenate these two sequences and create a ISequence object
        finalSet = firstSetConcatenated + secondSetConcatenated
        
        # Create a new sequence object
        concatenatedSeqence = Sequence(Alphabets.DNA, finalSet);
        
        # Save the sequence to the outputFile
        save_seq(concatenatedSeqence, outPutFile)
        
        print "\nThe file which contains the concatenated sequence has been stored at:", outPutFile

        # prompt to go again
        again = " "
        while "yYnN".find(again[0]) == -1:
            again = raw_input("Would you like to enter another sequence? (y/n): ")
            if len(again) == 0:
                again = " "

def StripNonAlphabets():
    "This script will strip illegal characters (not ACGT, etc.) from a sequence including gaps\ne.g. If your input sequence = 'AA-A-GC'\nThe resulting output will be = 'AAAGC'"
    
    # This program accepts a file names as input. It parses the files
    # and extracts all the sequences present in the file. On each 
    # sequence it removes any non-alphabet character and stores these
    # cleaned up sequences in an output file
    
    print StripNonAlphabets.__doc__
    
    again = "y"
    while "yY".find(again[0]) != -1:

        # Get the input file name.
        inputFile = GetInputFileName("\nPlease enter the sequence filename: ")
        
        # Get a list of all sequences in the string
        firstSet = open_seq(inputFile)
        if firstSet == None:
            inputFile = GetInputFileName("\nInvalid filename. Please enter the sequence filename: ")
            firstSet = open_seq(inputFile)
            if firstSet == None:
                return None

        # Get the output file name.
        outputFile = GetInputFileName("\nPlease enter the target sequence filename: ")
        
        # Stores the modified list
        sequenceList = list()
        
        for sequence in firstSet:
        
            # Flag to check if the sequence contained any non-alphabet character
            sequenceChanged = 0
            sequenceString = Helper.ConvertSequenceToString(sequence)
            newSequence = ""
            for c in sequenceString:
                if c.isalpha() != 0:
                    newSequence += c                
                else:
                    sequenceChanged = 1
                                    
            # Remove the older alphabets and add the new cleaned-up alphabets
            sequence = Sequence(sequence.Alphabet, newSequence)
                
            sequenceList.append(sequence)
            
        # Convert a python list to C-sharp list
        c_sharp_list = List[ISequence](sequenceList)    
        
        # Save the sequence to the outputFile
        save_seq(c_sharp_list, outputFile)
        
        print "\nThe file containing the cleaned up sequence has been stored at:", outputFile

        # prompt to go again
        again = " "
        while "yYnN".find(again[0]) == -1:
            again = raw_input("Would you like to enter another sequence? (y/n): ")
            if len(again) == 0:
                again = " "

def RemovePolyATail():
    "Removes the Poly-A tail from a sequence when the A-tail length is greater than 5. For instance \nInput Sequence = 'TGCCGAAAAAAAAAAAAA'\n Output sequence = 'TGCCG'"
    
    # This program accepts a file names as input. It parses the files
    # and extracts all the sequences present in the file. In these sequences
    # the trailing "A" bases will be removed if the length of trailing "A" bases
    # is greater than 5
    
    print RemovePolyATail.__doc__
    
    again = "y"
    while "yY".find(again[0]) != -1:

        # Get the input file name.
        inputFile = GetInputFileName("\nPlease enter the sequence filename: ")

        # Parse the input file
        firstSet = open_seq(inputFile)
        if firstSet == None:
            inputFile = GetInputFileName("\nInvalid filename. Please enter the sequence filename: ")
            firstSet = open_seq(inputFile)
            if firstSet == None:
                return None
        
        # Get the output file name.
        outputFile = GetInputFileName("\nPlease enter the target sequence filename: ")
        
        # Stores the modified list
        sequenceList = list()
        
        # For all sequence in the set    
        for sequence in firstSet:
            sequenceString = Helper.ConvertSequenceToString(sequence)
            
            # Stores the length of the Poly-A tail
            polyATailLength = 0
            
            # Stores the index of the starting of the Poly-A tail
            endIndex = len(sequenceString) - 1
            
            # Get the length and starting index of the Poly-A tail
            while sequenceString[endIndex] == "A":
                  polyATailLength = polyATailLength + 1
                  endIndex = endIndex - 1
                  
            # If Poly-A tail length is greater than 5, then trim the Poly-A tail
            if  polyATailLength > 5:
                newsequence = Helper.RemoveSequencePolyTail(sequence, endIndex+1, polyATailLength)
            else:
                newsequence = sequence
                
            sequenceList.append(newsequence)
        
        # Convert a python list to C-sharp list
        c_sharp_list = List[ISequence](sequenceList)    
        
        # Save the sequence to the outputFile
        save_seq(c_sharp_list, outputFile)
        
        print "\nThe file containing the trimmed Poly-A tail is stored at:", outputFile
        
        # prompt to go again
        again = " "
        while "yYnN".find(again[0]) == -1:
            again = raw_input("Would you like to enter another sequence? (y/n): ")
            if len(again) == 0:
                again = " "
    
def DiffSeq():
    "Reads two sequences which typically are very similar or almost identical. It finds regions of overlap between the two sequences and reports on differences between the features of the two sequences within these regions.\n For e.g Sequence1='AAAAAA' and Sequence2='AAATAA'.\n The output is 'Insertion of 1 bases in 2' "
    print DiffSeq.__doc__ 

    again = "y"
    while "yY".find(again[0]) != -1:

        # Get the first file name
        firstFile = GetInputFileName("\nPlease enter the first sequence filename: ")
        try:
            # Parse the first list
            first_Sequence = open_seq(firstFile)[0]
            if first_Sequence == None:
                firstFile = GetInputFileName("\nInvalid filename. Please enter the first sequence filename: ")
                first_Sequence = open_seq(firstFile)[0]
                if first_Sequence == None:
                    return None
        
            # Get the second file name
            secondFile = GetInputFileName("\nPlease enter the second sequence filename: ")

            # Parse the second file
            second_Sequence = open_seq(secondFile)[0]
            if second_Sequence == None:
                secondFile = GetInputFileName("\nInvalid filename. Please enter the second sequence filename: ")
                second_Sequence = open_seq(secondFile)[0]
                if second_Sequence == None:
                    return None
        
            # Get a list of all KMER's.
            c_sharp_list = _kmerBuilder.Build(first_Sequence, 2);
        
            # Get all the matches. 
            nodes = WordMatch.BuildMatchTable(c_sharp_list, second_Sequence, 2);
        
            c_sharp_list = List[WordMatch](nodes) 
        
            # Get the minimal list of matches. 
            matchList = WordMatch.GetMinimalList(c_sharp_list, 2);
        
            c_sharp_list = List[WordMatch](matchList) 
        
            # Get the difference between the two nodes.
            diffNode = DifferenceNode.BuildDiffList(c_sharp_list, first_Sequence, second_Sequence);
        
            c_sharp_list = List[DifferenceNode](diffNode) 
        
            # Get the features of the nodes.
            features = DifferenceNode.OutputDiffList(c_sharp_list, first_Sequence, second_Sequence);
        
            for feature in features:
                print "\n", feature.Feature
        except:
            print "An error occurred: " + `sys.exc_info()` + "\n"

        # prompt to go again
        again = " "
        while "yYnN".find(again[0]) == -1:
            again = raw_input("Would you like to enter another sequence? (y/n): ")
            if len(again) == 0:
                again = " "
